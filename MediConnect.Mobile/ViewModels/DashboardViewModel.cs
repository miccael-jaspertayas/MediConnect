using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly SessionService _session;

        private string _greeting = "Welcome!";
        public string Greeting { get => _greeting; set => SetProperty(ref _greeting, value); }

        // Placeholder until Fred's Vitals endpoint is live
        private string _latestVitalsSummary = "No vitals recorded yet.";
        public string LatestVitalsSummary { get => _latestVitalsSummary; set => SetProperty(ref _latestVitalsSummary, value); }

        public ICommand GoToVitalsCommand { get; }
        public ICommand GoToRecordsCommand { get; }
        public ICommand GoToTriageCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        public DashboardViewModel(SessionService session)
        {
            _session = session;

            GoToVitalsCommand = new Command(async () => await Shell.Current.GoToAsync("//Vitals"));
            GoToRecordsCommand = new Command(async () => await Shell.Current.GoToAsync("//Records"));
            GoToTriageCommand = new Command(async () => await Shell.Current.GoToAsync("//Triage"));
            GoToProfileCommand = new Command(async () => await Shell.Current.GoToAsync("Profile"));
            LogoutCommand = new Command(Logout);
        }

        public void OnAppearing()
        {
            Greeting = "Welcome back!";
            // TODO (Fred, once Vitals GET exists): call VitalsService here and set LatestVitalsSummary to the most recent entry.
        }

        private void Logout()
        {
            _session.Clear();
            Shell.Current.GoToAsync("//Login");
        }
    }
}
