using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class CreateDriverRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    public DateTime? LicenseExpiry { get; set; }
}

// What the client sends when updating a driver
public class UpdateDriverRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    public DateTime? LicenseExpiry { get; set; }
}

public class DriverResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}