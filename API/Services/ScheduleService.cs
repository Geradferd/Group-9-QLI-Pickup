using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;
using System.Data.Common;
using System.ComponentModel;

namespace Api.Services;

// Handles all Transportation Type business logic
// Communicates with DB to manage transportation types

public class ScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    /// Get or Update existing operating hours
    public async Task<ScheduleResponse?> UpdateOperatingHours(int Id, UpdateScheduleRequest request)
    {
        var type = await _context.ScheduleTypes
            .FirstOrDefaultAsync(t => t.Id == Id && !t.IsDeleted);

        if (type == null)
            return null;

            day = type.day;
            startTime = type.startTime;
            endTime = type.endTime;
            enabledFlag = type.enabledFlag;

        await _context.SaveChangesAsync();

        return MapToResponse(type);
    }

        /// Update existing special dates
    public async Task<ScheduleResponse?> UpdateSpecialDates(int Id, UpdateScheduleRequest request)
    {
        var type = await _context.ScheduleTypes
            .FirstOrDefaultAsync(t => t.Id == Id && !t.IsDeleted);

        if (type == null)
            return null;

            type.Id = request.Id;
            type.date = request.date;
            type.specialStartTime = request.specialStartTime;
            type.closedFlag = request.closedFlag;
            type.description = request.description;

        await _context.SaveChangesAsync();

        return MapToResponse(type);
    }

        // Soft delete a special date type
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var type = await _context.TransportationTypes
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return false;

        // Check if any active trips are using this type
        var hasActiveTrips = await _context.Trips
            .AnyAsync(t => t.TransportationTypeId == id && !t.Status.Equals("Cancelled"));

        if (hasActiveTrips)
        {
            // Don't delete if there are active trips using this type
            return false;
        }

        type.closedFlag = true; /// Mark as closed

        await _context.SaveChangesAsync();

        return true;
    }

    // Helper method to map entity to response DTO
    private ScheduleResponse MapToResponse(ScheduleType type)
    {
        return new ScheduleResponse
        {
            day = type.day,
            startTime = type.startTime,
            endTime = type.endTime,
            enabledFlag = type.enabledFlag,
            Id = type.Id,
            date = type.date,
            specialStartTime = type.specialStartTime,
            closedFlag = type.closedFlag,
            description = type.description
        };
    }
}