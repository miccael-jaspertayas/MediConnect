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

        private string _greeting = "Welcome!";
        public string Greeting { get => _greeting; set => SetProperty(ref _greeting, value); }

        private string _latestVitalsSummary = "No vitals recorded yet.";
        public string LatestVitalsSummary { get => _latestVitalsSummary; set => SetProperty(ref _latestVitalsSummary, value); }

        public ICommand GoToVitalsCommand { get; }
        public ICommand GoToRecordsCommand { get; }
        public ICommand GoToTriageCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        public DashboardViewModel(SessionService session, VitalsService vitalsService)
        {
            _session = session;
            _vitalsService = vitalsService;

            GoToVitalsCommand = new Command(async () => await Shell.Current.GoToAsync("//Vitals"));
            GoToRecordsCommand = new Command(async () => await Shell.Current.GoToAsync("//Records"));
            GoToTriageCommand = new Command(async () => await Shell.Current.GoToAsync("//Triage"));
            GoToProfileCommand = new Command(async () => await Shell.Current.GoToAsync("ProfilePage"));
            LogoutCommand = new Command(Logout);

            // Subscribe to real-time additions of new vitals
            _session.OnVitalsUpdated += HandleVitalsUpdated;
        }

        public async void OnAppearing()
        {
            Greeting = "Welcome back!";
            await LoadLatestVitalsSummaryAsync();
        }

        private async Task LoadLatestVitalsSummaryAsync()
        {
            try
            {
                // Retrieve the actual vitals from the API service
                var vitalsList = await _vitalsService.GetVitalsAsync(_session.PatientID);
                var latest = vitalsList.OrderByDescending(v => v.RecordedAt).FirstOrDefault();

                if (latest != null)
                {
                    _session.UpdateMostRecentVital(latest);

                    LatestVitalsSummary = $"Latest Vitals: HR {latest.HeartRate}bpm, BP {latest.SystolicBP}/{latest.DiastolicBP}, Temp {latest.Temperature}°C";
                }
                else
                {
                    LatestVitalsSummary = "No vitals recorded yet.";
                }
            }
            catch
            {
                LatestVitalsSummary = "Unable to fetch vitals summary.";
            }
        }

        private void HandleVitalsUpdated(Vitals updatedVitals)
        {
            
            if (updatedVitals != null)
            {
                
            }
        }

        private void Logout()
        {
            _session.Clear();
            Shell.Current.GoToAsync("//Login");
        }
    }
}
