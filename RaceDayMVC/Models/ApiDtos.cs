namespace RaceDayMVC.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string RoleName { get; set; } = "";
        public string? ProfilePictureUrl { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string RoleName { get; set; } = "";
    }
}
