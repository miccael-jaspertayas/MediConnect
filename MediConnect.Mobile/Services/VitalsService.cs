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
        private const string BaseUrl = "api/vitals";

        public VitalsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Fetch patient vitals history
        public async Task<List<VitalsModel>> GetVitalsByPatientIdAsync(int patientId)
        {
            return await _httpClient.GetFromJsonAsync<List<VitalsModel>>($"{BaseUrl}/patient/{patientId}")
                   ?? new List<VitalsModel>();
        }

        // POST: Add new vital entry from the app
        public async Task<bool> AddVitalsAsync(VitalsModel vitals)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, vitals);
            return response.IsSuccessStatusCode;
        }
    }
}