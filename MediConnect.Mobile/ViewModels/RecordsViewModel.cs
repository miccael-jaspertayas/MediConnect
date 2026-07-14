using MediConnect.Mobile.Models;
using MediConnect.Mobile.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MediConnect.Mobile.ViewModels
{
    public class RecordsViewModel : BaseViewModel
    {
        private readonly RecordsService _recordsService;
        private readonly SessionService _sessionService;

        private MedicalRecord? _editingRecord;

        public ObservableCollection<MedicalRecord> Records { get; }
            = new ObservableCollection<MedicalRecord>();

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

        public ICommand LoadRecordsCommand { get; }
        public ICommand AddRecordCommand { get; }
        public ICommand DeleteRecordCommand { get; }
        public ICommand EditRecordCommand { get; }

        public RecordsViewModel(
            RecordsService recordsService,
            SessionService sessionService)
        {
            _recordsService = recordsService;
            _sessionService = sessionService;

            LoadRecordsCommand =
                new Command(async () => await LoadRecordsAsync());

            AddRecordCommand =
                new Command(async () => await AddOrUpdateRecordAsync());

            DeleteRecordCommand =
                new Command<MedicalRecord>(async (record) =>
                    await DeleteRecordAsync(record));

            EditRecordCommand =
                new Command<MedicalRecord>(EditRecord);
        }

        public async Task LoadRecordsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                Records.Clear();

                var records =
                    await _recordsService.GetRecordsAsync(_sessionService.PatientID);

                records = records
                    .OrderByDescending(r => r.VisitDate)
                    .ToList();

                foreach (var record in records)
                    Records.Add(record);
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

        private async Task AddOrUpdateRecordAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
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

                var record = new MedicalRecord
                {
                    PatientID = _sessionService.PatientID,
                    VisitDate = VisitDate,
                    HospitalName = HospitalName,
                    DoctorName = DoctorName,
                    Diagnosis = Diagnosis,
                    Notes = Notes
                };

                if (_editingRecord == null)
                {
                    var added =
                        await _recordsService.AddRecordAsync(record);

                    if (added == null)
                    {
                        ErrorMessage = "Unable to add record.";
                        return;
                    }

                    Records.Insert(0, added);
                }
                else
                {
                    record.RecordID = _editingRecord.RecordID;

                    var success =
                        await _recordsService.UpdateRecordAsync(record);

                    if (!success)
                    {
                        ErrorMessage = "Unable to update record.";
                        return;
                    }

                    await LoadRecordsAsync();

                    _editingRecord = null;
                }

                ClearForm();
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

        private void EditRecord(MedicalRecord? record)
        {
            if (record == null)
                return;

            _editingRecord = record;

            VisitDate = record.VisitDate;
            HospitalName = record.HospitalName;
            DoctorName = record.DoctorName;
            Diagnosis = record.Diagnosis;
            Notes = record.Notes;
        }

        private async Task DeleteRecordAsync(MedicalRecord? record)
        {
            if (record == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var success =
                    await _recordsService.DeleteRecordAsync(record.RecordID);

                if (success)
                {
                    Records.Remove(record);
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

        private void ClearForm()
        {
            VisitDate = DateTime.Today;
            HospitalName = string.Empty;
            DoctorName = string.Empty;
            Diagnosis = string.Empty;
            Notes = string.Empty;
        }
    }
}