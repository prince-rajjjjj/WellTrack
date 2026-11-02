using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Wellness.Repositories
{
	public interface IActivityRepository : IRepository<Activity>
	{
		Task<IEnumerable<Activity>> GetByProgramIdAsync(Guid programId);

		// Attendance
		Task<IEnumerable<ActivityAttendance>> GetAttendanceByActivityIdAsync(Guid activityId);
		Task<ActivityAttendance?> GetAttendanceRecordAsync(Guid activityId, Guid employeeId);
		Task AddAttendanceAsync(ActivityAttendance record);
		Task UpdateAttendanceAsync(ActivityAttendance record);
		Task DeleteAttendanceAsync(ActivityAttendance record);
		Task<int> CountAttendanceAsync(Expression<Func<ActivityAttendance, bool>> predicate);

	}
}
