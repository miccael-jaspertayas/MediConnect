using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<List<MedicalRecord>> GetRecordsAsync(int patientId)
        {
            return await _apiService.GetAsync<List<MedicalRecord>>
            (
                $"api/records/{patientId}"
            ) ?? new List<MedicalRecord>();
        }

        public async Task<MedicalRecord?> AddRecordAsync(MedicalRecord record)
        {
            return await _apiService.PostAsync<MedicalRecord, MedicalRecord>
            (
                "api/records",
                record
            );
        }

        public async Task<bool> UpdateRecordAsync(MedicalRecord record)
        {
            return await _apiService.PutAsync
            (
                $"api/records/{record.RecordID}",
                record
            );
        }

        public async Task<bool> DeleteRecordAsync(int recordId)
        {
            return await _apiService.DeleteAsync
            (
                $"api/records/{recordId}"
            );
        }
    }
}