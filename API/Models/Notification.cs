using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

// Notification types for the system
public enum NotificationType
{
    TripRequest,        // New trip has been requested
    TripAssigned,       // Trip assigned to a driver
    TripStarted,        // Driver started the trip
    TripCompleted,      // Trip was completed
    TripCancelled,      // Trip was cancelled
    DriverArriving,     // Driver is approaching pickup
    DriverArrived,      // Driver has arrived at pickup
    SystemAlert,        // General system notifications
    Message             // General message notifications
}

// Notification delivery channels
public enum NotificationChannel
{
    InApp,              // In-app notification
    Push,               // Push notification to mobile device
    Email,              // Email notification
    SMS                 // SMS text message
}

// Delivery status tracking
public enum DeliveryStatus
{
    Pending,            // Queued but not sent yet
    Sent,               // Successfully sent
    Delivered,          // Confirmed delivered (e.g., push notification delivered)
    Failed,             // Failed to send
    Read                // User has read the notification
}

// Notification Table in DB
// Tracks what was sent to whom and whether it was delivered/read
[Index(nameof(RecipientUserId), nameof(CreatedAt))]
[Index(nameof(DeliveryStatus))]
public class Notification
{
    public int Id { get; set; }

    // Recipient - required
    [Required]
    public int RecipientUserId { get; set; }
    public User RecipientUser { get; set; } = null!; // Navigation property

    // Related trip - nullable (not all notifications are trip-related)
    public int? TripId { get; set; }
    // Uncomment when Trip model is created:
    // public Trip? Trip { get; set; }

    // Notification metadata
    [Required]
    public NotificationType Type { get; set; }

    [Required]
    public NotificationChannel Channel { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Body { get; set; } = string.Empty;

    // Delivery tracking
    [Required]
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

    // Timestamps
    public DateTime? SentAt { get; set; } // When notification was sent

    public DateTime? ReadAt { get; set; } // When user read the notification

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When notification was created

    // Optional: Store any additional data as JSON
    [MaxLength(2000)]
    public string? AdditionalData { get; set; } // JSON string for extra context
}
