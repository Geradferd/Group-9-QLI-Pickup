using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class CreateTripRequest
{
    public int? RiderId { get; set; }

    [Required]
    public int TransportationTypeId { get; set; }

    [Required]
    [MaxLength(500)]
    public string PickupAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string DestinationAddress { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduledPickupTime { get; set; }

    [Required]
    [Range(1, 20)]
    public int PassengerCount { get; set; } = 1;

    public bool RequiresWheelchair { get; set; } = false;

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class UpdateTripRequest
{
    public int? RiderId { get; set; }

    [Required]
    public int TransportationTypeId { get; set; }

    [Required]
    [MaxLength(500)]
    public string PickupAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string DestinationAddress { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduledPickupTime { get; set; }

    [Required]
    [Range(1, 20)]
    public int PassengerCount { get; set; } = 1;

    public bool RequiresWheelchair { get; set; } = false;

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class AssignTripRequest
{
    [Required]
    public int DriverId { get; set; }

    [Required]
    public int VehicleId { get; set; }
}

public class DenyTripRequest
{
    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
}

public class CancelTripRequest
{
    [MaxLength(1000)]
    public string? Reason { get; set; }
}

public class CompleteTripRequest
{
    public double? DistanceMiles { get; set; }
}

public class TripResponse
{
    public int Id { get; set; }
    public int? RiderId { get; set; }
    public int? DriverId { get; set; }
    public int? VehicleId { get; set; }
    public int TransportationTypeId { get; set; }
    public int RequestedByUserId { get; set; }
    public int? ApprovedByUserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public DateTime ScheduledPickupTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    public DateTime? ActualDropoffTime { get; set; }
    public int PassengerCount { get; set; }
    public bool RequiresWheelchair { get; set; }
    public double? DistanceMiles { get; set; }
    public string? Notes { get; set; }
    public string? DenialReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}