using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MediConnect.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly SessionService _session;

#if ANDROID
        private const string BaseUrl = "http://10.177.52.175:5016/";
#else
        private const string BaseUrl = "http://localhost:5016/";
#endif

        public ApiService(SessionService session)
        {
            _session = session;
            _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        private void AttachAuthHeader()
        {
            if (!string.IsNullOrEmpty(_session.Token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _session.Token);
            }
            else
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AttachAuthHeader();
            var response = await _http.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body)
        {
            AttachAuthHeader();
            var response = await _http.PostAsJsonAsync(endpoint, body);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest body)
        {
            AttachAuthHeader();
            var response = await _http.PutAsJsonAsync(endpoint, body);
            return response.IsSuccessStatusCode;
        }

        // Overload for PUT that returns a response body
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body)
        {
            AttachAuthHeader();
            var response = await _http.PutAsJsonAsync(endpoint, body);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            AttachAuthHeader();
            var response = await _http.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}