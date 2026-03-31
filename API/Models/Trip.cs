using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

// Trip Table in DB
public class Trip
{
    public int Id { get; set; }

    // Relationships

    // Foreign key to Rider
    public int? RiderId { get; set; }
    public Rider? Rider { get; set; }

    // Foreign key to Driver 
    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }

    // Foreign key to Vehicle
    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    // Foreign key to TransportationType
    [Required]
    public int TransportationTypeId { get; set; }
    public TransportationType TransportationType { get; set; } = null!;

    // User who submitted the request
    [Required]
    public int RequestedByUserId { get; set; }
    public User RequestedByUser { get; set; } = null!;

    // User who approved the request
    public int? ApprovedByUserId { get; set; }
    public User? ApprovedByUser { get; set; }

    // Status
    // Current status of the trip
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
    // Requested pickup time submitted by the rider/admin
    [Required]
    public DateTime ScheduledPickupTime { get; set; }

    // Actual time the driver started the trip 
    public DateTime? ActualPickupTime { get; set; }

    // Actual time the driver completed the trip 
    public DateTime? ActualDropoffTime { get; set; }

    // Passengers & Accessibility
    [Required]
    [Range(1, 20)]
    public int PassengerCount { get; set; } = 1;

    // Whether the trip requires wheelchair-accessible vehicle/space
    public bool RequiresWheelchair { get; set; } = false;
    
    // Distance

    public double? DistanceMiles { get; set; }

    // Notes and Denial Reason

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Required when an admin denies the trip
    [MaxLength(1000)]
    public string? DenialReason { get; set; }

    // Soft Delete

    public bool IsDeleted { get; set; } = false;


    // Every status change for this trip
    // REMOVE COMMENT LINES BELOW WHEN SOMEONE FINISHES TRIP STATUS HISTORY
    // public ICollection<TripStatusHistory> StatusHistory { get; set; } = new List<TripStatusHistory>();

    // GPS Track Point information
    public ICollection<GPS_Track_Point> TrackPoints { get; set; } = new List<GPS_Track_Point>();

    // Notifications for this trip
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

// Valid status values for Trip.Status.

public static class TripStatus
{
    public const string Pending    = "Pending";
    public const string Approved   = "Approved";
    public const string Denied     = "Denied";
    public const string Scheduled  = "Scheduled";
    public const string InProgress = "InProgress";
    public const string Completed  = "Completed";
    public const string Cancelled  = "Cancelled";
    public const string NoShow     = "NoShow";

    // Returns true if the given transition is valid
    public static bool IsValidTransition(string from, string to) => (from, to) switch
    {
        (Pending,    Approved)   => true,
        (Pending,    Denied)     => true,
        (Pending,    Cancelled)  => true,
        (Approved,   Scheduled)  => true,
        (Approved,   Cancelled)  => true,
        (Scheduled,  InProgress) => true,
        (Scheduled,  Cancelled)  => true,
        (InProgress, Completed)  => true,
        (InProgress, NoShow)     => true,
        _ => false
    };
}