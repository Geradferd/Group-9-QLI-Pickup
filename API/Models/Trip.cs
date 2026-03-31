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
    [MaxLength(50)]
    public string Status { get; set; } = TripStatus.Pending;

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

    // Navigation Collections
    public ICollection<TripStatusHistory> StatusHistory { get; set; } = new List<TripStatusHistory>();
    public ICollection<GpsTrackPoint> TrackPoints { get; set; } = new List<GpsTrackPoint>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
