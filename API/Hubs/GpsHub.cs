using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Api.DTOs;
using System.Security.Claims;

namespace Api.Hubs;

// SignalR Hub for real-time GPS location tracking
// Drivers push their location updates, clients subscribe to receive real-time updates
// Supports both individual driver tracking and broadcast of all active drivers
[Authorize]
public class GpsHub : Hub
{
    private readonly ILogger<GpsHub> _logger;

    public GpsHub(ILogger<GpsHub> logger)
    {
        _logger = logger;
    }

    // Override OnConnected to track connected users
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
        
        _logger.LogInformation($"User {userId} (Role: {userRole}) connected to GPS hub. ConnectionId: {Context.ConnectionId}");
        
        await base.OnConnectedAsync();
    }

    // Override OnDisconnected
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        _logger.LogInformation($"User {userId} disconnected from GPS hub. ConnectionId: {Context.ConnectionId}");
        
        await base.OnDisconnectedAsync(exception);
    }

    // Driver calls this to broadcast their current location
    // Called by: Driver clients
    public async Task BroadcastDriverLocation(RealtimeLocationUpdate locationUpdate)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole != "Driver")
        {
            _logger.LogWarning($"Non-driver user {userId} attempted to broadcast location");
            await Clients.Caller.SendAsync("Error", "Only drivers can broadcast location");
            return;
        }

        if (locationUpdate.Timestamp == default)
            locationUpdate.Timestamp = DateTime.UtcNow;

        _logger.LogInformation($"Driver {locationUpdate.DriverId} broadcast location: ({locationUpdate.Latitude}, {locationUpdate.Longitude})");

        // Broadcast to all connected clients
        await Clients.All.SendAsync("DriverLocationUpdated", locationUpdate);
    }

    // Clients call this to subscribe to a specific driver's real-time location
    // Called by: Riders or admin users wanting to track a specific driver
    public async Task SubscribeToDriver(int driverId)
    {
        var groupName = $"driver-{driverId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation($"User subscribed to driver {driverId}");
        await Clients.Caller.SendAsync("SubscriptionConfirmed", $"Subscribed to driver {driverId}");
    }

    // Driver-specific location update (sent only to subscribers of that driver)
    public async Task BroadcastDriverLocationToSubscribers(RealtimeLocationUpdate locationUpdate)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole != "Driver")
        {
            _logger.LogWarning($"Non-driver user {userId} attempted to broadcast location");
            await Clients.Caller.SendAsync("Error", "Only drivers can broadcast location");
            return;
        }

        if (locationUpdate.Timestamp == default)
            locationUpdate.Timestamp = DateTime.UtcNow;

        var groupName = $"driver-{locationUpdate.DriverId}";
        
        _logger.LogInformation($"Driver {locationUpdate.DriverId} broadcast location to subscribers");

        // Send to all subscribers of this specific driver
        await Clients.Group(groupName).SendAsync("DriverLocationUpdated", locationUpdate);
    }

    // Clients call this to unsubscribe from a driver
    public async Task UnsubscribeFromDriver(int driverId)
    {
        var groupName = $"driver-{driverId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation($"User unsubscribed from driver {driverId}");
        await Clients.Caller.SendAsync("UnsubscriptionConfirmed", $"Unsubscribed from driver {driverId}");
    }

    // Broadcast all active drivers' locations
    public async Task BroadcastAllActiveDrivers(ActiveDriversPositionsResponse response)
    {
        _logger.LogInformation($"Broadcasting {response.ActiveDrivers.Count} active driver positions");
        
        // Broadcast to all connected clients
        await Clients.All.SendAsync("AllActiveDriversUpdated", response);
    }

    // Request current location from specific driver (for polling)
    public async Task RequestDriverLocation(int driverId)
    {
        var driverGroupName = $"driver-{driverId}";
        
        _logger.LogInformation($"Location requested for driver {driverId}");
        
        // Notify the driver that their location is being requested
        await Clients.Group($"driver-active-{driverId}").SendAsync("LocationRequested");
    }
}
