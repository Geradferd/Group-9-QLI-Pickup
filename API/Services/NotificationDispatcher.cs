using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Api.Services;
/// Notification Dispatcher - auto-creates notifications on trip status changes 
/// Called by TripService after every status transition
/// Routes notifications to the right person based on status:
///   Pending = admins notified of new request
///   Approved/Denied = requester notified
///   Scheduled = driver and requester notified
///   InProgress/Completed = requester notified
///   NoShow = admins notified
///   Cancelled = requester and assigned driver notified

/// Automatically creates notifications when trip status changes (FR-19)
/// Called by TripService after each status transition
/// Each status change notifies the right person with the right message
public class NotificationDispatcher
{
    private readonly AppDbContext _context;

    public NotificationDispatcher(AppDbContext context)
    {
        _context = context;
    }

    /// Main method - called after every trip status change
    /// Determines who to notify and what to say based on the new status
    public async Task DispatchAsync(int tripId, TripStatus newStatus, int changedByUserId)
    {
        var trip = await _context.Trips
            .Include(t => t.Rider)
            .FirstOrDefaultAsync(t => t.Id == tripId);

        if (trip == null) return;

        switch (newStatus)
        {
            case TripStatus.Pending:
                // Notify all admins that a new trip was requested
                await NotifyAdmins(
                    tripId,
                    "New Trip Request",
                    $"A new trip has been requested from {trip.PickupAddress} to {trip.DestinationAddress}."
                );
                break;

            case TripStatus.Approved:
                // Notify the rider/requester that their trip was approved
                await NotifyUser(
                    trip.RequestedByUserId,
                    tripId,
                    NotificationType.TripRequest,
                    "Trip Approved",
                    $"Your trip from {trip.PickupAddress} to {trip.DestinationAddress} has been approved."
                );
                break;

            case TripStatus.Denied:
                // Notify the rider/requester that their trip was denied
                await NotifyUser(
                    trip.RequestedByUserId,
                    tripId,
                    NotificationType.TripCancelled,
                    "Trip Denied",
                    $"Your trip from {trip.PickupAddress} to {trip.DestinationAddress} has been denied. Reason: {trip.DenialReason}"
                );
                break;

            case TripStatus.Scheduled:
                /// Notify the assigned driver that they have a new trip
                if (trip.DriverId.HasValue)
                {
                    var driver = await _context.Drivers
                        .FirstOrDefaultAsync(d => d.Id == trip.DriverId.Value);
                    if (driver != null)
                    {
                        await NotifyUser(
                            driver.UserId,
                            tripId,
                            NotificationType.TripAssigned,
                            "New Trip Assigned",
                            $"You have been assigned a trip from {trip.PickupAddress} to {trip.DestinationAddress} at {trip.ScheduledPickupTime:g}."
                        );
                    }
                }
                /// Also notify the requester that a driver was assigned
                await NotifyUser(
                    trip.RequestedByUserId,
                    tripId,
                    NotificationType.TripAssigned,
                    "Driver Assigned",
                    $"A driver has been assigned to your trip from {trip.PickupAddress} to {trip.DestinationAddress}."
                );
                break;

            case TripStatus.InProgress:
                /// Notify the requester that the driver has started the trip
                await NotifyUser(
                    trip.RequestedByUserId,
                    tripId,
                    NotificationType.TripStarted,
                    "Trip Started",
                    $"Your trip from {trip.PickupAddress} to {trip.DestinationAddress} is now in progress."
                );
                break;

            case TripStatus.Completed:
                /// Notify the requester that the trip is complete
                await NotifyUser(
                    trip.RequestedByUserId,
                    tripId,
                    NotificationType.TripCompleted,
                    "Trip Completed",
                    $"Your trip from {trip.PickupAddress} to {trip.DestinationAddress} has been completed."
                );
                break;

            case TripStatus.NoShow:
                /// Notify admins that a rider didn't show up
                await NotifyAdmins(
                    tripId,
                    "Rider No-Show",
                    $"The rider did not show up for the trip from {trip.PickupAddress} to {trip.DestinationAddress}."
                );
                break;

            case TripStatus.Cancelled:
                /// Notify the requester if someone else cancelled it
                if (changedByUserId != trip.RequestedByUserId)
                {
                    await NotifyUser(
                        trip.RequestedByUserId,
                        tripId,
                        NotificationType.TripCancelled,
                        "Trip Cancelled",
                        $"Your trip from {trip.PickupAddress} to {trip.DestinationAddress} has been cancelled."
                    );
                }
                /// If a driver was assigned, notify them too
                if (trip.DriverId.HasValue)
                {
                    var driver = await _context.Drivers
                        .FirstOrDefaultAsync(d => d.Id == trip.DriverId.Value);
                    if (driver != null && driver.UserId != changedByUserId)
                    {
                        await NotifyUser(
                            driver.UserId,
                            tripId,
                            NotificationType.TripCancelled,
                            "Trip Cancelled",
                            $"A trip you were assigned to from {trip.PickupAddress} to {trip.DestinationAddress} has been cancelled."
                        );
                    }
                }
                break;
        }
    }

    /// Helper - create a notification for a specific user
    private async Task NotifyUser(int userId, int tripId, NotificationType type, string title, string body)
    {
        var notification = new Notification
        {
            RecipientUserId = userId,
            TripId = tripId,
            Type = type,
            Channel = NotificationChannel.InApp,
            Title = title,
            Body = body,
            DeliveryStatus = DeliveryStatus.Sent,
            SentAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    /// Helper - notify all admin users
    private async Task NotifyAdmins(int tripId, string title, string body)
    {
        var admins = await _context.Users
            .Where(u => u.Role == UserRole.Admin && u.IsActive)
            .ToListAsync();

        foreach (var admin in admins)
        {
            await NotifyUser(admin.Id, tripId, NotificationType.TripRequest, title, body);
        }
    }
}
