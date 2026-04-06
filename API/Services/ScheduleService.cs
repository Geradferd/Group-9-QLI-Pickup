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
    public async Task<ScheduleResponse?> UpdateAsync(int Id, UpdateScheduleRequest request)
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