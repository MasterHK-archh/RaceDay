using System.Net.Http.Headers;

namespace RaceDayMVC.Services
{
    public class SessionTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _http;
        public SessionTokenHandler(IHttpContextAccessor http) => _http = http;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var token = _http.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, ct);
        }
    }
}
