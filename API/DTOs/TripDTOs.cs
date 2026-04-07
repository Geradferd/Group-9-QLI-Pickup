using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.DTOs;

public class CreateTripRequest
{
    public int? RiderId { get; set; }

    [Required(ErrorMessage = "Transportation type is required")]
    public int TransportationTypeId { get; set; }

    [Required(ErrorMessage = "Pickup address is required")]
    [MaxLength(500)]
    public string PickupAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination address is required")]
    [MaxLength(500)]
    public string DestinationAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Scheduled pickup time is required")]
    public DateTime ScheduledPickupTime { get; set; }

    [Range(1, 20, ErrorMessage = "Passenger count must be between 1 and 20")]
    public int PassengerCount { get; set; } = 1;

    public bool RequiresWheelchair { get; set; } = false;

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class UpdateTripRequest
{
    public int? RiderId { get; set; }
    public int? TransportationTypeId { get; set; }

    [MaxLength(500)]
    public string? PickupAddress { get; set; }

    [MaxLength(500)]
    public string? DestinationAddress { get; set; }

    public DateTime? ScheduledPickupTime { get; set; }

    [Range(1, 20, ErrorMessage = "Passenger count must be between 1 and 20")]
    public int? PassengerCount { get; set; }

    public bool? RequiresWheelchair { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class TripQueryParams
{
    public TripStatus? Status { get; set; }
    public int? RiderId { get; set; }
    public int? DriverId { get; set; }
    public int? TransportationTypeId { get; set; }
    public DateTime? Date { get; set; }
}

public class TripResponse
{
    public int Id { get; set; }
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
}
