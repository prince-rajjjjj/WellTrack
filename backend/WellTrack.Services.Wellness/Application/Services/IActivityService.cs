using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Application.Services
{
	public interface IActivityService
	{
		Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync();
		Task<IEnumerable<ActivityDto>> GetActivitiesByProgramIdAsync(Guid programId);
		Task<ActivityDto?> GetByIdAsync(Guid id);
		Task<ActivityDto> createActivityAsync(CreateActivityDto dto);
		Task<ActivityDto> UpdateActivityAsync(Guid activityId, CreateActivityDto dto);
		Task<ActivityDto> AssignCoachAsync(Guid activityId, AssignCoachDto dto);
		Task<ActivityDto> StartActivityAsync(Guid activityId);
		Task<ActivityDto> CompleteActivityAsync(Guid activityId);
		Task<ActivityDto> CancelActivityAsync(Guid activityId);
		Task<AttendanceDto> JoinActivityAsync(Guid activityId, Guid employeeId, string employeeName);
		Task LeaveActivityAsync(Guid activityId, Guid employeeId);
		Task<AttendanceDto> MarkWatchedAsync(Guid activityId, Guid employeeId, string employeeName);

		Task<IEnumerable<AttendanceDto>> GetAttendanceAsync(Guid activityId);
		Task<AttendanceDto> MarkAttendanceAsync(Guid activityId, MarkAttendanceDto dto);
		Task<ActivityDto> FinalizeAttendanceAsync(Guid activityId, FinalizeAttendanceDto dto);


		Task<PagedResult<ActivityDto>> GetPagedActivitiesAsync(int page, int pageSize, Guid? programId = null, string? search = null, string? mode = null, string? status = null, string? view = null);
	}
}
