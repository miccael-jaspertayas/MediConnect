using MediConnect.Mobile.Models;

namespace MediConnect.Mobile.Services
{
    public class RecordsService
    {
        private readonly ApiService _apiService;

        public RecordsService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Get all records for a patient
        public async Task<List<MedicalRecord>> GetRecordsAsync(int patientId)
        {
            return await _apiService.GetAsync<List<MedicalRecord>>
            (
                $"api/records/{patientId}"
            ) ?? new List<MedicalRecord>();
        }

        // Get a single record by RecordID
        public async Task<MedicalRecord?> GetRecordAsync(int recordId)
        {
            return await _apiService.GetAsync<MedicalRecord>
            (
                $"api/records/detail/{recordId}"
            );
        }

        // Add a new record
        public async Task<MedicalRecord?> AddRecordAsync(MedicalRecord record)
        {
            return await _apiService.PostAsync<MedicalRecord, MedicalRecord>
            (
                "api/records",
                record
            );
        }

        // Update an existing record
        public async Task<bool> UpdateRecordAsync(MedicalRecord record)
        {
            return await _apiService.PutAsync
            (
                $"api/records/{record.RecordID}",
                record
            );
        }

        // Delete a record
        public async Task<bool> DeleteRecordAsync(int recordId)
        {
            return await _apiService.DeleteAsync
            (
                $"api/records/{recordId}"
            );
        }
    }
}