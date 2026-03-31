using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripStatusHistoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public TripStatusHistoryController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetByTrip(int tripId)
    {
        var history = await _db.TripStatusHistories
            .AsNoTracking()
            .Where(h => h.TripId == tripId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        return Ok(history);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTripStatusHistoryRequest request)
    {
        var trip = await _db.Trips.FindAsync(request.TripId);
        if (trip == null)
            return NotFound($"Trip with id {request.TripId} not found.");

        var changedBy = await _db.Users.FindAsync(request.ChangedByUserId);
        if (changedBy == null)
            return NotFound($"User with id {request.ChangedByUserId} not found.");

        var entry = new TripStatusHistory
        {
            TripId = request.TripId,
            FromStatus = request.FromStatus,
            ToStatus = request.ToStatus,
            ChangedByUserId = request.ChangedByUserId,
            Reason = request.Reason,
            ChangedAt = DateTime.UtcNow
        };

        trip.Status = request.ToStatus;
        trip.UpdatedAt = DateTime.UtcNow;

        _db.TripStatusHistories.Add(entry);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByTrip), new { tripId = entry.TripId }, entry);
    }

    public class CreateTripStatusHistoryRequest
    {
        public int TripId { get; set; }
        public TripStatus FromStatus { get; set; }
        public TripStatus ToStatus { get; set; }
        public int ChangedByUserId { get; set; }
        public string? Reason { get; set; }
    }
}
