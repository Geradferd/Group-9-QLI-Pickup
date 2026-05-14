using Api.Data;
using Api.DTOs;
using Api.Models;
using Api.Services;
using Api.Tests.Helpers;
using NUnit.Framework;

namespace Api.Tests;

/// <summary>
/// Tests the TripService state machine:
///   - Valid status transitions are allowed
///   - Invalid / out-of-order transitions are rejected (return null)
///   - Terminal statuses cannot be cancelled
///   - Audit history is recorded for every change
/// </summary>
[TestFixture]
public class TripStateMachineTests
{
    private AppDbContext _ctx = null!;
    private TripService _svc = null!;
    private int _adminId;
    private int _ttId;

    [SetUp]
    public void SetUp()
    {
        _ctx = TestDbFactory.CreateInMemory();
        (_adminId, _ttId) = TestDbFactory.SeedDefaults(_ctx);
        _svc = new TripService(_ctx, new NotificationDispatcher(_ctx));
    }

    [TearDown]
    public void TearDown() => _ctx.Dispose();

    // ───────────────────────────────────────────────
    // CREATE
    // ───────────────────────────────────────────────

    [Test]
    public async Task CreateTrip_SetsStatusToAuthorized()
    {
        var req = MakeCreateRequest();
        var result = await _svc.CreateAsync(req, _adminId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo("Authorized"));
    }

    [Test]
    public async Task CreateTrip_RecordsAuditHistory()
    {
        var req = MakeCreateRequest();
        var result = await _svc.CreateAsync(req, _adminId);

        var history = _ctx.TripStatusHistories.Where(h => h.TripId == result.Id).ToList();
        Assert.That(history, Has.Count.GreaterThanOrEqualTo(1));
        Assert.That(history.First().ToStatus, Is.EqualTo(TripStatus.Authorized));
    }

    // ───────────────────────────────────────────────
    // VALID TRANSITIONS
    // ───────────────────────────────────────────────

    [Test]
    public async Task AcceptTrip_FromAuthorized_Succeeds()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        var result = await _svc.AcceptAsync(trip.Id, _adminId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Accepted"));
    }

