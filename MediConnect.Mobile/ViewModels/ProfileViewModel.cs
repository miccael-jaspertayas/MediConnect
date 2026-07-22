using MediConnect.Mobile.Dtos;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MediConnect.Mobile.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiService _api;
        private readonly ExternalApiService _externalApi;
        private readonly SessionService _session;

        private int _patientId;

        private string _name = string.Empty;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private DateTime _dob = DateTime.Today.AddYears(-25);
        public DateTime Dob { get => _dob; set => SetProperty(ref _dob, value); }

        public List<string> BloodTypeOptions { get; } = new()
        {
            "Unknown", "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };

        private string _bloodType = "Unknown";
        public string BloodType { get => _bloodType; set => SetProperty(ref _bloodType, value); }

        private string _allergies = string.Empty;
        public string Allergies { get => _allergies; set => SetProperty(ref _allergies, value); }

        public ObservableCollection<MedicationEntry> Medications { get; } = new();

        public ICommand AddMedicationCommand { get; }
        public ICommand RemoveMedicationCommand { get; }

        private string _emergencyContactName = string.Empty;
        public string EmergencyContactName { get => _emergencyContactName; set => SetProperty(ref _emergencyContactName, value); }

        private string _emergencyContactPhone = string.Empty;
        public string EmergencyContactPhone { get => _emergencyContactPhone; set => SetProperty(ref _emergencyContactPhone, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _isSaveSuccess;
        public bool IsSaveSuccess { get => _isSaveSuccess; set => SetProperty(ref _isSaveSuccess, value); }

        public DateTime MaxDob { get; } = DateTime.Today;

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LookupMedicationCommand { get; }

        public ProfileViewModel(ApiService api, SessionService session, ExternalApiService externalApi)
        {
            _api = api;
            _session = session;
            _externalApi = externalApi;

            LoadCommand = new Command(async () => await LoadAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            LookupMedicationCommand = new Command(async () => await LookupAllMedicationsAsync());
            AddMedicationCommand = new Command(AddMedicationField);
            RemoveMedicationCommand = new Command<MedicationEntry>(RemoveMedicationField);

            // Always start with at least one field
            Medications.Add(new MedicationEntry());
        }

        private void AddMedicationField()
        {
            Medications.Add(new MedicationEntry());
        }

        private void RemoveMedicationField(MedicationEntry? entry)
        {
            if (entry == null) return;

            // Never let the list drop to zero fields
            if (Medications.Count <= 1)
            {
                Medications[0].Name = string.Empty;
                Medications[0].LookupResult = null;
                Medications[0].LookupStatusMessage = string.Empty;
                return;
            }

            Medications.Remove(entry);
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
                Dob = (dto.DOB.HasValue && dto.DOB.Value <= MaxDob) ? dto.DOB.Value : DateTime.Today.AddYears(-25);
                BloodType = string.IsNullOrWhiteSpace(dto.BloodType) ? "Unknown" : dto.BloodType;
                Allergies = dto.Allergies ?? string.Empty;
                EmergencyContactName = dto.EmergencyContactName ?? string.Empty;
                EmergencyContactPhone = dto.EmergencyContactPhone ?? string.Empty;

                // Split the saved comma-separated string back into individual fields
                Medications.Clear();
                var savedMeds = (dto.Medications ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

                if (savedMeds.Count == 0)
                {
                    Medications.Add(new MedicationEntry());
                }
                else
                {
                    foreach (var med in savedMeds)
                        Medications.Add(new MedicationEntry { Name = med });
                }
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
                IsSaveSuccess = false;
                StatusMessage = "Name is required.";
                return false;
            }

            if (Dob > MaxDob)
            {
                IsSaveSuccess = false;
                StatusMessage = "Date of birth cannot be in the future.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmergencyContactPhone) &&
                !Regex.IsMatch(EmergencyContactPhone, @"^[0-9+\-\s()]{7,20}$"))
            {
                IsSaveSuccess = false;
                StatusMessage = "Emergency contact phone looks invalid.";
                return false;
            }

            return true;
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            StatusMessage = string.Empty;

            if (!Validate())
            {
                IsSaveSuccess = false;
                return;
            }

            IsBusy = true;
            try
            {
                // Join non-empty medication fields into one comma-separated string
                var medicationsString = string.Join(", ",
                    Medications
                        .Select(m => m.Name.Trim())
                        .Where(name => !string.IsNullOrWhiteSpace(name)));

                var dto = new PatientDto
                {
                    PatientID = _patientId,
                    Name = Name,
                    DOB = Dob,
                    BloodType = BloodType == "Unknown" ? null : BloodType,
                    Allergies = Allergies,
                    Medications = medicationsString,
                    EmergencyContactName = EmergencyContactName,
                    EmergencyContactPhone = EmergencyContactPhone
                };

                var success = await _api.PutAsync($"api/patients/{_patientId}", dto);

                IsSaveSuccess = success;
                StatusMessage = success ? "Profile saved." : "Failed to save profile.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LookupAllMedicationsAsync()
        {
            if (IsBusy) return;

            var entriesToLookup = Medications.Where(m => !string.IsNullOrWhiteSpace(m.Name)).ToList();

            if (entriesToLookup.Count == 0)
            {
                StatusMessage = "Enter at least one medication first.";
                return;
            }

            IsBusy = true;
            try
            {
                // Look up each medication independently so one failure
                // doesn't block the others, and each field gets its own result.
                foreach (var entry in entriesToLookup)
                {
                    var result = await _externalApi.LookupMedicationAsync(entry.Name);
                    entry.LookupResult = result;
                    entry.LookupStatusMessage = result.Found ? string.Empty : "No info found.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}