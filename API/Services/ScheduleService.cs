using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

// Handles all Schedule / Hours of Operation business logic
public class ScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    // ── Operating Hours ──────────────────────────────────────────

    public async Task<List<OperatingHoursResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.OperatingHours.AsQueryable();

        if (!includeInactive)
            query = query.Where(r => r.IsActive && !r.IsDeleted);

        var operatingHours = await query
            .OrderBy(r => r.day)
            .ThenBy(r => r.startTime)
            .ToListAsync();

        return operatingHours.Select(MapOperatingHoursToResponse).ToList();
    }

    public async Task<OperatingHoursResponse?> UpdateOperatingHoursAsync(int id, UpdateOperatingHoursRequest request)
    {
        var type = await _context.OperatingHours
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return null;

        type.day = request.day;
        type.startTime = request.startTime;
        type.endTime = request.endTime;
        type.enabledFlag = request.enabledFlag;
        type.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return MapOperatingHoursToResponse(type);
    }

    // ── Special Schedules ────────────────────────────────────────

    public async Task<List<SpecialScheduleResponse>> GetAllSpecialSchedulesAsync(bool includeInactive = false)
    {
        var query = _context.SpecialSchedules.AsQueryable();

        if (!includeInactive)
            query = query.Where(r => r.IsActive && !r.IsDeleted);

        var specialSchedules = await query
            .OrderBy(r => r.date)
            .ThenBy(r => r.specialStartTime)
            .ToListAsync();

        return specialSchedules.Select(MapSpecialScheduleToResponse).ToList();
    }

    public async Task<SpecialScheduleResponse?> GetSpecialScheduleByIdAsync(int id)
    {
        var type = await _context.SpecialSchedules
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        return type == null ? null : MapSpecialScheduleToResponse(type);
    }

    public async Task<SpecialScheduleResponse?> CreateAsync(CreateSpecialScheduleRequest request)
    {
        if (request.UserId.HasValue)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return null;
        }

        var specialSchedule = new SpecialDate
        {
            UserId = request.UserId,
            date = request.date,
            specialStartTime = request.specialStartTime,
            specialEndTime = request.specialEndTime,
            closedFlag = request.closedFlag,
            description = request.description,
        };

        _context.SpecialSchedules.Add(specialSchedule);
        await _context.SaveChangesAsync();
        return MapSpecialScheduleToResponse(specialSchedule);
    }

    public async Task<SpecialScheduleResponse?> UpdateSpecialSchedulesAsync(int id, UpdateSpecialDatesRequest request)
    {
        var type = await _context.SpecialSchedules
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return null;

        type.date = request.date;
        type.specialStartTime = request.specialStartTime;
        type.closedFlag = request.closedFlag;
        type.description = request.description;
        type.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return MapSpecialScheduleToResponse(type);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var type = await _context.SpecialSchedules
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return false;

        type.IsDeleted = true;
        type.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // ── Mappers ──────────────────────────────────────────────────

    private OperatingHoursResponse MapOperatingHoursToResponse(Hours_of_Operation h) => new()
    {
        Id = h.Id,
        day = h.day,
        startTime = h.startTime,
        endTime = h.endTime,
        enabledFlag = h.enabledFlag,
        IsActive = h.IsActive
    };

    private SpecialScheduleResponse MapSpecialScheduleToResponse(SpecialDate s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        date = s.date,
        specialStartTime = s.specialStartTime,
        specialEndTime = s.specialEndTime,
        closedFlag = s.closedFlag,
        description = s.description,
        IsActive = s.IsActive
    };
}