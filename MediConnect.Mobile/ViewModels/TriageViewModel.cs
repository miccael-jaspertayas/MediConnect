using System.Collections.ObjectModel;
using System.Windows.Input;
using MediConnect.Mobile.Dtos;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class TriageViewModel : BindableObject
    {
        private readonly ExternalApiService _externalApiService;
        private readonly ApiService _apiService;
        private readonly SessionService _session;
        private CancellationTokenSource? _debounceCts;

        private int _historyPage = 1;
        private const int HistoryPageSize = 10;
        private bool _hasMoreHistory = true;

        private int? _editingLogId = null; // null = adding new, set = editing existing

        public ObservableCollection<string> Suggestions { get; } = new();
        public ObservableCollection<string> SelectedSymptoms { get; } = new();
        public ObservableCollection<TriageLogResponse> History { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); OnSearchTextChanged(value); }
        }

        private string _resultTier = string.Empty;
        public string ResultTier
        {
            get => _resultTier;
            set { _resultTier = value; OnPropertyChanged(); OnPropertyChanged(nameof(ResultColor)); }
        }

        private string _resultExplanation = string.Empty;
        public string ResultExplanation
        {
            get => _resultExplanation;
            set { _resultExplanation = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private bool _isHistoryLoading;
        public bool IsHistoryLoading
        {
            get => _isHistoryLoading;
            set { _isHistoryLoading = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); OnPropertyChanged(nameof(LogButtonText)); }
        }

        public string LogButtonText => IsEditing ? "Update Log" : "Add/Log Symptom(s)";

        public Color ResultColor => ResultTier switch
        {
            "Emergency" => Colors.Red,
            "Hospital" => Colors.Orange,
            "LocalHealthUnit" => Colors.Green,
            _ => Colors.Gray
        };

        public ICommand SelectSymptomCommand { get; }
        public ICommand RemoveSymptomCommand { get; }
        public ICommand CheckSymptomsCommand { get; }
        public ICommand LogSymptomsCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand EditLogCommand { get; }
        public ICommand DeleteLogCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand LoadMoreHistoryCommand { get; }

        public TriageViewModel(ExternalApiService externalApiService, ApiService apiService, SessionService session)
        {
            _externalApiService = externalApiService;
            _apiService = apiService;
            _session = session;

            SelectSymptomCommand = new Command<string>(symptom =>
            {
                if (!string.IsNullOrWhiteSpace(symptom) && !SelectedSymptoms.Contains(symptom))
                    SelectedSymptoms.Add(symptom);

                Suggestions.Clear();
                SearchText = string.Empty;
            });

            RemoveSymptomCommand = new Command<string>(symptom => SelectedSymptoms.Remove(symptom));

            CheckSymptomsCommand = new Command(async () => await CheckSymptomsAsync());
            LogSymptomsCommand = new Command(async () => await LogSymptomsAsync());
            CancelEditCommand = new Command(CancelEdit);
            EditLogCommand = new Command<TriageLogResponse>(StartEdit);
            DeleteLogCommand = new Command<TriageLogResponse>(async (log) => await DeleteLogAsync(log));
            ClearAllCommand = new Command(async () => await ClearAllAsync());
            LoadMoreHistoryCommand = new Command(async () => await LoadHistoryAsync());

            _ = LoadHistoryAsync();
        }

        private async void OnSearchTextChanged(string value)
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            try
            {
                await Task.Delay(300, token);
                if (token.IsCancellationRequested) return;

                if (string.IsNullOrWhiteSpace(value))
                {
                    Suggestions.Clear();
                    return;
                }

                var results = await _externalApiService.SearchSymptomsAsync(value);
                Suggestions.Clear();
                foreach (var r in results.Take(10))
                    Suggestions.Add(r);
            }
            catch (TaskCanceledException) { }
        }

        // Pure lookup -- does not save anything
        private async Task CheckSymptomsAsync()
        {
            if (SelectedSymptoms.Count == 0)
            {
                ResultTier = string.Empty;
                ResultExplanation = "Please select at least one symptom.";
                return;
            }

            IsLoading = true;
            try
            {
                var request = new TriageRequest { PatientID = _session.PatientID, Symptoms = SelectedSymptoms.ToList() };
                var response = await _apiService.PostAsync<TriageRequest, TriageResponse>("api/triage/assess", request);

                if (response is null)
                {
                    ResultTier = string.Empty;
                    ResultExplanation = "Unable to check symptoms right now. Please try again.";
                }
                else
                {
                    ResultTier = response.Tier;
                    ResultExplanation = response.Explanation;
                }
            }
            catch (Exception)
            {
                ResultTier = string.Empty;
                ResultExplanation = "Unable to check symptoms right now. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Explicit save (or update, if _editingLogId is set)
        private async Task LogSymptomsAsync()
        {
            if (SelectedSymptoms.Count == 0)
            {
                await ShowAlertAsync("Nothing to log", "Please select at least one symptom first.");
                return;
            }

            IsLoading = true;
            try
            {
                var request = new TriageRequest { PatientID = _session.PatientID, Symptoms = SelectedSymptoms.ToList() };

                if (_editingLogId is int id)
                {
                    var updated = await _apiService.PutAsync($"api/triage/log/{id}", request);
                    if (!updated)
                    {
                        await ShowAlertAsync("Update failed", "Could not update this log entry. Please try again.");
                        return;
                    }
                }
                else
                {
                    var created = await _apiService.PostAsync<TriageRequest, TriageLogResponse>("api/triage/log", request);
                    if (created is null)
                    {
                        await ShowAlertAsync("Log failed", "Could not log these symptoms. Please try again.");
                        return;
                    }
                }

                // Refresh history from page 1 so the new/updated entry shows correctly
                _historyPage = 1;
                _hasMoreHistory = true;
                History.Clear();
                await LoadHistoryAsync();

                CancelEdit(); // resets editing state, clears selection
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void StartEdit(TriageLogResponse log)
        {
            _editingLogId = log.LogID;
            IsEditing = true;

            SelectedSymptoms.Clear();
            foreach (var s in log.Symptoms)
                SelectedSymptoms.Add(s);

            ResultTier = log.Tier;
            ResultExplanation = log.Explanation;
        }

        private void CancelEdit()
        {
            _editingLogId = null;
            IsEditing = false;
            SelectedSymptoms.Clear();
            ResultTier = string.Empty;
            ResultExplanation = string.Empty;
        }

        private async Task DeleteLogAsync(TriageLogResponse log)
        {
            bool confirm = await ShowConfirmAsync("Delete entry?",
                $"Remove this logged check-in from {log.CreatedAt:MMM dd, yyyy}? This cannot be undone.");
            if (!confirm) return;

            var success = await _apiService.DeleteAsync($"api/triage/log/{log.LogID}?patientId={_session.PatientID}");
            if (success)
                History.Remove(log);
            else
                await ShowAlertAsync("Delete failed", "Could not delete this entry. Please try again.");
        }

        private async Task ClearAllAsync()
        {
            if (History.Count == 0) return;

            bool confirm = await ShowConfirmAsync("Clear all history?",
                "This will permanently delete ALL your logged symptom check-ins. This cannot be undone.");
            if (!confirm) return;

            var success = await _apiService.DeleteAsync($"api/triage/history/patient/{_session.PatientID}");
            if (success)
            {
                History.Clear();
                _historyPage = 1;
                _hasMoreHistory = true;
            }
            else
            {
                await ShowAlertAsync("Failed", "Could not clear history. Please try again.");
            }
        }

        private async Task LoadHistoryAsync()
        {
            if (IsHistoryLoading || !_hasMoreHistory) return;
            IsHistoryLoading = true;

            try
            {
                var result = await _apiService.GetAsync<PagedResult<TriageLogResponse>>(
                    $"api/triage/history/patient/{_session.PatientID}?page={_historyPage}&pageSize={HistoryPageSize}");

                if (result != null)
                {
                    foreach (var log in result.Items)
                        History.Add(log);

                    _hasMoreHistory = _historyPage < result.TotalPages;
                    _historyPage++;
                }
            }
            finally
            {
                IsHistoryLoading = false;
            }
        }

        // Simple dialog helpers -- calling MainPage directly here is a pragmatic shortcut
        // for a student project; a stricter MVVM setup would inject a dialog/navigation service.
        private static Task ShowAlertAsync(string title, string message) =>
            Application.Current?.MainPage?.DisplayAlertAsync(title, message, "OK") ?? Task.CompletedTask;

        private static Task<bool> ShowConfirmAsync(string title, string message) =>
            Application.Current?.MainPage?.DisplayAlertAsync(title, message, "Yes, delete", "Cancel") ?? Task.FromResult(false);
    }
}