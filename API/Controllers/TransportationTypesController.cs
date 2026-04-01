using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers;

// API controller for managing transportation types
// [Route("api/[controller]")] means all endpoints start with /api/transportationtypes
// Most operations require Admin role for security
[ApiController]
[Route("api/[controller]")]
public class TransportationTypesController : ControllerBase
{
    private readonly TransportationTypeService _transportationTypeService;

    public TransportationTypesController(TransportationTypeService transportationTypeService)
    {
        _transportationTypeService = transportationTypeService;
    }

    // GET /api/transportationtypes
    // Get all transportation types (excluding soft-deleted ones)
    // Anyone authenticated can view transportation types
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
    {
        // Only admins can see deleted types
        if (includeDeleted && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var types = await _transportationTypeService.GetAllAsync(includeDeleted);
        return Ok(types);
    }

    // GET /api/transportationtypes/{id}
    // Get a specific transportation type by ID
    // Anyone authenticated can view a specific transportation type
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _transportationTypeService.GetByIdAsync(id);

        if (type == null)
            return NotFound(new { message = "Transportation type not found" });

        return Ok(type);
    }

    // POST /api/transportationtypes
    // Create a new transportation type
    // Only admins can create new transportation types
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTransportationTypeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var type = await _transportationTypeService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    // PUT /api/transportationtypes/{id}
    // Update an existing transportation type
    // Only admins can update transportation types
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransportationTypeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var type = await _transportationTypeService.UpdateAsync(id, request);

        if (type == null)
            return NotFound(new { message = "Transportation type not found" });

        return Ok(type);
    }

    // DELETE /api/transportationtypes/{id}
    // Soft delete a transportation type
    // Only admins can delete transportation types
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _transportationTypeService.SoftDeleteAsync(id);

        if (!success)
            return BadRequest(new { message = "Transportation type not found or has active trips associated with it" });

        return Ok(new { message = "Transportation type deleted successfully" });
    }
}