    [Test]
    public async Task CancelTrip_FromAuthorized_Succeeds()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        var result = await _svc.CancelAsync(trip.Id, _adminId, "No longer needed");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Cancelled"));
    }

    [Test]
    public async Task CancelTrip_FromAccepted_Succeeds()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.AcceptAsync(trip.Id, _adminId);
        var result = await _svc.CancelAsync(trip.Id, _adminId, "Changed mind");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Cancelled"));
    }

    [Test]
    public async Task AssignTrip_FromAccepted_Succeeds()
    {
        // Seed driver + vehicle
        var driverUser = new User { Email = "d1@qli.test", PasswordHash = "x", DisplayName = "D1", Role = UserRole.Driver, IsActive = true };
        _ctx.Users.Add(driverUser);
        await _ctx.SaveChangesAsync();
        var driver = new Driver { UserId = driverUser.Id, FirstName = "John", LastName = "Smith", LicenseNumber = "LIC001", IsActive = true };
        var vehicle = new Vehicle { Make = "Ford", Model = "Transit", Year = 2022, LicensePlate = "ABC123", IsActive = true };
        _ctx.Drivers.Add(driver);
        _ctx.Vehicles.Add(vehicle);
        await _ctx.SaveChangesAsync();

        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.AcceptAsync(trip.Id, _adminId);

        var assignReq = new AssignTripRequest { DriverId = driver.Id, VehicleId = vehicle.Id };
        var result = await _svc.AssignAsync(trip.Id, assignReq, _adminId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Scheduled"));
    }

    [Test]
    public async Task StartTrip_FromScheduled_Succeeds()
    {
        var driverUser = new User { Email = "d2@qli.test", PasswordHash = "x", DisplayName = "D2", Role = UserRole.Driver, IsActive = true };
        _ctx.Users.Add(driverUser);
        await _ctx.SaveChangesAsync();
        var driver = new Driver { UserId = driverUser.Id, FirstName = "Jane", LastName = "Doe", LicenseNumber = "LIC002", IsActive = true };
        var vehicle = new Vehicle { Make = "Dodge", Model = "Caravan", Year = 2021, LicensePlate = "XYZ999", IsActive = true };
        _ctx.Drivers.Add(driver);
        _ctx.Vehicles.Add(vehicle);
        await _ctx.SaveChangesAsync();

        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.AcceptAsync(trip.Id, _adminId);
        await _svc.AssignAsync(trip.Id, new AssignTripRequest { DriverId = driver.Id, VehicleId = vehicle.Id }, _adminId);
        var result = await _svc.StartAsync(trip.Id, _adminId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("InProgress"));
    }

    [Test]
    public async Task CompleteTrip_FromInProgress_Succeeds()
    {
        var (trip, driver, vehicle) = await CreateScheduledTrip();
        await _svc.StartAsync(trip.Id, _adminId);
        var result = await _svc.CompleteAsync(trip.Id, _adminId, new CompleteTripRequest { DistanceMiles = 5.2 });

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Completed"));
    }

    [Test]
    public async Task NoShow_FromInProgress_Succeeds()
    {
        var (trip, _, _) = await CreateScheduledTrip();
        await _svc.StartAsync(trip.Id, _adminId);
        var result = await _svc.NoShowAsync(trip.Id, _adminId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("NoShow"));
    }

    // ───────────────────────────────────────────────
    // INVALID / REJECTED TRANSITIONS
    // ───────────────────────────────────────────────

    [Test]
    public async Task AcceptTrip_FromScheduled_ReturnsNull()
    {
        var (trip, _, _) = await CreateScheduledTrip();
        var result = await _svc.AcceptAsync(trip.Id, _adminId);

        Assert.That(result, Is.Null, "Accept should be rejected when trip is already Scheduled");
    }

    [Test]
    public async Task StartTrip_FromAuthorized_ReturnsNull()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        var result = await _svc.StartAsync(trip.Id, _adminId);

        Assert.That(result, Is.Null, "Cannot start a trip that is not yet Scheduled");
    }

    [Test]
    public async Task CompleteTrip_FromAuthorized_ReturnsNull()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        var result = await _svc.CompleteAsync(trip.Id, _adminId, new CompleteTripRequest { DistanceMiles = 3 });

        Assert.That(result, Is.Null, "Cannot complete a trip that has not started");
    }

    [Test]
    public async Task CancelTrip_AfterCompleted_ReturnsNull()
    {
        var (trip, _, _) = await CreateScheduledTrip();
        await _svc.StartAsync(trip.Id, _adminId);
        await _svc.CompleteAsync(trip.Id, _adminId, new CompleteTripRequest { DistanceMiles = 1 });

        var result = await _svc.CancelAsync(trip.Id, _adminId, "Too late");
        Assert.That(result, Is.Null, "Cannot cancel a completed trip");
    }

    [Test]
    public async Task CancelTrip_AfterCancelled_ReturnsNull()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.CancelAsync(trip.Id, _adminId, "First cancel");
        var result = await _svc.CancelAsync(trip.Id, _adminId, "Second cancel");

        Assert.That(result, Is.Null, "Cannot cancel an already-cancelled trip");
    }

    // ───────────────────────────────────────────────
    // SOFT DELETE / UPDATE
    // ───────────────────────────────────────────────

    [Test]
    public async Task SoftDelete_HidesTripFromGetAll()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.SoftDeleteAsync(trip.Id);

        var all = await _svc.GetAllAsync(new TripQueryParams());
        Assert.That(all.Any(t => t.Id == trip.Id), Is.False);
    }

    [Test]
    public async Task UpdateTrip_ChangesPickupAddress()
    {
        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        var updated = await _svc.UpdateAsync(trip.Id, new UpdateTripRequest { PickupAddress = "456 New Ave" });

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.PickupAddress, Is.EqualTo("456 New Ave"));
    }

    [Test]
    public async Task UpdateTrip_WhenInProgress_ReturnsNull()
    {
        var (trip, _, _) = await CreateScheduledTrip();
        await _svc.StartAsync(trip.Id, _adminId);

        var result = await _svc.UpdateAsync(trip.Id, new UpdateTripRequest { PickupAddress = "Should Fail" });
        Assert.That(result, Is.Null, "Locked trips should not be updatable");
    }

    // ───────────────────────────────────────────────
    // AUDIT HISTORY
    // ───────────────────────────────────────────────

    [Test]
    public async Task FullLifecycle_RecordsAllStatusChanges()
    {
        var (trip, _, _) = await CreateScheduledTrip();
        await _svc.StartAsync(trip.Id, _adminId);
        await _svc.CompleteAsync(trip.Id, _adminId, new CompleteTripRequest { DistanceMiles = 10 });

        var history = _ctx.TripStatusHistories
            .Where(h => h.TripId == trip.Id)
            .OrderBy(h => h.ChangedAt)
            .ToList();

        // Authorized (create) → Accepted → Scheduled → InProgress → Completed = 5 entries
        Assert.That(history.Count, Is.GreaterThanOrEqualTo(4));
        Assert.That(history.Last().ToStatus, Is.EqualTo(TripStatus.Completed));
    }

    // ───────────────────────────────────────────────
    // HELPERS
    // ───────────────────────────────────────────────

    private CreateTripRequest MakeCreateRequest() => new()
    {
        TransportationTypeId = _ttId,
        PickupAddress = "123 Main St",
        DestinationAddress = "789 Oak Ave",
        ScheduledPickupTime = DateTime.Now.AddDays(1),
        PassengerCount = 1
    };

    /// Helper: creates a trip and advances it all the way to Scheduled.
    private async Task<(TripResponse trip, Driver driver, Vehicle vehicle)> CreateScheduledTrip()
    {
        var driverUser = new User { Email = $"drv-{Guid.NewGuid():N}@qli.test", PasswordHash = "x", DisplayName = "Driver", Role = UserRole.Driver, IsActive = true };
        _ctx.Users.Add(driverUser);
        await _ctx.SaveChangesAsync();

        var driver = new Driver { UserId = driverUser.Id, FirstName = "Test", LastName = "Driver", LicenseNumber = $"LIC-{Guid.NewGuid():N}", IsActive = true };
        var plate = Guid.NewGuid().ToString("N")[..8].ToUpper();
        var vehicle = new Vehicle { Make = "Toyota", Model = "Sienna", Year = 2023, LicensePlate = plate, IsActive = true };
        _ctx.Drivers.Add(driver);
        _ctx.Vehicles.Add(vehicle);
        await _ctx.SaveChangesAsync();

        var trip = await _svc.CreateAsync(MakeCreateRequest(), _adminId);
        await _svc.AcceptAsync(trip.Id, _adminId);
        await _svc.AssignAsync(trip.Id, new AssignTripRequest { DriverId = driver.Id, VehicleId = vehicle.Id }, _adminId);

        var latest = (await _svc.GetByIdAsync(trip.Id))!;
        return (latest, driver, vehicle);
    }
}
