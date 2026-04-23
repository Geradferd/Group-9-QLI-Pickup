using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

/// Handles all Notification business logic
public class NotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    /// Get paginated notifications for a specific user
    /// Only returns notifications belonging to the requesting user
    public async Task<PagedNotificationResponse> GetForUserAsync(int userId, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Notifications
            .Where(n => n.RecipientUserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();
        var unreadCount = await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && n.ReadAt == null);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedNotificationResponse
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            UnreadCount = unreadCount
        };
    }

    /// Get the unread notification count for a user
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && n.ReadAt == null);
    }

    /// Mark a single notification as read
    /// Returns false if not found or doesn't belong to the user
    public async Task<bool> MarkReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);

        if (notification == null)
            return false;

        /// Already read - no-op, still return success
        if (notification.ReadAt != null)
            return true;

        notification.ReadAt = DateTime.UtcNow;
        notification.DeliveryStatus = DeliveryStatus.Read;

        await _context.SaveChangesAsync();
        return true;
    }

    /// Mark ALL notifications as read for a user
    /// Returns the number of notifications that were marked
    public async Task<int> MarkAllReadAsync(int userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.RecipientUserId == userId && n.ReadAt == null)
            .ToListAsync();

        if (unread.Count == 0)
            return 0;

        var now = DateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.ReadAt = now;
            notification.DeliveryStatus = DeliveryStatus.Read;
        }

        await _context.SaveChangesAsync();
        return unread.Count;
    }

    /// Maps a Notification entity to the response DTO
    private NotificationResponse MapToResponse(Notification n)
    {
        return new NotificationResponse
        {
            Id = n.Id,
            RecipientUserId = n.RecipientUserId,
            TripId = n.TripId,
            Type = n.Type.ToString(),
            Channel = n.Channel.ToString(),
            Title = n.Title,
            Body = n.Body,
            DeliveryStatus = n.DeliveryStatus.ToString(),
            IsRead = n.ReadAt != null,
            SentAt = n.SentAt,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt,
            AdditionalData = n.AdditionalData
        };
    }
}
