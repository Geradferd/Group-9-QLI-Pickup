using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.DTOs;

namespace Api.Services;

public class GPSService
{
    private readonly AppDbContext _context;

    public GPSService(AppDbContext context)
    {
        _context = context;
    }

    // Record a new GPS breadcrumb for a driver
    public async Task<GPSBreadcrumbResponse> CreateBreadcrumbAsync(int driverId, CreateGPSBreadcrumbRequest request)
    {
        // Verify driver exists
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == driverId);
        if (driver == null)
            throw new Exception("Driver not found");

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

    // Get all breadcrumbs (location history) for a specific trip
    public async Task<List<GPSBreadcrumbResponse>> GetTripBreadcrumbsAsync(int tripId)
    {
        // Verify trip exists
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
        if (trip == null)
            throw new Exception("Trip not found");

        var breadcrumbs = await _context.GPS_TrackPoints
            .Where(gps => gps.TripId == tripId)
            .OrderBy(gps => gps.DeviceTimestamp)
            .ToListAsync();

        return breadcrumbs.Select(MapToResponse).ToList();
    }

    // Get the latest GPS position for a specific driver
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

    // Get latest positions for all active drivers (those who have reported location in last X minutes)
    public async Task<List<DriverLatestPositionResponse>> GetActiveDriversPositionsAsync(int minutesThreshold = 30)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesThreshold);

        var activeDriverPositions = await _context.GPS_TrackPoints
            .Where(gps => gps.ServerTimestamp >= cutoffTime)
            .GroupBy(gps => gps.DriverId)
            .Select(g => g.OrderByDescending(gps => gps.ServerTimestamp).First())
            .ToListAsync();

        var result = new List<DriverLatestPositionResponse>();

        foreach (var position in activeDriverPositions)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == position.DriverId);
            if (driver == null) continue;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == driver.UserId);
            var driverName = user?.DisplayName ?? "Unknown Driver";

            result.Add(new DriverLatestPositionResponse
            {
                DriverId = position.DriverId,
                DriverName = driverName,
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Speed = position.Speed,
                Heading = position.Heading,
                LastUpdated = position.ServerTimestamp,
                ActiveTripId = position.TripId
            });
        }

        return result;
    }

    // Convert GPS_Track_Point to DTO
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
