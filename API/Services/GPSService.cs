using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.DTOs;

namespace Api.Services;

/// GPS Service - handles all GPS tracking logic
/// Original CRUD operations created by team
///
/// added by Angel:
/// - GPS deduplication: skips storing points within minimum distance of last point
/// - Trip validation: verifies trip is InProgress and driver is assigned before recording
/// - Haversine formula calculation for accurate GPS point comparison
/// - Stale position filtering for active drivers

public class GPSService
{
    private readonly AppDbContext _context;

    /// Minimum distance in meters between GPS points before storing a new one
    /// Prevents database bloat when driver is stationary or moving slowly
    private const double MinDistanceMeters = 10.0;

    public GPSService(AppDbContext context)
    {
        _context = context;
    }

    /// Record a new GPS breadcrumb for a driver
    /// Includes deduplication and trip validation
    public async Task<GPSBreadcrumbResponse?> CreateBreadcrumbAsync(int driverId, CreateGPSBreadcrumbRequest request)
    {
        /// Verify driver exists
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == driverId);
        if (driver == null)
            throw new Exception("Driver not found");

        /// If tripId is provided, validate the trip
        if (request.TripId.HasValue)
        {
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == request.TripId.Value);

            /// Trip must exist
            if (trip == null)
                throw new Exception("Trip not found");

            /// Trip must be InProgress to record GPS
            if (trip.Status != TripStatus.InProgress)
                throw new Exception("GPS tracking is only active for trips that are in progress");

            /// Driver must be assigned to this trip
            if (trip.DriverId != driver.Id)
                throw new Exception("Driver is not assigned to this trip");
        }

        /// GPS Deduplication - check distance from last recorded point
        var lastPoint = await _context.GPS_TrackPoints
            .Where(gps => gps.DriverId == driver.Id)
            .OrderByDescending(gps => gps.ServerTimestamp)
            .FirstOrDefaultAsync();

        if (lastPoint != null)
        {
            var distance = CalculateDistanceMeters(
                lastPoint.Latitude, lastPoint.Longitude,
                request.Latitude, request.Longitude
            );

            /// Skip storing if within minimum distance threshold
            if (distance < MinDistanceMeters)
            {
                return null; /// Point too close to last one, skipped
            }
        }

        var breadcrumb = new GPS_Track_Point
        {
            DriverId = driver.Id,
            TripId = request.TripId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Speed = request.Speed,
            Heading = request.Heading,
            Accuracy = request.Accuracy,
            DeviceTimestamp = request.DeviceTimestamp,
            ServerTimestamp = DateTime.UtcNow
        };

        _context.GPS_TrackPoints.Add(breadcrumb);
        await _context.SaveChangesAsync();

        return MapToResponse(breadcrumb);
    }

    /// Get all breadcrumbs (location history) for a specific trip
    public async Task<List<GPSBreadcrumbResponse>> GetTripBreadcrumbsAsync(int tripId)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
        if (trip == null)
            throw new Exception("Trip not found");

        var breadcrumbs = await _context.GPS_TrackPoints
            .Where(gps => gps.TripId == tripId)
            .OrderBy(gps => gps.DeviceTimestamp)
            .AsNoTracking()
            .ToListAsync();

        return breadcrumbs.Select(MapToResponse).ToList();
    }

    /// Get the latest GPS position for a specific driver
    public async Task<DriverLatestPositionResponse?> GetDriverLatestPositionAsync(int driverId)
    {
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
        if (driver == null)
            return null;

        var latestPosition = await _context.GPS_TrackPoints
            .Where(gps => gps.DriverId == driverId)
            .OrderByDescending(gps => gps.ServerTimestamp)
            .FirstOrDefaultAsync();

        if (latestPosition == null)
            return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == driver.UserId);
        var driverName = user?.DisplayName ?? "Unknown Driver";

        return new DriverLatestPositionResponse
        {
            DriverId = driverId,
            DriverName = driverName,
            Latitude = latestPosition.Latitude,
            Longitude = latestPosition.Longitude,
            Speed = latestPosition.Speed,
            Heading = latestPosition.Heading,
            LastUpdated = latestPosition.ServerTimestamp,
            ActiveTripId = latestPosition.TripId
        };
    }

    /// Get latest positions for all active drivers
    /// Only includes drivers with an active (InProgress) trip
    public async Task<List<DriverLatestPositionResponse>> GetActiveDriversPositionsAsync(int minutesThreshold = 30)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesThreshold);

        /// Get all active trip driver IDs to filter only truly active drivers
        var activeDriverIds = await _context.Trips
            .Where(t => t.Status == TripStatus.InProgress && t.DriverId.HasValue)
            .Select(t => t.DriverId!.Value)
            .Distinct()
            .ToListAsync();

        var result = new List<DriverLatestPositionResponse>();

        foreach (var activeDriverId in activeDriverIds)
        {
            var latestPoint = await _context.GPS_TrackPoints
                .Where(gps => gps.DriverId == activeDriverId && gps.ServerTimestamp >= cutoffTime)
                .OrderByDescending(gps => gps.ServerTimestamp)
                .FirstOrDefaultAsync();

            if (latestPoint == null) continue;

            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == activeDriverId);
            if (driver == null) continue;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == driver.UserId);
            var driverName = user?.DisplayName ?? "Unknown Driver";

            result.Add(new DriverLatestPositionResponse
            {
                DriverId = activeDriverId,
                DriverName = driverName,
                Latitude = latestPoint.Latitude,
                Longitude = latestPoint.Longitude,
                Speed = latestPoint.Speed,
                Heading = latestPoint.Heading,
                LastUpdated = latestPoint.ServerTimestamp,
                ActiveTripId = latestPoint.TripId
            });
        }

        return result;
    }

    /// Haversine formula - calculates the distance in meters between two GPS coordinates
    /// Used for deduplication to skip storing points that are too close together
    private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusMeters = 6371000;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    /// Convert GPS_Track_Point to DTO
    private GPSBreadcrumbResponse MapToResponse(GPS_Track_Point breadcrumb)
    {
        return new GPSBreadcrumbResponse
        {
            Id = breadcrumb.Id,
            DriverId = breadcrumb.DriverId,
            TripId = breadcrumb.TripId,
            Latitude = breadcrumb.Latitude,
            Longitude = breadcrumb.Longitude,
            Speed = breadcrumb.Speed,
            Heading = breadcrumb.Heading,
            Accuracy = breadcrumb.Accuracy,
            DeviceTimestamp = breadcrumb.DeviceTimestamp,
            ServerTimestamp = breadcrumb.ServerTimestamp
        };
    }
}
