using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;
/// Trip Service - handles all trip business logic
/// Original CRUD operations (GetAll, GetById, Create, Update, SoftDelete)
/// were created by Gavin .
///
/// Added by Angel:
/// - State machine (ValidTransitions dictionary) that enforces only valid status changes 
///  -Pending can only go to Approved, Denied, or Cancelled — not directly to Completed
/// - Status action methods: Approve, Deny, Assign, Start, Complete, NoShow, Cancel
/// - Audit logging: every status change is recorded in TripStatusHistory with who, when, and why
/// - Added audit log call to CreateAsync so trip creation is also tracked
/// - Auto-dispatch notifications on every status change (FR-19)

public class TripService
{
    private readonly AppDbContext _context;
    private readonly NotificationDispatcher _dispatcher;

    public TripService(AppDbContext context, NotificationDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    /// ========================
    /// STATE MACHINE (FR-07)
    /// Only these transitions are allowed — anything else gets rejected
    /// ========================
    private static readonly Dictionary<TripStatus, List<TripStatus>> ValidTransitions = new()
    {
        { TripStatus.Pending,     new List<TripStatus> { TripStatus.Authorized, TripStatus.Cancelled } },
        { TripStatus.Authorized,  new List<TripStatus> { TripStatus.Accepted, TripStatus.Cancelled } },
        { TripStatus.Accepted,    new List<TripStatus> { TripStatus.Scheduled, TripStatus.Cancelled } },
        { TripStatus.Approved,    new List<TripStatus> { TripStatus.Scheduled, TripStatus.Cancelled } }, // legacy
        { TripStatus.Scheduled,   new List<TripStatus> { TripStatus.InProgress, TripStatus.Cancelled } },
        { TripStatus.InProgress,  new List<TripStatus> { TripStatus.Completed, TripStatus.NoShow } },
    };

    /// Terminal statuses — cancel is never allowed once here
    private static readonly TripStatus[] TerminalStatuses =
    [
        TripStatus.Completed,
        TripStatus.Cancelled,
        TripStatus.Denied,
        TripStatus.NoShow,
    ];

    /// Statuses where the trip can no longer be edited
    private static readonly TripStatus[] LockedStatuses =
    [
        TripStatus.InProgress,
        TripStatus.Completed,
        TripStatus.Cancelled,
        TripStatus.Denied,
        TripStatus.NoShow
    ];

    /// ========================
    /// GET ALL TRIPS with filters (FR-06)
    /// ========================
    public async Task<List<TripResponse>> GetAllAsync(TripQueryParams query)
    {
        var q = _context.Trips
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (query.Status.HasValue)
            q = q.Where(t => t.Status == query.Status.Value);
        if (query.RiderId.HasValue)
            q = q.Where(t => t.RiderId == query.RiderId.Value);
        if (query.DriverId.HasValue)
            q = q.Where(t => t.DriverId == query.DriverId.Value);
        if (query.TransportationTypeId.HasValue)
            q = q.Where(t => t.TransportationTypeId == query.TransportationTypeId.Value);
        if (query.Date.HasValue)
            q = q.Where(t => t.ScheduledPickupTime.Date == query.Date.Value.Date);

        var trips = await q
            .OrderBy(t => t.ScheduledPickupTime)
            .ToListAsync();

        return trips.Select(MapToResponse).ToList();
    }

    /// ========================
    /// GET TRIP BY ID
    /// ========================
    public async Task<TripResponse?> GetByIdAsync(int id)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null)
            return null;

