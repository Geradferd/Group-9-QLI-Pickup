using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Services;

namespace Api.Controllers;

/// Notifications API Controller
/// All endpoints require authentication - users can only access their own notifications
/// GET    /api/notifications                   - Paginated list of user's notifications
/// GET    /api/notifications/unread-count      - Unread notification count
/// POST   /api/notifications/{id}/mark-read   - Mark a single notification as read
/// POST   /api/notifications/mark-all-read    - Mark all notifications as read

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // Helper: get the logged-in user's ID from their JWT token
    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    /// GET /api/notifications?page=1&pageSize=20
    /// Returns paginated list of the authenticated user's notifications, newest first
    /// Also includes the total unread count in the response envelope
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.GetForUserAsync(userId, page, pageSize);
        return Ok(result);
    }

    /// GET /api/notifications/unread-count
    /// Returns just the unread count - lightweight endpoint for badge/indicator polling
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { unreadCount = count });
    }

    /// POST /api/notifications/{id}/mark-read
    /// Marks a single notification as read
    /// Returns 404 if the notification doesn't exist or doesn't belong to the user
    [HttpPost("{id}/mark-read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _notificationService.MarkReadAsync(id, userId);

        if (!success)
            return NotFound(new { message = "Notification not found" });

        return Ok(new { message = "Notification marked as read" });
    }

    /// POST /api/notifications/mark-all-read
    /// Marks ALL of the authenticated user's unread notifications as read
    /// Returns the count of notifications that were updated
    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = GetCurrentUserId();
        var updatedCount = await _notificationService.MarkAllReadAsync(userId);
        return Ok(new { message = "All notifications marked as read", updatedCount });
    }
}
