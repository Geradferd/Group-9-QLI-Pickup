using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

// GPS Track Point Table in DB
// High-volume append-only table for storing GPS location data from drivers
// Uses auto-increment integer PK instead of GUID for better insert performance
[Index(nameof(DriverId), nameof(DeviceTimestamp))]
public class GPS_Track_Point
{
    // Auto-increment integer primary key for better performance on high-volume inserts
    public int Id { get; set; }

    // Foreign key to Driver - required
    [Required]
    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!; // Navigation property

    // Foreign key to Trip - nullable (driver may be driving without an active trip)
    public int? TripId { get; set; }
    // Uncomment when Trip model is created:
    // public Trip? Trip { get; set; }

    // GPS Coordinates
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    // Speed in meters per second (nullable if not available from device)
    public double? Speed { get; set; }

    // Heading/bearing in degrees (0-360, where 0 is North) - nullable
    public double? Heading { get; set; }

    // GPS accuracy in meters (nullable if not provided by device)
    public double? Accuracy { get; set; }

    // Timestamp from the device when the GPS point was captured
    [Required]
    public DateTime DeviceTimestamp { get; set; }

    // Timestamp when the server received/stored this GPS point
    [Required]
    public DateTime ServerTimestamp { get; set; } = DateTime.UtcNow;
}   