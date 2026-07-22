using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediConnect.Mobile.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly SessionService _session;
        private readonly VitalsService _vitalsService;
        private readonly ApiService _apiService;

        private string _greeting = "Welcome!";
        public string Greeting { get => _greeting; set => SetProperty(ref _greeting, value); }

        private Vitals? _latestVitals;
        public Vitals? LatestVitals { get => _latestVitals; set => SetProperty(ref _latestVitals, value); }

        private bool _hasVitals;
        public bool HasVitals { get => _hasVitals; set => SetProperty(ref _hasVitals, value); }

        private string _latestVitalsDate = string.Empty;
        public string LatestVitalsDate { get => _latestVitalsDate; set => SetProperty(ref _latestVitalsDate, value); }

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

            _session.OnVitalsUpdated += HandleVitalsUpdated;
        }

        public async void OnAppearing()
        {
            await LoadGreetingAsync();
            await LoadLatestVitalsSummaryAsync();
        }

        private async Task LoadGreetingAsync()
        {
            try
            {
                var patient = await _apiService.GetAsync<Dtos.PatientDto>($"api/patients/{_session.PatientID}");
                Greeting = !string.IsNullOrWhiteSpace(patient?.Name)
                    ? $"Welcome back, {patient.Name.Split(' ')[0]}!"
                    : "Welcome back!";
            }
            catch
            {
                Greeting = "Welcome back!";
            }
        }

        private async Task LoadLatestVitalsSummaryAsync()
        {
            try
            {
                var vitalsList = await _vitalsService.GetVitalsAsync(_session.PatientID);
                var latest = vitalsList.OrderByDescending(v => v.RecordedAt).FirstOrDefault();

                if (latest != null)
                {
                    _session.UpdateMostRecentVital(latest);
                    ApplyLatestVital(latest);
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

        private void HandleVitalsUpdated(Vitals updatedVitals)
        {
            if (updatedVitals != null)
            {
                ApplyLatestVital(updatedVitals);
            }
        }

        private void Logout()
        {
            _session.Clear();
            Shell.Current.GoToAsync("//Login");
        }
    }
}