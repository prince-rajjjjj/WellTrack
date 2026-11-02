using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.Services.Wellness.Infrastructure.EF;

namespace WellTrack.Services.Wellness.Repositories
{
	public class ActivityRepository : IActivityRepository
	{
		private readonly ProgramDbContext _dbContext;
		public ActivityRepository(ProgramDbContext dbContext) { _dbContext = dbContext; }

		public async Task<Activity> AddAsync(Activity entity)
		{
			await _dbContext.Activities.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

		public async Task<int> CountAsync(Expression<Func<Activity, bool>>? predicate = null) =>
			predicate == null
				? await _dbContext.Activities.CountAsync()
				: await _dbContext.Activities.CountAsync(predicate);

		public async Task DeleteAsync(Activity entity)
		{
			_dbContext.Activities.Remove(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<Activity, bool>> predicate) =>
			await _dbContext.Activities.AnyAsync(predicate);

		public async Task<IEnumerable<Activity>> FindAsync(Expression<Func<Activity, bool>> predicate) =>
			await _dbContext.Activities.Include(p => p.Program).Where(predicate).ToListAsync();

		public async Task<IEnumerable<Activity>> GetAllAsync() =>
			await _dbContext.Activities.Include(a => a.Program).ToListAsync();

		public async Task<Activity?> GetByIdAsync(Guid id) =>
			await _dbContext.Activities.Include(a => a.Program).FirstOrDefaultAsync(a => a.Id == id);

		public async Task<IEnumerable<Activity>> GetByProgramIdAsync(Guid programId) =>
			await _dbContext.Activities
				.Include(a => a.Program)
				.Where(a => a.ProgramID == programId)
				.OrderBy(a => a.Date)
				.ToListAsync();

		public async Task UpdateAsync(Activity entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			_dbContext.Activities.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		// --- Attendance ---
		public async Task<IEnumerable<ActivityAttendance>> GetAttendanceByActivityIdAsync(Guid activityId) =>
			await _dbContext.ActivityAttendances.Where(a => a.ActivityID == activityId).ToListAsync();

		public async Task<ActivityAttendance?> GetAttendanceRecordAsync(Guid activityId, Guid employeeId) =>
			await _dbContext.ActivityAttendances.FirstOrDefaultAsync(a => a.ActivityID == activityId && a.EmployeeID == employeeId);

		public async Task AddAttendanceAsync(ActivityAttendance record)
		{
			await _dbContext.ActivityAttendances.AddAsync(record);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateAttendanceAsync(ActivityAttendance record)
		{
			record.UpdatedAt = DateTime.UtcNow;
			_dbContext.ActivityAttendances.Update(record);
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteAttendanceAsync(ActivityAttendance record)
		{
			_dbContext.ActivityAttendances.Remove(record);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<int> CountAttendanceAsync(Expression<Func<ActivityAttendance, bool>> predicate) =>
			await _dbContext.ActivityAttendances.CountAsync(predicate);
	}
}
