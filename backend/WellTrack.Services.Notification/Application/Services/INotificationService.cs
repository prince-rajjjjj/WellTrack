using WellTrack.Services.Notification.Application.DTOs;

namespace WellTrack.Services.Notification.Application.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetAllAsync();
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto);
    Task MarkAsReadAsync(Guid id);
    Task DeleteAsync(Guid id);
	Task<int> MarkAllAsReadAsync(Guid userId);
}
