using MediConnect.Mobile.Models;

namespace MediConnect.Mobile.Services
{
    public class VitalsService
    {
        private readonly ApiService _apiService;

        public VitalsService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Get all vitals for a patient
        public async Task<List<Vitals>> GetVitalsAsync(int patientId)
        {
            return await _apiService.GetAsync<List<Vitals>>
            (
                $"api/vitals/patient/{patientId}"
            ) ?? new List<Vitals>();
        }

        // Get a single vitals entry by VitalID
        public async Task<Vitals?> GetVitalAsync(int vitalId)
        {
            return await _apiService.GetAsync<Vitals>
            (
                $"api/vitals/{vitalId}"
            );
        }

        // Add a new vitals record
        public async Task<Vitals?> AddVitalsAsync(Vitals vitals)
        {
            return await _apiService.PostAsync<Vitals, Vitals>
            (
                "api/vitals",
                vitals
            );
        }

        // Update an existing vitals record
        public async Task<bool> UpdateVitalsAsync(Vitals vitals)
        {
            return await _apiService.PutAsync
            (
                $"api/vitals/{vitals.VitalID}",
                vitals
            );
        }

        // Delete a vitals record
        public async Task<bool> DeleteVitalsAsync(int vitalId)
        {
            return await _apiService.DeleteAsync
            (
                $"api/vitals/{vitalId}"
            );
        }
    }
}