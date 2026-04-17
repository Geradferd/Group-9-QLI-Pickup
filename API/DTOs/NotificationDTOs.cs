using Api.Models;

namespace Api.DTOs;

/// Response DTO - returned to the client for each notification
public class NotificationResponse
{
    public int Id { get; set; }
    public int RecipientUserId { get; set; }
    public int? TripId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AdditionalData { get; set; }
}

/// Paginated list response wrapper
public class PagedNotificationResponse
{
    public List<NotificationResponse> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int UnreadCount { get; set; }
}

/// Request DTO for marking a single notification as read
public class MarkReadRequest
{
    public int NotificationId { get; set; }
}
