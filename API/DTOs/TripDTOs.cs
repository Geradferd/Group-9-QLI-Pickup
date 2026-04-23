using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.DTOs;

/// Trip Data Transfer Objects
/// Original CRUD DTOs (CreateTripRequest, UpdateTripRequest, TripQueryParams, TripResponse)
/// were created by Gavin 
///
/// Added by Angel:
/// - AssignTripRequest: for admins to assign a driver and vehicle to a trip 
/// - DenyTripRequest: for admins to deny a trip with a required reason 
/// - CancelTripRequest: for cancelling a trip with an optional reason
/// - CompleteTripRequest: for drivers to complete a trip with distance data 
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

// What the client sends when updating a trip
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

/// Query parameters for filtering trips (FR-06)
public class TripQueryParams
{
    public TripStatus? Status { get; set; }
    public int? RiderId { get; set; }
    public int? DriverId { get; set; }
    public int? TransportationTypeId { get; set; }
    public DateTime? Date { get; set; }
}

/// What the server sends back
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

/// Admin assigns a driver and vehicle to an approved trip (FR-03)
public class AssignTripRequest
{
    [Required]
    public int DriverId { get; set; }

    [Required]
    public int VehicleId { get; set; }
}

/// Admin denies a trip with a reason (FR-02)
public class DenyTripRequest
{
    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
}

/// Optional reason for cancelling
public class CancelTripRequest
{
    [MaxLength(1000)]
    public string? Reason { get; set; }
}

// Driver completes a trip with actual distance (FR-04)
public class CompleteTripRequest
{
    public double? DistanceMiles { get; set; }
}
