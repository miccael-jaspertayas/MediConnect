using System.Collections.ObjectModel;
using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class VitalsViewModel : BaseViewModel
    {
        private readonly VitalsService _vitalsService;
        private readonly SessionService _sessionService;

        public VitalsViewModel(
            VitalsService vitalsService,
            SessionService sessionService)
        {
            _vitalsService = vitalsService;
            _sessionService = sessionService;

            Vitals = new ObservableCollection<VitalsModel>();

            LoadVitalsCommand = new Command(async () => await LoadVitalsAsync());
        }

        // Collection displayed in the Vitals page
        public ObservableCollection<VitalsModel> Vitals { get; }

        public ICommand LoadVitalsCommand { get; }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Loads all vitals for the logged-in patient
        public async Task LoadVitalsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                Vitals.Clear();

                var vitals = await _vitalsService.GetVitalsAsync(_sessionService.PatientID);

                foreach (var item in vitals)
                {
                    Vitals.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}