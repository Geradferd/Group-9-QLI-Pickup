using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripService _tripService;

    public TripsController(TripService tripService)
    {
        _tripService = tripService;
    }

    // GET /api/trips
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] TripQueryParams query)
    {
        var trips = await _tripService.GetAllAsync(query);
        return Ok(trips);
    }

    // GET /api/trips/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await _tripService.GetByIdAsync(id);
        if (trip == null)
            return NotFound(new { message = "Trip not found" });
        return Ok(trip);
    }

    // POST /api/trips
    [HttpPost]
    [Authorize]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
    
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var trip = await _tripService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    // PUT /api/trips/{id}
    // Only editable until InProgress 
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTripRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var trip = await _tripService.UpdateAsync(id, request);
        if (trip == null)
            return BadRequest(new { message = "Trip not found or cannot be edited once it is in progress" });
        return Ok(trip);
    }

    // DELETE /api/trips/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _tripService.SoftDeleteAsync(id);
        if (!success)
            return NotFound(new { message = "Trip not found" });
        return Ok(new { message = "Trip deleted successfully" });
    }
}
