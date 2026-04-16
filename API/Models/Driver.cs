using System.ComponentModel.DataAnnotations;

namespace Api.Models;


public class Driver
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } /// Foreign key - links to the User table
    public User User { get; set; } = null!; /// Navigation property

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; } /// Driver's license number

    public DateTime? LicenseExpiry { get; set; } /// When their license expires (nullable)

    [MaxLength(500)]
    public string? DeviceToken { get; set; } /// For push notifications later

    public bool IsActive { get; set; } = true; /// Soft delete 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