        return MapToResponse(trip);
    }

    /// ========================
    /// CREATE TRIP (FR-01)
    /// ========================
    public async Task<TripResponse> CreateAsync(CreateTripRequest request, int requestedByUserId)
    {
        var trip = new Trip
        {
            RiderId = request.RiderId,
            TransportationTypeId = request.TransportationTypeId,
            Status = TripStatus.Authorized,
            PickupAddress = request.PickupAddress,
            DestinationAddress = request.DestinationAddress,
            ScheduledPickupTime = request.ScheduledPickupTime,
            PassengerCount = request.PassengerCount,
            RequiresWheelchair = request.RequiresWheelchair,
            Notes = request.Notes,
            RequestedByUserId = requestedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        /// Log directly as Authorized since admin creates it pre-authorized
        await LogStatusChange(trip.Id, TripStatus.Authorized, TripStatus.Authorized, requestedByUserId, "Trip created and authorized by admin");

        /// Notify about the new authorized trip
        await _dispatcher.DispatchAsync(trip.Id, TripStatus.Authorized, requestedByUserId);

        return MapToResponse(trip);
    }

    /// ========================
    /// UPDATE TRIP (FR-05 - only before InProgress)
    /// ========================
    public async Task<TripResponse?> UpdateAsync(int id, UpdateTripRequest request)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null)
            return null;

        if (LockedStatuses.Contains(trip.Status))
            return null;

        if (request.RiderId.HasValue)
            trip.RiderId = request.RiderId;
        if (request.TransportationTypeId.HasValue)
            trip.TransportationTypeId = request.TransportationTypeId.Value;
        if (!string.IsNullOrEmpty(request.PickupAddress))
            trip.PickupAddress = request.PickupAddress;
        if (!string.IsNullOrEmpty(request.DestinationAddress))
            trip.DestinationAddress = request.DestinationAddress;
        if (request.ScheduledPickupTime.HasValue)
            trip.ScheduledPickupTime = request.ScheduledPickupTime.Value;
        if (request.PassengerCount.HasValue)
            trip.PassengerCount = request.PassengerCount.Value;
        if (request.RequiresWheelchair.HasValue)
            trip.RequiresWheelchair = request.RequiresWheelchair.Value;
        if (request.Notes != null)
            trip.Notes = request.Notes;

        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToResponse(trip);
    }

    // ========================
    // SOFT DELETE
    // ========================
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null)
            return false;

        trip.IsDeleted = true;
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// ========================
    /// AUTHORIZE (Admin) — Pending → Authorized
    /// ========================
    public async Task<TripResponse?> AuthorizeAsync(int id, int userId)
    {
        return await ChangeStatus(id, TripStatus.Authorized, userId, "Trip authorized by admin");
    }

    /// ========================
    /// ACCEPT (Rider) — Authorized → Accepted
    /// ========================
    public async Task<TripResponse?> AcceptAsync(int id, int userId)
    {
        return await ChangeStatus(id, TripStatus.Accepted, userId, "Trip accepted by rider");
    }

    /// ========================
    /// APPROVE (legacy alias → Authorized) — kept for compatibility
    /// ========================
    public async Task<TripResponse?> ApproveAsync(int id, int userId)
    {
        return await AuthorizeAsync(id, userId);
    }

    /// ========================
    /// DENY (FR-02) — Pending → Denied (reason required)
    /// ========================
    public async Task<TripResponse?> DenyAsync(int id, int userId, string reason)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null || !IsValidTransition(trip.Status, TripStatus.Denied))
            return null;

        var fromStatus = trip.Status;
        trip.Status = TripStatus.Denied;
        trip.DenialReason = reason;
        trip.ApprovedByUserId = userId;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, TripStatus.Denied, userId, reason);
        await _dispatcher.DispatchAsync(id, TripStatus.Denied, userId);

        return MapToResponse(trip);
    }

    /// ========================
    /// ASSIGN (FR-03) — Approved → Scheduled (assign driver + vehicle)
    /// ========================
    public async Task<TripResponse?> AssignAsync(int id, AssignTripRequest request, int userId)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null || !IsValidTransition(trip.Status, TripStatus.Scheduled))
            return null;

        /// Validate driver exists and is active
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == request.DriverId && d.IsActive);
        if (driver == null)
            return null;

        /// Validate vehicle exists and is active
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.IsActive);
        if (vehicle == null)
            return null;

        var fromStatus = trip.Status;
        trip.Status = TripStatus.Scheduled;
        trip.DriverId = request.DriverId;
        trip.VehicleId = request.VehicleId;
        trip.ApprovedByUserId = userId;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, TripStatus.Scheduled, userId, "Driver and vehicle assigned");
        await _dispatcher.DispatchAsync(id, TripStatus.Scheduled, userId);

        return MapToResponse(trip);
    }

    /// ========================
    /// START (FR-04) — Scheduled → InProgress
    /// ========================
    public async Task<TripResponse?> StartAsync(int id, int userId)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null || !IsValidTransition(trip.Status, TripStatus.InProgress))
            return null;

        var fromStatus = trip.Status;
        trip.Status = TripStatus.InProgress;
        trip.ActualPickupTime = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, TripStatus.InProgress, userId, "Trip started");
        await _dispatcher.DispatchAsync(id, TripStatus.InProgress, userId);

        return MapToResponse(trip);
    }

    /// ========================
    /// COMPLETE (FR-04) — InProgress → Completed
    /// ========================
    public async Task<TripResponse?> CompleteAsync(int id, int userId, CompleteTripRequest request)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null || !IsValidTransition(trip.Status, TripStatus.Completed))
            return null;

        var fromStatus = trip.Status;
        trip.Status = TripStatus.Completed;
        trip.ActualDropoffTime = DateTime.UtcNow;
        trip.DistanceMiles = request.DistanceMiles;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, TripStatus.Completed, userId, "Trip completed");
        await _dispatcher.DispatchAsync(id, TripStatus.Completed, userId);

        return MapToResponse(trip);
    }

    /// ========================
    /// NO SHOW (FR-04) — InProgress → NoShow
    /// ========================
    public async Task<TripResponse?> NoShowAsync(int id, int userId)
    {
        return await ChangeStatus(id, TripStatus.NoShow, userId, "Rider did not show up");
    }

    /// ========================
    /// CANCEL (FR-02) — Pending/Approved/Scheduled → Cancelled
    /// ========================
    public async Task<TripResponse?> CancelAsync(int id, int userId, string? reason)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (trip == null || TerminalStatuses.Contains(trip.Status))
            return null;

        var fromStatus = trip.Status;
        trip.Status = TripStatus.Cancelled;
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, TripStatus.Cancelled, userId, reason ?? "Trip cancelled");
        await _dispatcher.DispatchAsync(id, TripStatus.Cancelled, userId);
        return MapToResponse(trip);
    }

    /// ========================
    /// HELPER — Change status with validation and audit logging
    /// ========================
    private async Task<TripResponse?> ChangeStatus(int id, TripStatus newStatus, int userId, string reason)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (trip == null || !IsValidTransition(trip.Status, newStatus))
            return null;

        var fromStatus = trip.Status;
        trip.Status = newStatus;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await LogStatusChange(id, fromStatus, newStatus, userId, reason);
        await _dispatcher.DispatchAsync(id, newStatus, userId);

        return MapToResponse(trip);
    }

    /// ========================
    /// HELPER — Check if transition is valid using state machine
    /// ========================
    private bool IsValidTransition(TripStatus from, TripStatus to)
    {
        if (!ValidTransitions.ContainsKey(from))
            return false;
        return ValidTransitions[from].Contains(to);
    }

    /// ========================
    /// HELPER — Log every status change to TripStatusHistory (FR-07)
    /// ========================
    private async Task LogStatusChange(int tripId, TripStatus from, TripStatus to, int userId, string reason)
    {
        var history = new TripStatusHistory
        {
            TripId = tripId,
            FromStatus = from,
            ToStatus = to,
            ChangedByUserId = userId,
            Reason = reason,
            ChangedAt = DateTime.UtcNow
        };

        _context.TripStatusHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    /// ========================
    /// HELPER — Map Trip to Response DTO
    /// ========================
    private TripResponse MapToResponse(Trip trip)
    {
        return new TripResponse
        {
            Id = trip.Id,
            Status = trip.Status.ToString(),
            PickupAddress = trip.PickupAddress,
            DestinationAddress = trip.DestinationAddress,
            ScheduledPickupTime = trip.ScheduledPickupTime,
            ActualPickupTime = trip.ActualPickupTime,
            ActualDropoffTime = trip.ActualDropoffTime,
            PassengerCount = trip.PassengerCount,
            RequiresWheelchair = trip.RequiresWheelchair,
            DistanceMiles = trip.DistanceMiles,
            Notes = trip.Notes
        };
    }
}
