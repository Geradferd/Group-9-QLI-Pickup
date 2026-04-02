using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

public class Trip
{
    public int Id { get; set; }

    // Relationships
    public int? RiderId { get; set; }
    public Rider? Rider { get; set; }

    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }

    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Required]
    public int TransportationTypeId { get; set; }
    public TransportationType TransportationType { get; set; } = null!;

    [Required]
    public int RequestedByUserId { get; set; }
    public User RequestedByUser { get; set; } = null!;

    public int? ApprovedByUserId { get; set; }
    public User? ApprovedByUser { get; set; }

    // Status
    [Required]
    public TripStatus Status { get; set; } = TripStatus.Pending;

    // Pickup & Destination
    [Required]
    [MaxLength(500)]
    public string PickupAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string DestinationAddress { get; set; } = string.Empty;

    // Scheduling
    [Required]
    public DateTime ScheduledPickupTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    public DateTime? ActualDropoffTime { get; set; }

    // Passengers & Accessibility
    [Required]
    [Range(1, 20)]
    public int PassengerCount { get; set; } = 1;
    public bool RequiresWheelchair { get; set; } = false;

    // Distance
    public double? DistanceMiles { get; set; }

    // Notes
    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(1000)]
    public string? DenialReason { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; } = false;

    // Date the trip was created and updated.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Collections
    public ICollection<TripStatusHistory> StatusHistory { get; set; } = new List<TripStatusHistory>();
    public ICollection<GPS_Track_Point> TrackPoints { get; set; } = new List<GPS_Track_Point>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
