using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

// This is the API controller that handles registration and login requests
// [Route("api/[controller]")] means all endpoints start with /api/auth
// So register is POST /api/auth/register and login is POST /api/auth/login
[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

        /// ScheduleService gets auto injected
    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // GET operating hours
    // Any authenticated user can view operating hours
    // Only admins can see inactive operating hours
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var operatingHours = await _scheduleService.GetAllAsync(includeInactive);
        return Ok(operatingHours);
    }

    // UPDATE operating hours
    // Only admins can update operating hours
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOperatingHours(int id, [FromBody] UpdateOperatingHoursRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _scheduleService.UpdateOperatingHoursAsync(id, request);

        if (schedule == null)
            return NotFound(new { message = "Operating hours not found" });

        return Ok(schedule);
    }

    // GET special schedules
    // Any authenticated user can view special schedules
    // Only admins can see inactive special schedules
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var specialSchedules = await _scheduleService.GetAllAsync(includeInactive);
        return Ok(specialSchedules);
    }

    // UPDATE Special Dates
    // Only admins can update special dates
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSpecialDates(int id, [FromBody] UpdateSpecialDatesRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _scheduleService.UpdateSpecialDatesAsync(id, request);

        if (schedule == null)
            return NotFound(new { message = "Special dates not found" });

        return Ok(schedule);
    }

    // SOFT DELETE A special schedule
    // Only admins can delete special schedules
    // Note: if there is a problem with the code check the variable name for id for special schedule data types
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _scheduleService.SoftDeleteAsync(id);

        if (!success)
            return NotFound(new { message = "Special schedule not found" });

        return Ok(new { message = "Special schedule deleted successfully" });
    }

    // CREATE A special schedule
    // Only admins can create special schedules
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSpecialScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var specialSchedule = await _scheduleService.CreateAsync(request);

        if (specialSchedule == null)
            return BadRequest(new { message = "Failed to create special schedule" });

        return CreatedAtAction(nameof(GetById), new { id = specialSchedule.Id }, specialSchedule);
    }
}