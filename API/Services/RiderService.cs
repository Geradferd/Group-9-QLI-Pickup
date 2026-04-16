using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;


public class RiderService
{
    private readonly AppDbContext _context;

    public RiderService(AppDbContext context)
    {
        _context = context;
    }

    /// GET ALL RIDERS
    /// Returns all active riders (soft-deleted riders are hidden by default)
    public async Task<List<RiderResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Riders.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        var riders = await query
            .OrderBy(r => r.LastName)
            .ThenBy(r => r.FirstName)
            .ToListAsync();

        return riders.Select(r => MapToResponse(r)).ToList();
    }

    /// RIDER ID
    public async Task<RiderResponse?> GetByIdAsync(int id)
    {
        var rider = await _context.Riders
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (rider == null)
            return null;

        return MapToResponse(rider);
    }

 
    /// CREATE A NEW RIDER

    public async Task<RiderResponse?> CreateAsync(CreateRiderRequest request)
    {
        /// Make sure the user exists before creating
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return null;

        var rider = new Rider
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            RoomNumber = request.RoomNumber,
            MobilityNotes = request.MobilityNotes,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Riders.Add(rider);
        await _context.SaveChangesAsync();

        return MapToResponse(rider);
    }


    /// UPDATE AN EXISTING RIDER

    public async Task<RiderResponse?> UpdateAsync(int id, UpdateRiderRequest request)
    {
        var rider = await _context.Riders
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (rider == null)
            return null;

        /// updates only the fields that are allowed to change
        rider.FirstName = request.FirstName;
        rider.LastName = request.LastName;
        rider.Phone = request.Phone;
        rider.RoomNumber = request.RoomNumber;
        rider.MobilityNotes = request.MobilityNotes;
        rider.EmergencyContactName = request.EmergencyContactName;
        rider.EmergencyContactPhone = request.EmergencyContactPhone;

        await _context.SaveChangesAsync();

        return MapToResponse(rider);
    }


    /// SOFT DELETE A RIDER
    /// Sets IsActive to false instead of actually deleting 
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var rider = await _context.Riders
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (rider == null)
            return false;

        rider.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    /// HELPER - MAP RIDER TO RESPONSE DTO
    private RiderResponse MapToResponse(Rider rider)
    {
        return new RiderResponse
        {
            Id = rider.Id,
            UserId = rider.UserId,
            FirstName = rider.FirstName,
            LastName = rider.LastName,
            Phone = rider.Phone,
            RoomNumber = rider.RoomNumber,
            MobilityNotes = rider.MobilityNotes,
            EmergencyContactName = rider.EmergencyContactName,
            EmergencyContactPhone = rider.EmergencyContactPhone,
            IsActive = rider.IsActive,
            CreatedAt = rider.CreatedAt,
        };
    }
}
