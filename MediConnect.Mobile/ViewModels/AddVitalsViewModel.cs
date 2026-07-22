using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class AddVitalsViewModel : BaseViewModel
    {
        private readonly VitalsService _vitalsService;
        private readonly SessionService _sessionService;

        public AddVitalsViewModel(VitalsService vitalsService, SessionService sessionService)
        {
            _vitalsService = vitalsService;
            _sessionService = sessionService;

            SaveCommand = new Command(async () => await SaveAsync());
            DeleteCommand = new Command(async () => await DeleteAsync(), () => IsEditing);
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public bool IsEditing => SelectedVital != null;

        private Vitals? _selectedVital;
        public Vitals? SelectedVital
        {
            get => _selectedVital;
            set
            {
                SetProperty(ref _selectedVital, value);
                OnPropertyChanged(nameof(IsEditing));
                (DeleteCommand as Command)?.ChangeCanExecute();

                if (value != null)
                {
                    RecordedAt = value.RecordedAt;
                    Temperature = value.Temperature?.ToString() ?? string.Empty;
                    SystolicBP = value.SystolicBP?.ToString() ?? string.Empty;
                    DiastolicBP = value.DiastolicBP?.ToString() ?? string.Empty;
                    HeartRate = value.HeartRate?.ToString() ?? string.Empty;
                    SpO2 = value.SpO2?.ToString() ?? string.Empty;
                }
            }
        }

        private DateTime _recordedAt = DateTime.Today;
        public DateTime RecordedAt { get => _recordedAt; set => SetProperty(ref _recordedAt, value); }

        private string _temperature = string.Empty;
        public string Temperature { get => _temperature; set => SetProperty(ref _temperature, value); }

        private string _systolicBP = string.Empty;
        public string SystolicBP { get => _systolicBP; set => SetProperty(ref _systolicBP, value); }

        private string _diastolicBP = string.Empty;
        public string DiastolicBP { get => _diastolicBP; set => SetProperty(ref _diastolicBP, value); }

        private string _heartRate = string.Empty;
        public string HeartRate { get => _heartRate; set => SetProperty(ref _heartRate, value); }

        private string _spO2 = string.Empty;
        public string SpO2 { get => _spO2; set => SetProperty(ref _spO2, value); }

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        // Load an existing vitals entry for editing
        public async Task LoadVitalAsync(int vitalId)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var vital = await _vitalsService.GetVitalAsync(vitalId);

                if (vital != null)
                    SelectedVital = vital;
                else
                    ErrorMessage = "Vital record not found.";
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

        public async Task SaveAsync()
        {
            if (IsBusy) return;

            ErrorMessage = string.Empty;

            // Validation
            if (RecordedAt.Date > DateTime.Today)
            {
                ErrorMessage = "Date cannot be in the future.";
                return;
            }
            if (!double.TryParse(Temperature, out var temp) || temp < 30 || temp > 45)
            {
                ErrorMessage = "Valid temperature (30-45°C) required.";
                return;
            }
            if (!int.TryParse(SystolicBP, out var sys) || sys < 50 || sys > 250)
            {
                ErrorMessage = "Valid systolic BP required.";
                return;
            }
            if (!int.TryParse(DiastolicBP, out var dia) || dia < 30 || dia > 150)
            {
                ErrorMessage = "Valid diastolic BP required.";
                return;
            }
            if (!int.TryParse(HeartRate, out var hr) || hr < 30 || hr > 220)
            {
                ErrorMessage = "Valid heart rate required.";
                return;
            }
            if (!int.TryParse(SpO2, out var spo2) || spo2 < 50 || spo2 > 100)
            {
                ErrorMessage = "Valid SpO2 required.";
                return;
            }

            IsBusy = true;

            try
            {
                if (SelectedVital == null)
                {
                    var vital = new Vitals
                    {
                        PatientID = _sessionService.PatientID,
                        RecordedAt = RecordedAt,
                        Temperature = temp,
                        SystolicBP = sys,
                        DiastolicBP = dia,
                        HeartRate = hr,
                        SpO2 = spo2
                    };

                    var added = await _vitalsService.AddVitalsAsync(vital);

                    if (added == null)
                    {
                        ErrorMessage = "Unable to add vitals.";
                        return;
                    }
                }
                else
                {
                    SelectedVital.RecordedAt = RecordedAt;
                    SelectedVital.Temperature = temp;
                    SelectedVital.SystolicBP = sys;
                    SelectedVital.DiastolicBP = dia;
                    SelectedVital.HeartRate = hr;
                    SelectedVital.SpO2 = spo2;

                    var success = await _vitalsService.UpdateVitalsAsync(SelectedVital);

                    if (!success)
                    {
                        ErrorMessage = "Unable to update vitals.";
                        return;
                    }
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

        public async Task DeleteAsync()
        {
            if (SelectedVital == null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete", "Are you sure you want to delete this entry?", "Yes", "No");
            if (!confirm) return;

            IsBusy = true;

            try
            {
                var success = await _vitalsService.DeleteVitalsAsync(SelectedVital.VitalID);

                if (success)
                    await Shell.Current.GoToAsync("..");
                else
                    ErrorMessage = "Unable to delete vitals.";
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