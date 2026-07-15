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
        private CancellationTokenSource? _debounceCts;

        public ObservableCollection<string> Suggestions { get; } = new();
        public ObservableCollection<string> SelectedSymptoms { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnSearchTextChanged(value);
            }
        }

        private string _resultTier = string.Empty;
        public string ResultTier
        {
            get => _resultTier;
            set
            {
                _resultTier = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ResultColor));
            }
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

        public TriageViewModel(ExternalApiService externalApiService, ApiService apiService)
        {
            _externalApiService = externalApiService;
            _apiService = apiService;

            SelectSymptomCommand = new Command<string>(symptom =>
            {
                if (!string.IsNullOrWhiteSpace(symptom) && !SelectedSymptoms.Contains(symptom))
                    SelectedSymptoms.Add(symptom);

                Suggestions.Clear();
                SearchText = string.Empty;
            });

            RemoveSymptomCommand = new Command<string>(symptom =>
            {
                SelectedSymptoms.Remove(symptom);
            });

            CheckSymptomsCommand = new Command(async () => await CheckSymptomsAsync());
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
            catch (TaskCanceledException)
            {
                // expected when a newer keystroke cancels this pending search
            }
        }

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
                var request = new TriageRequest { Symptoms = SelectedSymptoms.ToList() };
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
    }
}