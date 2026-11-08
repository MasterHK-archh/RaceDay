using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";
    [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = "";
    [Required]
    public string FirstName { get; set; } = "";
    [Required]
    public string LastName { get; set; } = "";
    [Required]
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