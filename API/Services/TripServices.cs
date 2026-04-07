using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;
namespace Api.Services;
public class TripService
{
    private readonly AppDbContext _context;
    public TripService(AppDbContext context)
    {
        _context = context;
    }
    private static readonly TripStatus[] LockedStatuses =
    [
        TripStatus.InProgress,
        TripStatus.Completed,
        TripStatus.Cancelled,
        TripStatus.Denied,
        TripStatus.NoShow
    ];
    public async Task<List<TripResponse>> GetAllAsync(TripQueryParams query)
    {
        var q = _context.Trips
            .Where(t => !t.IsDeleted)
            .AsQueryable();
        if (query.Status.HasValue)
            q = q.Where(t => t.Status == query.Status.Value);
        if (query.RiderId.HasValue)
            q = q.Where(t => t.RiderId == query.RiderId.Value);
        if (query.DriverId.HasValue)
            q = q.Where(t => t.DriverId == query.DriverId.Value);
        if (query.TransportationTypeId.HasValue)
            q = q.Where(t => t.TransportationTypeId == query.TransportationTypeId.Value);
        if (query.Date.HasValue)
            q = q.Where(t => t.ScheduledPickupTime.Date == query.Date.Value.Date);
        var trips = await q
            .OrderBy(t => t.ScheduledPickupTime)
            .ToListAsync();
        return trips.Select(MapToResponse).ToList();
    }
    public async Task<TripResponse?> GetByIdAsync(int id)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (trip == null)
            return null;
        return MapToResponse(trip);
    }
    public async Task<TripResponse> CreateAsync(CreateTripRequest request, int requestedByUserId)
    {
        var trip = new Trip
        {
            RiderId = request.RiderId,
            TransportationTypeId = request.TransportationTypeId,
            Status = TripStatus.Pending,
            PickupAddress = request.PickupAddress,
            DestinationAddress = request.DestinationAddress,
            ScheduledPickupTime = request.ScheduledPickupTime,
            PassengerCount = request.PassengerCount,
            RequiresWheelchair = request.RequiresWheelchair,
            Notes = request.Notes,
            RequestedByUserId = requestedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();
        return MapToResponse(trip);
    }
    public async Task<TripResponse?> UpdateAsync(int id, UpdateTripRequest request)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (trip == null)
            return null;
        if (LockedStatuses.Contains(trip.Status))
            return null;
        if (request.RiderId.HasValue)
            trip.RiderId = request.RiderId;
        if (request.TransportationTypeId.HasValue)
            trip.TransportationTypeId = request.TransportationTypeId.Value;
        if (!string.IsNullOrEmpty(request.PickupAddress))
            trip.PickupAddress = request.PickupAddress;
        if (!string.IsNullOrEmpty(request.DestinationAddress))
            trip.DestinationAddress = request.DestinationAddress;
        if (request.ScheduledPickupTime.HasValue)
            trip.ScheduledPickupTime = request.ScheduledPickupTime.Value;
        if (request.PassengerCount.HasValue)
            trip.PassengerCount = request.PassengerCount.Value;
        if (request.RequiresWheelchair.HasValue)
            trip.RequiresWheelchair = request.RequiresWheelchair.Value;
        if (request.Notes != null)
            trip.Notes = request.Notes;
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToResponse(trip);
    }
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (trip == null)
            return false;
        trip.IsDeleted = true;
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    private TripResponse MapToResponse(Trip trip)
    {
        return new TripResponse
        {
            Id = trip.Id,
            Status = trip.Status.ToString(),
            PickupAddress = trip.PickupAddress,
            DestinationAddress = trip.DestinationAddress,
            ScheduledPickupTime = trip.ScheduledPickupTime,
            ActualPickupTime = trip.ActualPickupTime,
            ActualDropoffTime = trip.ActualDropoffTime,
            PassengerCount = trip.PassengerCount,
            RequiresWheelchair = trip.RequiresWheelchair,
            DistanceMiles = trip.DistanceMiles,
            Notes = trip.Notes
        };
    }
}
