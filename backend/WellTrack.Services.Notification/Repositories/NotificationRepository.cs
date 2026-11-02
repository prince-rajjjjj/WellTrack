using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Notification.Domain.Entities;
using WellTrack.Services.Notification.Infrastructure.EF;
using WellTrack.Services.Notification.Repositories;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Notification.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationEntity> AddAsync(NotificationEntity entity)
    {
        await _context.Notifications.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> CountAsync(Expression<Func<NotificationEntity, bool>>? predicate = null)
        => predicate == null
            ? await _context.Notifications.CountAsync()
            : await _context.Notifications.CountAsync(predicate);

    public async Task DeleteAsync(NotificationEntity entity)
    {
        _context.Notifications.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<NotificationEntity, bool>> predicate)
        => await _context.Notifications.AnyAsync(predicate);

    public async Task<IEnumerable<NotificationEntity>> FindAsync(Expression<Func<NotificationEntity, bool>> predicate)
        => await _context.Notifications.Where(predicate).ToListAsync();

    public async Task<IEnumerable<NotificationEntity>> GetAllAsync()
        => await _context.Notifications.ToListAsync();

    public async Task<NotificationEntity?> GetByIdAsync(Guid id)
        => await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);

    public async Task UpdateAsync(NotificationEntity entity)
    {
        _context.Notifications.Update(entity);
        await _context.SaveChangesAsync();
    }

	public async Task<int> MarkAllAsReadAsync(Guid userId)
	{
		var unread = await _context.Notifications
			.Where(n => n.UserID == userId && n.Status == StatusType.Active)
			.ToListAsync();

		foreach (var n in unread)
		{
			n.Status = StatusType.Inactive;
			n.UpdatedAt = DateTime.UtcNow;
		}

		await _context.SaveChangesAsync();
		return unread.Count;
	}
}
