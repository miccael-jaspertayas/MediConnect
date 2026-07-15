using System;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MediConnect.Mobile.Models;

namespace MediConnect.Mobile.Services
{
    public class VitalsService
    {
        private readonly HttpClient _httpClient;

        public VitalsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/vitals/patient/{patientId}
        public async Task<List<VitalsModel>> GetVitalsAsync(int patientId)
        {
            try
            {
                var vitals = await _httpClient.GetFromJsonAsync<List<VitalsModel>>($"api/vitals/patient/{patientId}");
                return vitals ?? new List<VitalsModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Get Error: {ex.Message}");
                return new List<VitalsModel>();
            }
        }

        // POST: api/vitals
        public async Task<bool> AddVitalsAsync(VitalsModel vitals)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/vitals", vitals);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // If the backend rejects the save, read the reason from the response
                var errorReason = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Create Rejected: {response.StatusCode} - {errorReason}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Post Error: {ex.Message}");
                return false;
            }
        }

        // PUT: api/vitals/{id}
        public async Task<bool> UpdateVitalsAsync(VitalsModel vitals)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/vitals/{vitals.VitalID}", vitals);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorReason = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Update Rejected: {response.StatusCode} - {errorReason}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Put Error: {ex.Message}");
                return false;
            }
        }

        // DELETE: api/vitals/{id}
        public async Task<bool> DeleteVitalsAsync(int vitalId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/vitals/{vitalId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Delete Error: {ex.Message}");
                return false;
            }
        }
    }
}