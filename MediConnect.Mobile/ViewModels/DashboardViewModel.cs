using MediConnect.Mobile.Dtos;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;
using Microcharts;
using Microsoft.Maui.Controls;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MediConnect.Mobile.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly SessionService _session;
        private readonly VitalsService _vitalsService;
        private readonly ApiService _apiService;

        private List<Vitals> _allVitals = new();

        private string _greeting = "Welcome!";
        public string Greeting { get => _greeting; set => SetProperty(ref _greeting, value); }

        private Vitals? _latestVitals;
        public Vitals? LatestVitals { get => _latestVitals; set => SetProperty(ref _latestVitals, value); }

        private bool _hasVitals;
        public bool HasVitals { get => _hasVitals; set => SetProperty(ref _hasVitals, value); }

        private string _latestVitalsDate = string.Empty;
        public string LatestVitalsDate { get => _latestVitalsDate; set => SetProperty(ref _latestVitalsDate, value); }

        // History range toggle: 0 = Last 10, 1 = 1 Week, 2 = All Time
        private int _selectedRange;
        public int SelectedRange
        {
            get => _selectedRange;
            set
            {
                if (SetProperty(ref _selectedRange, value))
                    RebuildCharts();
            }
        }

        public ICommand SetRangeCommand { get; }

        private Chart _bpSystolicChart = new LineChart();
        public Chart BpSystolicChart { get => _bpSystolicChart; set => SetProperty(ref _bpSystolicChart, value); }

        private Chart _bpDiastolicChart = new LineChart();
        public Chart BpDiastolicChart { get => _bpDiastolicChart; set => SetProperty(ref _bpDiastolicChart, value); }

        private Chart _heartRateChart = new LineChart();
        public Chart HeartRateChart { get => _heartRateChart; set => SetProperty(ref _heartRateChart, value); }

        private Chart _temperatureChart = new LineChart();
        public Chart TemperatureChart { get => _temperatureChart; set => SetProperty(ref _temperatureChart, value); }

        private Chart _spO2Chart = new LineChart();
        public Chart SpO2Chart { get => _spO2Chart; set => SetProperty(ref _spO2Chart, value); }

        public ObservableCollection<TriageLogResponse> RecentCheckIns { get; } = new();

        private bool _hasCheckIns;
        public bool HasCheckIns { get => _hasCheckIns; set => SetProperty(ref _hasCheckIns, value); }

        public ICommand GoToVitalsCommand { get; }
        public ICommand GoToRecordsCommand { get; }
        public ICommand GoToTriageCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        public DashboardViewModel(SessionService session, VitalsService vitalsService, ApiService apiService)
        {
            _session = session;
            _vitalsService = vitalsService;
            _apiService = apiService;

            GoToVitalsCommand = new Command(async () => await Shell.Current.GoToAsync("//Vitals"));
            GoToRecordsCommand = new Command(async () => await Shell.Current.GoToAsync("//Records"));
            GoToTriageCommand = new Command(async () => await Shell.Current.GoToAsync("//Triage"));
            GoToProfileCommand = new Command(async () => await Shell.Current.GoToAsync("ProfilePage"));
            LogoutCommand = new Command(Logout);
            SetRangeCommand = new Command<string>(r => SelectedRange = int.Parse(r!));

            _session.OnVitalsUpdated += HandleVitalsUpdated;
        }

        public async void OnAppearing()
        {
            await LoadGreetingAsync();
            await LoadVitalsAsync();
            await LoadRecentCheckInsAsync();
        }

        private async Task LoadGreetingAsync()
        {
            try
            {
                var patient = await _apiService.GetAsync<PatientDto>($"api/patients/{_session.PatientID}");
                Greeting = !string.IsNullOrWhiteSpace(patient?.Name)
                    ? $"Welcome, {patient.Name.Split(' ')[0]}!"
                    : "Welcome!";
            }
            catch
            {
                Greeting = "Welcome!";
            }
        }

        private async Task LoadVitalsAsync()
        {
            try
            {
                var vitalsList = await _vitalsService.GetVitalsAsync(_session.PatientID);
                _allVitals = vitalsList.OrderBy(v => v.RecordedAt).ToList(); // chronological, oldest first, for charting

                var latest = _allVitals.LastOrDefault();
                if (latest != null)
                {
                    _session.UpdateMostRecentVital(latest);
                    ApplyLatestVital(latest);
                    RebuildCharts();
                }
                else
                {
                    HasVitals = false;
                    LatestVitals = null;
                }
            }
            catch
            {
                HasVitals = false;
                LatestVitals = null;
            }
        }

        private void ApplyLatestVital(Vitals vital)
        {
            LatestVitals = vital;
            LatestVitalsDate = vital.RecordedAt.ToString("MMM d, yyyy 'at' h:mm tt");
            HasVitals = true;
        }

        private void RebuildCharts()
        {
            if (_allVitals.Count == 0) return;

            IEnumerable<Vitals> filtered = SelectedRange switch
            {
                0 => _allVitals.TakeLast(10),
                1 => _allVitals.Where(v => v.RecordedAt >= DateTime.Now.AddDays(-7)),
                _ => _allVitals
            };

            var points = filtered.ToList();
            if (points.Count == 0)
                points = _allVitals.TakeLast(10).ToList();

            BpSystolicChart = BuildLineChart(points, v => (float)(v.SystolicBP ?? 0), "#1C6F6F");
            BpDiastolicChart = BuildLineChart(points, v => (float)(v.DiastolicBP ?? 0), "#6FA8A3");
            HeartRateChart = BuildLineChart(points, v => (float)(v.HeartRate ?? 0), "#E5484D");
            TemperatureChart = BuildLineChart(points, v => (float)(v.Temperature ?? 0), "#F5A623");
            SpO2Chart = BuildLineChart(points, v => (float)(v.SpO2 ?? 0), "#4A90D9");
        }

        private static LineChart BuildLineChart(List<Vitals> points, Func<Vitals, float> selector, string hexColor)
        {
            var entries = points.Select(v => new ChartEntry(selector(v))
            {
                Label = v.RecordedAt.ToString("M/d"),
                ValueLabel = selector(v).ToString("0.#"),
                Color = SKColor.Parse(hexColor)
            }).ToArray();

            var min = entries.Min(e => e.Value) ?? 0f;
            var max = entries.Max(e => e.Value) ?? 0f;
            var range = max - min;
            var padding = range == 0 ? 1f : range * 0.15f;

            return new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                LineSize = 3,
                PointSize = 6,
                PointMode = PointMode.Circle,
                LabelTextSize = 28,
                BackgroundColor = SKColor.Empty,
                MinValue = min - padding,
                MaxValue = max + padding
            };
        }

        private async Task LoadRecentCheckInsAsync()
        {
            try
            {
                var result = await _apiService.GetAsync<PagedResult<TriageLogResponse>>(
                    $"api/triage/history/patient/{_session.PatientID}?page=1&pageSize=3");

                RecentCheckIns.Clear();

                if (result?.Items != null)
                {
                    foreach (var item in result.Items)
                        RecentCheckIns.Add(item);
                }

                HasCheckIns = RecentCheckIns.Count > 0;
            }
            catch
            {
                HasCheckIns = false;
            }
        }

        private void HandleVitalsUpdated(Vitals updatedVitals)
        {
            if (updatedVitals != null)
            {
                _allVitals.Add(updatedVitals);
                _allVitals = _allVitals.OrderBy(v => v.RecordedAt).ToList();
                ApplyLatestVital(updatedVitals);
                RebuildCharts();
            }
        }

        private void Logout()
        {
            _session.Clear();
            Shell.Current.GoToAsync("//Login");
        }
    }
}