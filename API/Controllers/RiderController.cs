using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;

/// Admin can do everything, Riders can only view
[ApiController]
[Route("api/[controller]")]
public class RiderController : ControllerBase
{
    private readonly RiderService _riderService;

    public RiderController(RiderService riderService)
    {
        _riderService = riderService;
    }

    /// GET ALL RIDERS
    /// Any authenticated user can view riders
    /// Only admins can see inactive riders
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var riders = await _riderService.GetAllAsync(includeInactive);
        return Ok(riders);
    }

    /// GET RIDER BY ID
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var rider = await _riderService.GetByIdAsync(id);

        if (rider == null)
            return NotFound(new { message = "Rider not found" });

        return Ok(rider);
    }


    /// CREATE A NEW RIDER
    /// Only admins can create rider profiles
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateRiderRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var rider = await _riderService.CreateAsync(request);

        if (rider == null)
            return BadRequest(new { message = "User not found for the provided UserId" });

        return CreatedAtAction(nameof(GetById), new { id = rider.Id }, rider);
    }

    /// UPDATE A RIDER
    /// Only admins can update rider profiles
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRiderRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var rider = await _riderService.UpdateAsync(id, request);

        if (rider == null)
            return NotFound(new { message = "Rider not found" });

        return Ok(rider);
    }

  
    /// SOFT DELETE A RIDER
    /// Only admins can delete rider profiles
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _riderService.SoftDeleteAsync(id);

        if (!success)
            return NotFound(new { message = "Rider not found" });

        return Ok(new { message = "Rider deleted successfully" });
    }
}
