using RaceDayMVC.Models;

namespace RaceDayMVC.Services
{
    public class CurrentUser
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUser(IHttpContextAccessor http) => _http = http;

        private ISession Session => _http.HttpContext!.Session;

        public bool IsAuthenticated => !string.IsNullOrEmpty(Session.GetString("JwtToken"));
        public int UserId => int.TryParse(Session.GetString("UserId"), out var id) ? id : 0;
        public string Email => Session.GetString("Email") ?? "";
        public string FirstName => Session.GetString("FirstName") ?? "";
        public string LastName => Session.GetString("LastName") ?? "";
        public string RoleName => Session.GetString("RoleName") ?? "";
        public bool IsOrganiser => RoleName == "Organiser";
        public bool IsParticipant => RoleName == "Participant";

        public void SignIn(AuthResponse auth)
        {
            Session.SetString("JwtToken", auth.Token);
            Session.SetString("UserId", auth.UserId.ToString());
            Session.SetString("Email", auth.Email);
            Session.SetString("FirstName", auth.FirstName);
            Session.SetString("LastName", auth.LastName);
            Session.SetString("RoleName", auth.RoleName);
        }

        public void SignOut() => Session.Clear();
    }
}
