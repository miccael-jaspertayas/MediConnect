using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class AddVitalsViewModel : BaseViewModel
    {
        private readonly VitalsService _vitalsService;
        private readonly SessionService _sessionService;

        public AddVitalsViewModel(
            VitalsService vitalsService,
            SessionService sessionService)
        {
            _vitalsService = vitalsService;
            _sessionService = sessionService;

            SaveCommand = new Command(async () => await SaveAsync());
        }

        public ICommand SaveCommand { get; }

        // For future editing support
        private VitalsModel? _selectedVitals;
        public VitalsModel? SelectedVitals
        {
            get => _selectedVitals;
            set => SetProperty(ref _selectedVitals, value);
        }

        private DateTime _recordedAt = DateTime.Now;
        public DateTime RecordedAt
        {
            get => _recordedAt;
            set => SetProperty(ref _recordedAt, value);
        }

        private double _temperature;
        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private int _systolicBP;
        public int SystolicBP
        {
            get => _systolicBP;
            set => SetProperty(ref _systolicBP, value);
        }

        private int _diastolicBP;
        public int DiastolicBP
        {
            get => _diastolicBP;
            set => SetProperty(ref _diastolicBP, value);
        }

        private int _heartRate;
        public int HeartRate
        {
            get => _heartRate;
            set => SetProperty(ref _heartRate, value);
        }

        private double _weight;
        public double Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            ErrorMessage = string.Empty;

            // Simple validation
            if (Temperature <= 0 &&
                HeartRate <= 0 &&
                SystolicBP <= 0 &&
                DiastolicBP <= 0 &&
                Weight <= 0)
            {
                ErrorMessage = "Please enter at least one vital sign.";
                return;
            }

            IsBusy = true;

            try
            {
                var vitals = new VitalsModel
                {
                    PatientID = _sessionService.PatientID,
                    RecordedAt = RecordedAt,
                    Temperature = Temperature,
                    SystolicBP = SystolicBP,
                    DiastolicBP = DiastolicBP,
                    HeartRate = HeartRate,
                    Weight = Weight
                };

                var success = await _vitalsService.AddVitalsAsync(vitals);

                if (!success)
                {
                    ErrorMessage = "Unable to save vitals.";
                    return;
                }

                await Shell.Current.GoToAsync("..");
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

        // Placeholder for future editing support
        public Task LoadVitalsAsync(int vitalId)
        {
            return Task.CompletedTask;
        }
    }
}