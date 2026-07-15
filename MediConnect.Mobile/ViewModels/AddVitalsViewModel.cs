using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    // Receive the VitalID query parameter passed during navigation
    [QueryProperty(nameof(VitalId), "Id")]
    public class AddVitalsViewModel : BaseViewModel
    {
        private readonly VitalsService _vitalsService;
        private readonly SessionService _sessionService;

        private string? _vitalId;
        public string? VitalId
        {
            get => _vitalId;
            set
            {
                _vitalId = value;
                if (!string.IsNullOrEmpty(_vitalId) && int.TryParse(_vitalId, out int id))
                {
                    _ = LoadVitalsAsync(id);
                }
            }
        }

        public AddVitalsViewModel(
            VitalsService vitalsService,
            SessionService sessionService)
        {
            _vitalsService = vitalsService;
            _sessionService = sessionService;

            SaveCommand = new Command(async () => await SaveAsync());
        }

        public ICommand SaveCommand { get; }

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

        // Loads existing vitals when a VitalID query parameter is passed
        public async Task LoadVitalsAsync(int vitalId)
        {
            IsBusy = true;
            try
            {
                // We fetch all records and filter by ID, or if you have an API route: api/vitals/{id}
                var list = await _vitalsService.GetVitalsAsync(_sessionService.PatientID);
                var vital = list.Find(v => v.VitalID == vitalId);

                if (vital != null)
                {
                    SelectedVitals = vital;
                    RecordedAt = vital.RecordedAt;
                    Temperature = vital.Temperature;
                    SystolicBP = vital.SystolicBP;
                    DiastolicBP = vital.DiastolicBP;
                    HeartRate = vital.HeartRate;
                    Weight = vital.Weight;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load vitals: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            ErrorMessage = string.Empty;

            // 1. Check if at least some data was entered
            if (Temperature == 0 && HeartRate == 0 && SystolicBP == 0 && DiastolicBP == 0 && Weight == 0)
            {
                ErrorMessage = "Please enter at least one vital sign.";
                return;
            }

            // 2. Validate Temperature (if entered, must be between 34.0°C and 43.0°C)
            if (Temperature != 0 && (Temperature < 34.0 || Temperature > 43.0))
            {
                ErrorMessage = "Please enter a valid temperature (34.0°C - 43.0°C).";
                return;
            }

            // 3. Validate Heart Rate (if entered, must be between 30 and 250 bpm)
            if (HeartRate != 0 && (HeartRate < 30 || HeartRate > 250))
            {
                ErrorMessage = "Please enter a valid heart rate (30 - 250 bpm).";
                return;
            }

            // 4. Validate Blood Pressure (if either is entered, validate both ranges)
            if (SystolicBP != 0 || DiastolicBP != 0)
            {
                if (SystolicBP < 50 || SystolicBP > 250)
                {
                    ErrorMessage = "Please enter a valid Systolic BP (50 - 250 mmHg).";
                    return;
                }
                if (DiastolicBP < 30 || DiastolicBP > 150)
                {
                    ErrorMessage = "Please enter a valid Diastolic BP (30 - 150 mmHg).";
                    return;
                }
            }

            // 5. Validate Weight (if entered, must be a realistic positive number)
            if (Weight != 0 && (Weight < 2.0 || Weight > 500.0))
            {
                ErrorMessage = "Please enter a realistic weight (2 - 500 kg).";
                return;
            }

            IsBusy = true;

            try
            {
                bool success;

                if (SelectedVitals != null)
                {
                    // Update mode
                    SelectedVitals.RecordedAt = RecordedAt;
                    SelectedVitals.Temperature = Temperature;
                    SelectedVitals.SystolicBP = SystolicBP;
                    SelectedVitals.DiastolicBP = DiastolicBP;
                    SelectedVitals.HeartRate = HeartRate;
                    SelectedVitals.Weight = Weight;

                    success = await _vitalsService.UpdateVitalsAsync(SelectedVitals);
                }
                else
                {
                    // Create mode
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

                    success = await _vitalsService.AddVitalsAsync(vitals);

                    if (success)
                    {
                        _sessionService.UpdateMostRecentVital(vitals);
                    }
                }

                if (!success)
                {
                    ErrorMessage = "Unable to save vitals. Connection error.";
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
    }
}