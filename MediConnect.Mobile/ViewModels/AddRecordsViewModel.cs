using System.Windows.Input;
using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;

namespace MediConnect.Mobile.ViewModels
{
    public class AddRecordViewModel : BaseViewModel
    {
        private readonly RecordsService _recordsService;
        private readonly SessionService _sessionService;

        public AddRecordViewModel(
            RecordsService recordsService,
            SessionService sessionService)
        {
            _recordsService = recordsService;
            _sessionService = sessionService;

            SaveCommand = new Command(async () => await SaveAsync());

            DeleteCommand = new Command(
                async () => await DeleteAsync(),
                () => IsEditing);
        }

        public ICommand SaveCommand { get; }

        public ICommand DeleteCommand { get; }

        public bool IsEditing => SelectedRecord != null;

        private MedicalRecord? _selectedRecord;
        public MedicalRecord? SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                SetProperty(ref _selectedRecord, value);

                OnPropertyChanged(nameof(IsEditing));

                (DeleteCommand as Command)?.ChangeCanExecute();

                if (value != null)
                {
                    VisitDate = value.VisitDate;
                    HospitalName = value.HospitalName;
                    DoctorName = value.DoctorName;
                    Diagnosis = value.Diagnosis;
                    Notes = value.Notes;
                }
            }
        }

        private DateTime _visitDate = DateTime.Today;
        public DateTime VisitDate
        {
            get => _visitDate;
            set => SetProperty(ref _visitDate, value);
        }

        private string _hospitalName = string.Empty;
        public string HospitalName
        {
            get => _hospitalName;
            set => SetProperty(ref _hospitalName, value);
        }

        private string _doctorName = string.Empty;
        public string DoctorName
        {
            get => _doctorName;
            set => SetProperty(ref _doctorName, value);
        }

        private string _diagnosis = string.Empty;
        public string Diagnosis
        {
            get => _diagnosis;
            set => SetProperty(ref _diagnosis, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Load an existing record for editing
        public async Task LoadRecordAsync(int recordId)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var record = await _recordsService.GetRecordAsync(recordId);

                if (record != null)
                {
                    SelectedRecord = record;
                }
                else
                {
                    ErrorMessage = "Record not found.";
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

        // Save (Add or Update)
        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            ErrorMessage = string.Empty;

            // Validation
            if (VisitDate > DateTime.Today)
            {
                ErrorMessage = "Visit date cannot be in the future.";
                return;
            }

            if (string.IsNullOrWhiteSpace(HospitalName))
            {
                ErrorMessage = "Hospital name is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(DoctorName))
            {
                ErrorMessage = "Doctor name is required.";
                return;
            }

            IsBusy = true;

            try
            {
                if (SelectedRecord == null)
                {
                    var record = new MedicalRecord
                    {
                        PatientID = _sessionService.PatientID,
                        VisitDate = VisitDate,
                        HospitalName = HospitalName,
                        DoctorName = DoctorName,
                        Diagnosis = Diagnosis,
                        Notes = Notes
                    };

                    var added = await _recordsService.AddRecordAsync(record);

                    if (added == null)
                    {
                        ErrorMessage = "Unable to add record.";
                        return;
                    }
                }
                else
                {
                    SelectedRecord.VisitDate = VisitDate;
                    SelectedRecord.HospitalName = HospitalName;
                    SelectedRecord.DoctorName = DoctorName;
                    SelectedRecord.Diagnosis = Diagnosis;
                    SelectedRecord.Notes = Notes;

                    var success = await _recordsService.UpdateRecordAsync(SelectedRecord);

                    if (!success)
                    {
                        ErrorMessage = "Unable to update record.";
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

        // Delete record
        public async Task DeleteAsync()
        {
            if (SelectedRecord == null)
                return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete", "Are you sure you want to delete this record?", "Yes", "No");
            if (!confirm) return;

            IsBusy = true;

            try
            {
                var success = await _recordsService.DeleteRecordAsync(SelectedRecord.RecordID);

                if (success)
                {
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Unable to delete record.";
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