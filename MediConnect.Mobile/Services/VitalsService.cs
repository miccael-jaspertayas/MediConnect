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
        public async Task<List<VitalsModel>> GetVitalsAsync(int patientId)
        {
            return await _apiService.GetAsync<List<VitalsModel>>
            (
                $"api/vitals/patient/{patientId}"
            ) ?? new List<VitalsModel>();
        }

        // Add a new vitals record
        public async Task<bool> AddVitalsAsync(VitalsModel vitals)
        {
            var result = await _apiService.PostAsync<VitalsModel, VitalsModel>
            (
                "api/vitals",
                vitals
            );

            return result != null;
        }

        // Update an existing vitals record
        public async Task<bool> UpdateVitalsAsync(VitalsModel vitals)
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