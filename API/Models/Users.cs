using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// defines three roles for our app
public enum UserRole
{
    Admin, /// can aprove and deny trips, assign drivers , manage everything
    Driver, /// can view assigned trips, start/complete/cancel trips
    Rider /// Can request trips, view trips on calendar
}

//Users table in database
public class User
{
    /// primary key
    public int Id { get; set; } /// self.id: int = 0
    [Required]
    [MaxLength(100)]
    public string Email {get; set;} = string.Empty; /// self.email: str = ""

    [Required] /// never store real password, hashes password
    public string PasswordHash { get; set;} = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DisplayName {get; set;} = string.Empty;

    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true; /// for disabling accounts

    public DateTime CreatedAt { get; set;} = DateTime.UtcNow;  /// date for account creation
}
