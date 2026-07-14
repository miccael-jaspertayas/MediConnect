using MediConnect.Mobile.Dtos;
using MediConnect.Mobile.Services;
using MediConnect.Mobile.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MediConnect.Mobile.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiService _api;
        private readonly SessionService _session;

        private int _patientId;

        private string _name = string.Empty;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private DateTime _dob = DateTime.Today.AddYears(-25);
        public DateTime Dob { get => _dob; set => SetProperty(ref _dob, value); }

        private string _bloodType = string.Empty;
        public string BloodType { get => _bloodType; set => SetProperty(ref _bloodType, value); }

        private string _allergies = string.Empty;
        public string Allergies { get => _allergies; set => SetProperty(ref _allergies, value); }

        private string _medications = string.Empty;
        public string Medications { get => _medications; set => SetProperty(ref _medications, value); }

        private string _emergencyContactName = string.Empty;
        public string EmergencyContactName { get => _emergencyContactName; set => SetProperty(ref _emergencyContactName, value); }

        private string _emergencyContactPhone = string.Empty;
        public string EmergencyContactPhone { get => _emergencyContactPhone; set => SetProperty(ref _emergencyContactPhone, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        // Medication lookup result
        private string _medicationLookupResult = string.Empty;
        public string MedicationLookupResult { get => _medicationLookupResult; set => SetProperty(ref _medicationLookupResult, value); }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }

        public ProfileViewModel(ApiService api, SessionService session)
        {
            _api = api;
            _session = session;

            LoadCommand = new Command(async () => await LoadAsync());
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                _patientId = _session.PatientID;
                var dto = await _api.GetAsync<PatientDto>($"api/patients/{_patientId}");
                if (dto is null)
                {
                    StatusMessage = "Could not load profile.";
                    return;
                }

                Name = dto.Name;
                Dob = dto.DOB ?? DateTime.Today.AddYears(-25);
                BloodType = dto.BloodType ?? string.Empty;
                Allergies = dto.Allergies ?? string.Empty;
                Medications = dto.Medications ?? string.Empty;
                EmergencyContactName = dto.EmergencyContactName ?? string.Empty;
                EmergencyContactPhone = dto.EmergencyContactPhone ?? string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                StatusMessage = "Name is required.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmergencyContactPhone) &&
                !Regex.IsMatch(EmergencyContactPhone, @"^[0-9+\-\s()]{7,20}$"))
            {
                StatusMessage = "Emergency contact phone looks invalid.";
                return false;
            }

            return true;
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            StatusMessage = string.Empty;

            if (!Validate()) return;

            IsBusy = true;
            try
            {
                var dto = new PatientDto
                {
                    PatientID = _patientId,
                    Name = Name,
                    DOB = Dob,
                    BloodType = BloodType,
                    Allergies = Allergies,
                    Medications = Medications,
                    EmergencyContactName = EmergencyContactName,
                    EmergencyContactPhone = EmergencyContactPhone
                };

                var success = await _api.PutAsync($"api/patients/{_patientId}", dto);
                StatusMessage = success ? "Profile saved." : "Failed to save profile.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
