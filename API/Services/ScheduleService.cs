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

    // TODO: Implement UpdateOperatingHours once ScheduleResponse and UpdateScheduleRequest DTOs are finalized
    // TODO: Implement UpdateSpecialDates once ScheduleResponse and UpdateScheduleRequest DTOs are finalized
    // TODO: Implement SoftDelete for special dates
}