using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class CreateVehicleRequest
{
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
    public string? VIN { get; set; }

    public int SeatCapacity { get; set; } = 1;

    public int WheelchairCapacity { get; set; } = 0;

    public double? Odometer { get; set; }
}

public class UpdateVehicleRequest
{
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
    public string? VIN { get; set; }

    public int SeatCapacity { get; set; } = 1;

    public int WheelchairCapacity { get; set; } = 0;

    public double? Odometer { get; set; }
}

public class VehicleResponse
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? LicensePlate { get; set; }
    public string? VIN { get; set; }
    public int SeatCapacity { get; set; }
    public int WheelchairCapacity { get; set; }
    public double? Odometer { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}