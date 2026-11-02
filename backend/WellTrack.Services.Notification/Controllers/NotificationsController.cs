using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Notification.Application.DTOs;
using WellTrack.Services.Notification.Application.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Notification.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService  = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetMyNotifications()
    {
        var notifications = await _notificationService.GetByUserIdAsync(_currentUserService.UserId);
        return Ok(new ApiResponse<IEnumerable<NotificationDto>>(notifications, "Your Notifications"));
    }

    [HttpGet("admin-notifications")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<NotificationDto>>(notifications, "All Notifications"));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<object>>> GetUnreadCount()
    {
        var notifications = await _notificationService.GetByUserIdAsync(_currentUserService.UserId);
        var count = notifications.Count(n => n.Status != "Inactive");
        return Ok(new ApiResponse<object>(new { count }, "Unread count"));
    }

    [HttpPost]
    [Authorize]   // any authenticated caller (including other services) may create notifications
    public async Task<ActionResult<ApiResponse<NotificationDto>>> Create([FromBody] CreateNotificationDto dto)
    {
        var notification = await _notificationService.CreateNotificationAsync(dto);
        return Ok(new ApiResponse<NotificationDto>(notification, "Notification created"));
    }

    [HttpPatch("{id}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead([FromRoute] Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new ApiResponse<object>(new { notificationId = id, isRead = true }, "Marked as read"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] Guid id)
    {
        await _notificationService.DeleteAsync(id);
        return Ok(new ApiResponse<object>("Notification deleted"));
    }

	[HttpPatch("mark-all-read")]
	public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
	{
		var userId = _currentUserService.UserId;
		var count = await _notificationService.MarkAllAsReadAsync(userId);
		return Ok(new ApiResponse<object>(new { count }, $"Marked {count} as read"));
	}
}
