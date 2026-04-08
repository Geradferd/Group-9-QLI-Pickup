using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

// Handles all Schedule / Hours of Operation business logic
// TODO: Full implementation pending ScheduleDTOs and HoursOfOperation model completion

public class ScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    // GET operating hours
    // Returns all active riders (soft-deleted riders are hidden by default)
    public async Task<List<OperatingHoursResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.OperatingHours.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        var operatingHours = await query
            .OrderBy(r => r.startTime)
            .ThenBy(r => r.endTime)
            .ToListAsync();

        return operatingHours.Select(r => MapToResponse(r)).ToList();
    }

    /// Update existing operating hours
    public async Task<OperatingHoursResponse?> UpdateOperatingHours(int Id, UpdateOperatingHoursRequest request)
    {
        var type = await _context.OperatingHours
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

    // GET special schedules
    // Returns all special schedules (soft-deleted riders are hidden by default)
    public async Task<List<SpecialScheduleResponse>> GetAllSpecialSchedulesAsync(bool includeInactive = false)
    {
        var query = _context.SpecialSchedules.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        var specialSchedules = await query
            .OrderBy(r => r.date)
            .ThenBy(r => r.specialStartTime)
            .ToListAsync();

        return specialSchedules.Select(r => MapToResponse(r)).ToList();
    }

    /// Update existing special dates
    public async Task<SpecialScheduleResponse?> UpdateSpecialSchedules(int Id, UpdateSpecialSchedulesRequest request)
    {
        var type = await _context.SpecialSchedules
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
    public async Task<bool> SoftDeleteSpecialScedule(int id)
    {
        var type = await _context.SpecialSchedules
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return false;

        type.closedFlag = true; /// Mark as closed
        await _context.SaveChangesAsync();

        return true;
    }

        // CREATE a new special schedule

    public async Task<SpecialScheduleResponse?> CreateSpecialScheduleAsync(CreateSpecialScheduleRequest request)
    {
        // Make sure the user exists before creating
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return null;

        var specialSchedule = new SpecialSchedule
        {
            UserId = request.UserId,
            date = request.date,
            specialStartTime = request.specialStartTime,
            closedFlag = request.closedFlag,
            description = request.description,
        };

        _context.SpecialSchedules.Add(specialSchedule);
        await _context.SaveChangesAsync();

        return MapToResponse(specialSchedule);
    }

    // Helper method to map entity to response DTO
    private OperatingHoursResponse MapToResponse(ScheduleType type)
    {
        return new OperatingHoursResponse
        {
            day = type.day,
            startTime = type.startTime,
            endTime = type.endTime,
            enabledFlag = type.enabledFlag,
        };
    }

    private SpecialScheduleResponse MapToResponse(ScheduleType type)
    {
        return new SpecialScheduleResponse
        {
            Id = type.Id,
            date = type.date,
            specialStartTime = type.specialStartTime,
            closedFlag = type.closedFlag,
            description = type.description
        };
    }
    // TODO: Implement UpdateOperatingHours once ScheduleResponse and UpdateScheduleRequest DTOs are finalized
    // TODO: Implement UpdateSpecialDates once ScheduleResponse and UpdateScheduleRequest DTOs are finalized
    // TODO: Implement SoftDelete for special dates
}