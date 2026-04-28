using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;


/// define what data gets sent to and from the API
/// We use these instead of the User model 
/// dont expose sensitive data like the password hash

/// registering a new account
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;
}

/// client sends when logging in
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// sends back after a successful login
/// no sensitive data only what the frontend needs
public class AuthResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
