using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;
/// Trips Controller - API endpoints for trip management
/// Original CRUD endpoints (GetAll, GetById, Create, Update, Delete)
/// were created by Gavin
///
/// Added by Angel:
/// - GetUserId() helper to extract user ID from JWT token
/// - POST /api/trips/{id}/approve — admin approves a pending trip 
/// - POST /api/trips/{id}/deny — admin denies a trip with reason 
/// - POST /api/trips/{id}/assign — admin assigns driver and vehicle 
/// - POST /api/trips/{id}/start — driver starts the trip 
/// - POST /api/trips/{id}/complete — driver completes the trip 
/// - POST /api/trips/{id}/noshow — driver marks rider as absent 
/// - POST /api/trips/{id}/cancel — any user cancels a trip 
/// - Role-based access: admin-only for approve/deny/assign, driver-only for start/complete/noshow

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripService _tripService;

    public TripsController(TripService tripService)
    {
        _tripService = tripService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] TripQueryParams query)
    {
        var trips = await _tripService.GetAllAsync(query);
        return Ok(trips);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await _tripService.GetByIdAsync(id);
        if (trip == null)
            return NotFound(new { message = "Trip not found" });
        return Ok(trip);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var trip = await _tripService.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _tripService.SoftDeleteAsync(id);
        if (!success)
            return NotFound(new { message = "Trip not found" });
        return Ok(new { message = "Trip deleted successfully" });
    }

    // TRIP STATUS ACTIONS 

    [HttpPost("{id}/authorize")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Authorize(int id)
    {
        var trip = await _tripService.AuthorizeAsync(id, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }

    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Rider,Admin")]
    public async Task<IActionResult> Accept(int id)
    {
        var trip = await _tripService.AcceptAsync(id, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var trip = await _tripService.AuthorizeAsync(id, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }

 
    [HttpPost("{id}/deny")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deny(int id, [FromBody] DenyTripRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var trip = await _tripService.DenyAsync(id, GetUserId(), request.Reason);
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }


    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignTripRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var trip = await _tripService.AssignAsync(id, request, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found, invalid transition, or driver/vehicle not found" });
        return Ok(trip);
    }

    [HttpPost("{id}/start")]
    [Authorize(Roles = "Driver,Admin")]
    public async Task<IActionResult> Start(int id)
    {
        var trip = await _tripService.StartAsync(id, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }


    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Driver,Admin")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteTripRequest request)
    {
        var trip = await _tripService.CompleteAsync(id, GetUserId(), request);
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }

 
    [HttpPost("{id}/noshow")]
    [Authorize(Roles = "Driver,Admin")]
    public async Task<IActionResult> NoShow(int id)
    {
        var trip = await _tripService.NoShowAsync(id, GetUserId());
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }


    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelTripRequest request)
    {
        var trip = await _tripService.CancelAsync(id, GetUserId(), request.Reason);
        if (trip == null)
            return BadRequest(new { message = "Trip not found or invalid status transition" });
        return Ok(trip);
    }
}
