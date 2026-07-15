using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
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

        public AddVitalsViewModel(VitalsService vitalsService, SessionService sessionService)
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

        // 1. Change properties to Nullable types to prevent 0 value collision
        private double? _temperature;
        public double? Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private int? _systolicBP;
        public int? SystolicBP
        {
            get => _systolicBP;
            set => SetProperty(ref _systolicBP, value);
        }

        private int? _diastolicBP;
        public int? DiastolicBP
        {
            get => _diastolicBP;
            set => SetProperty(ref _diastolicBP, value);
        }

        private int? _heartRate;
        public int? HeartRate
        {
            get => _heartRate;
            set => SetProperty(ref _heartRate, value);
        }

        private double? _weight;
        public double? Weight
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

        public async Task LoadVitalsAsync(int vitalId)
        {
            IsBusy = true;
            try
            {
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

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                // 1. Validation: Require at least one field
                if (Temperature == null && SystolicBP == null && DiastolicBP == null && HeartRate == null && Weight == null)
                {
                    ErrorMessage = "Please enter at least one vitals measurement.";
                    IsBusy = false;
                    return;
                }

                bool success;

                if (SelectedVitals != null)
                {
                    // --- UPDATE MODE (EDITING EXISTING) ---
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
                    // --- CREATE MODE (NEW RECORD) ---
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
                }

                if (success)
                {
                    // Navigate back to the previous page/history dashboard
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Failed to save vitals to the server. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Save Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }

            System.Diagnostics.Debug.WriteLine($"Saving Vitals for Patient ID: {_sessionService.PatientID}");

            if (_sessionService.PatientID <= 0)
            {
                ErrorMessage = "Error: Invalid Patient Session ID.";
                IsBusy = false;
                return;
            }
        }
    }
}