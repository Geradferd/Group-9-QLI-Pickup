using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly DriverService _driverService;

    public DriverController(DriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var drivers = await _driverService.GetAllAsync(includeInactive);
        return Ok(drivers);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var driver = await _driverService.GetByIdAsync(id);

        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        return Ok(driver);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDriverRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var driver = await _driverService.CreateAsync(request);

        if (driver == null)
            return BadRequest(new { message = "User not found for the provided UserId" });

        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var driver = await _driverService.UpdateAsync(id, request);

        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        return Ok(driver);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _driverService.SoftDeleteAsync(id);

        if (!success)
            return NotFound(new { message = "Driver not found" });

        return Ok(new { message = "Driver deleted successfully" });
    }
}