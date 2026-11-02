using WellTrack.Services.Notification.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Notification.Repositories;

public interface INotificationRepository : IRepository<NotificationEntity>
{

	Task<int> MarkAllAsReadAsync(Guid userId);
}
