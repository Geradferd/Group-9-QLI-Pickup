using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

/// Request to record a GPS location (breadcrumb)
public class CreateGPSBreadcrumbRequest
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    public double? Speed { get; set; }

    public double? Heading { get; set; }

    public double? Accuracy { get; set; }

    [Required]
    public DateTime DeviceTimestamp { get; set; }

    /// Optional: if driver is actively on a trip
    public int? TripId { get; set; }
}

/// Response for GPS location
public class GPSBreadcrumbResponse
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int? TripId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
    public double? Heading { get; set; }
    public double? Accuracy { get; set; }
    public DateTime DeviceTimestamp { get; set; }
    public DateTime ServerTimestamp { get; set; }
}

/// Response for driver's latest position
public class DriverLatestPositionResponse
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
    public double? Heading { get; set; }
    public DateTime LastUpdated { get; set; }
    public int? ActiveTripId { get; set; }
}

/// Response for all active drivers' positions
public class ActiveDriversPositionsResponse
{
    public List<DriverLatestPositionResponse> ActiveDrivers { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// Real-time location update for SignalR
public class RealtimeLocationUpdate
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
    public double? Heading { get; set; }
    public int? TripId { get; set; }
    public DateTime Timestamp { get; set; }
}
