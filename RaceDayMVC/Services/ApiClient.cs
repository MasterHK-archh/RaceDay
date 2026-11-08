using RaceDayMVC.Models;

namespace RaceDayMVC.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        public ApiClient(HttpClient http) => _http = http;

        public async Task<(bool ok, string? error, AuthResponse? data)> LoginAsync(LoginRequest req)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login", req);
            if (!resp.IsSuccessStatusCode)
                return (false, await resp.Content.ReadAsStringAsync(), null);

            var data = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            return (true, null, data);
        }

        public async Task<(bool ok, string? error, AuthResponse? data)> RegisterAsync(RegisterRequest req)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/register", req);
            if (!resp.IsSuccessStatusCode)
                return (false, await resp.Content.ReadAsStringAsync(), null);

            var data = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            return (true, null, data);
        }
    }
}