using System.ComponentModel.DataAnnotations;

namespace Api.Models;


public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty; 

    [MaxLength(50)]
    public string? Make { get; set; } 

    [MaxLength(50)]
    public string? Model { get; set; } 

    public int? Year { get; set; } 

    [MaxLength(20)]
    public string? LicensePlate { get; set; }

    [MaxLength(50)]
    public string? VIN { get; set; } /// Vehicle Identification Number

    public int SeatCapacity { get; set; } = 1; /// How many passengers it can hold

    public int WheelchairCapacity { get; set; } = 0; /// How many wheelchairs it can fit

    public double? Odometer { get; set; } /// Current mileage

    public bool IsActive { get; set; } = true; /// Soft delete (NFR-05)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
