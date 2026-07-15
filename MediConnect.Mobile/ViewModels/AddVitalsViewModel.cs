using System.Diagnostics;
using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    [QueryProperty(nameof(VitalId), "VitalId")]
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

        private int _vitalId;
        public int VitalId
        {
            get => _vitalId;
            set
            {
                _vitalId = value;
                _ = LoadVitalAsync(value);
            }
        }

        private VitalsModel? _selectedVital;
        public VitalsModel? SelectedVital
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
                    Weight = value.Weight?.ToString() ?? string.Empty;
                }
            }
        }

        public Command<DateTime> SelectDateCommand => new Command<DateTime>((selectedDate) =>
        {
            RecordedAt = selectedDate;
            Debug.WriteLine($"Date updated to: {RecordedAt}");
        });
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

        private string _weight = string.Empty;
        public string Weight { get => _weight; set => SetProperty(ref _weight, value); }

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public async Task LoadVitalAsync(int vitalId)
        {
            IsBusy = true;
            try
            {
                var allVitals = await _vitalsService.GetVitalsAsync(_sessionService.PatientID);
                SelectedVital = allVitals.FirstOrDefault(v => v.VitalID == vitalId);

                if (SelectedVital == null) ErrorMessage = "Vital record not found.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { IsBusy = false; }
        }

        public async Task SaveAsync()
        {
            if (IsBusy) return;

            // Simple Validation
            if (RecordedAt.Date > DateTime.Today)
            {
                ErrorMessage = "Date cannot be in the future.";
                return;
            }
            if (!double.TryParse(Temperature, out var temp) || temp < 30 || temp > 45) { ErrorMessage = "Valid Temp (30-45°C) required."; return; }
            if (!int.TryParse(SystolicBP, out var sys) || sys < 50 || sys > 250) { ErrorMessage = "Valid Systolic BP required."; return; }
            if (!int.TryParse(DiastolicBP, out var dia) || dia < 30 || dia > 150) { ErrorMessage = "Valid Diastolic BP required."; return; }
            if (!int.TryParse(HeartRate, out var hr) || hr < 30 || hr > 220) { ErrorMessage = "Valid Heart Rate required."; return; }
            if (!int.TryParse(Weight, out var wei) || wei < 0 || wei > 1000) { ErrorMessage = "Valid weight required."; return; }

            IsBusy = true;
            try
            {
                var vital = SelectedVital ?? new VitalsModel { PatientID = _sessionService.PatientID };
                vital.Temperature = temp;
                vital.SystolicBP = sys;
                vital.DiastolicBP = dia;
                vital.HeartRate = hr;
                vital.Weight = wei;
                vital.RecordedAt = RecordedAt;

                if (IsEditing)
                    await _vitalsService.UpdateVitalsAsync(vital);
                else
                    await _vitalsService.AddVitalsAsync(vital);

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { IsBusy = false; }
        }

        public async Task DeleteAsync()
        {
            if (SelectedVital == null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete", "Are you sure?", "Yes", "No");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                await _vitalsService.DeleteVitalsAsync(SelectedVital.VitalID);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { IsBusy = false; }
        }
    }
}