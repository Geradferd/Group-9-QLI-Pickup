using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using Api.Services;
using System.Security.Claims;

/// GPS Controller - API endpoints for GPS tracking
/// Original endpoints created by team
///
/// Updated by Angel:
/// - PostBreadcrumb handles deduplication response (null = point skipped)
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GPSController : ControllerBase
{
    private readonly GPSService _gpsService;
    private readonly ILogger<GPSController> _logger;

    public GPSController(GPSService gpsService, ILogger<GPSController> logger)
    {
        _gpsService = gpsService;
        _logger = logger;
    }

    /// POST /api/gps/breadcrumb
    /// Record a new GPS location (breadcrumb) for the current driver
    /// Must be authenticated as a driver
    [HttpPost("breadcrumb")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> PostBreadcrumb([FromBody] CreateGPSBreadcrumbRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var driverId))
                return Unauthorized(new { message = "Invalid user context" });

            var breadcrumb = await _gpsService.CreateBreadcrumbAsync(driverId, request);
            
            // Null means the point was deduplicated (too close to last point)
            if (breadcrumb == null)
                return Ok(new { message = "Point skipped - too close to last recorded position" });

            _logger.LogInformation($"Driver {driverId} recorded GPS location: ({breadcrumb.Latitude}, {breadcrumb.Longitude})");
            
            return CreatedAtAction(nameof(PostBreadcrumb), new { id = breadcrumb.Id }, breadcrumb);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error recording breadcrumb: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// GET /api/gps/trip/{tripId}/breadcrumbs
    /// Get all GPS breadcrumbs (location history) for a specific trip
    /// Anyone authenticated can view trip breadcrumbs
    [HttpGet("trip/{tripId}/breadcrumbs")]
    public async Task<IActionResult> GetTripBreadcrumbs(int tripId)
    {
        try
        {
            var breadcrumbs = await _gpsService.GetTripBreadcrumbsAsync(tripId);
            return Ok(breadcrumbs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving trip breadcrumbs: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// GET /api/gps/driver/{driverId}/latest
    /// Get the latest GPS position for a specific driver
    /// Anyone authenticated can view driver positions
    [HttpGet("driver/{driverId}/latest")]
    public async Task<IActionResult> GetDriverLatestPosition(int driverId)
    {
        try
        {
            var position = await _gpsService.GetDriverLatestPositionAsync(driverId);
            
            if (position == null)
                return NotFound(new { message = "Driver not found or no location history" });

            return Ok(position);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving driver position: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// GET /api/gps/drivers/active
    /// Get latest positions for all active drivers (reported location within last 30 minutes)
    /// Anyone authenticated can view active driver positions
    [HttpGet("drivers/active")]
    public async Task<IActionResult> GetActiveDriversPositions([FromQuery] int minutesThreshold = 30)
    {
        try
        {
            var positions = await _gpsService.GetActiveDriversPositionsAsync(minutesThreshold);
            var response = new ActiveDriversPositionsResponse
            {
                ActiveDrivers = positions,
                Timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving active drivers: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }
}
