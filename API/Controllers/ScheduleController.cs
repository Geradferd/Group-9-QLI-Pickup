using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;

// Schedule Controller
// GET    /api/schedule/operating-hours                  - List operating hours
// PUT    /api/schedule/operating-hours/{id}             - Update operating hours (Admin)
// GET    /api/schedule/special-schedules                - List special schedules
// GET    /api/schedule/special-schedules/{id}           - Get special schedule by ID
// POST   /api/schedule/special-schedules                - Create special schedule (Admin)
// PUT    /api/schedule/special-schedules/{id}           - Update special schedule (Admin)
// DELETE /api/schedule/special-schedules/{id}           - Soft delete special schedule (Admin)

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // ── Operating Hours ──────────────────────────────────────────

    // GET /api/schedule/operating-hours
    [HttpGet("operating-hours")]
    public async Task<IActionResult> GetOperatingHours([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
            return Forbid();

        var operatingHours = await _scheduleService.GetAllAsync(includeInactive);
        return Ok(operatingHours);
    }

    // PUT /api/schedule/operating-hours/{id}
    [HttpPut("operating-hours/{id}")]
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

    // ── Special Schedules ────────────────────────────────────────

    // GET /api/schedule/special-schedules
    [HttpGet("special-schedules")]
    public async Task<IActionResult> GetSpecialSchedules([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
            return Forbid();

        var specialSchedules = await _scheduleService.GetAllSpecialSchedulesAsync(includeInactive);
        return Ok(specialSchedules);
    }

    // GET /api/schedule/special-schedules/{id}
    [HttpGet("special-schedules/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var schedule = await _scheduleService.GetSpecialScheduleByIdAsync(id);

        if (schedule == null)
            return NotFound(new { message = "Special schedule not found" });

        return Ok(schedule);
    }

    // POST /api/schedule/special-schedules
    [HttpPost("special-schedules")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSpecialScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var specialSchedule = await _scheduleService.CreateAsync(request);

        if (specialSchedule == null)
            return BadRequest(new { message = "Failed to create special schedule - user not found" });

        return CreatedAtAction(nameof(GetById), new { id = specialSchedule.Id }, specialSchedule);
    }

    // PUT /api/schedule/special-schedules/{id}
    [HttpPut("special-schedules/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSpecialDates(int id, [FromBody] UpdateSpecialDatesRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _scheduleService.UpdateSpecialSchedulesAsync(id, request);

        if (schedule == null)
            return NotFound(new { message = "Special schedule not found" });

        return Ok(schedule);
    }

    // DELETE /api/schedule/special-schedules/{id}
    [HttpDelete("special-schedules/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _scheduleService.SoftDeleteAsync(id);

        if (!success)
            return NotFound(new { message = "Special schedule not found" });

        return Ok(new { message = "Special schedule deleted successfully" });
    }
}