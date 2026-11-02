using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Application.Services;
using WellTrack.Services.Wellness.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Controllers
{
	[ApiController]
	[Route("api/v1/activities")]
	[Authorize]
	public class ActivitiesController : ControllerBase
	{
		private readonly IActivityService _activityService;
		private readonly ICurrentUserService _currentUser;
		private readonly EngagementTrigger _engagement;
		private readonly ParticipationTrigger _participation;

		public ActivitiesController(
			IActivityService activityService,
			ICurrentUserService currentUser,
			EngagementTrigger engagement,
			ParticipationTrigger participation)
		{
			_activityService = activityService;
			_currentUser     = currentUser;
			_engagement      = engagement;
			_participation   = participation;
		}

		[HttpGet]
		public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetAll(
			[FromQuery] Guid? programId = null,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 9,
			[FromQuery] string? search = null,
			[FromQuery] string? mode = null,
			[FromQuery] string? status = null,
			[FromQuery] string? view = null)
		{
			var data = await _activityService.GetPagedActivitiesAsync(page, pageSize, programId, search, mode, status, view);
			return Ok(new ApiResponse<PagedResult<ActivityDto>>(data, "Activities retrieved successfully."));
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> GetById(Guid id)
		{
			var activity = await _activityService.GetByIdAsync(id);
			if (activity == null) return NotFound(new ApiResponse<ActivityDto>(null!, "Activity not found"));
			return Ok(new ApiResponse<ActivityDto>(activity));
		}

		[HttpPost]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> Create([FromBody] CreateActivityDto dto)
		{
			var activity = await _activityService.createActivityAsync(dto);
			return Ok(new ApiResponse<ActivityDto>(activity, "Activity created successfully"));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> Update(Guid id, [FromBody] CreateActivityDto dto)
		{
			var activity = await _activityService.UpdateActivityAsync(id, dto);
			return Ok(new ApiResponse<ActivityDto>(activity, "Activity updated successfully"));
		}

		[HttpPatch("{id}/assign-coach")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> AssignCoach(Guid id, [FromBody] AssignCoachDto dto)
		{
			var activity = await _activityService.AssignCoachAsync(id, dto);
			return Ok(new ApiResponse<ActivityDto>(activity, "Coach assigned successfully"));
		}

		[HttpPost("{id}/start")]
		[Authorize(Roles = "Coach,Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> Start(Guid id)
		{
			var activity = await _activityService.StartActivityAsync(id);
			return Ok(new ApiResponse<ActivityDto>(activity, "Activity started"));
		}

		[HttpPost("{id}/complete")]
		[Authorize(Roles = "Coach,Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> MarkComplete(Guid id)
		{
			var activity = await _activityService.CompleteActivityAsync(id);
			_participation.CompleteActivity(id, Request.Headers.Authorization.ToString());
			return Ok(new ApiResponse<ActivityDto>(activity, "Activity marked as completed"));
		}

		[HttpPost("{id}/cancel")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> Cancel(Guid id)
		{
			var activity = await _activityService.CancelActivityAsync(id);
			return Ok(new ApiResponse<ActivityDto>(activity, "Activity cancelled"));
		}

		// Employee self-enrolls
		[HttpPost("{id}/join")]
		public async Task<ActionResult<ApiResponse<AttendanceDto>>> Join(Guid id)
		{
			var record   = await _activityService.JoinActivityAsync(id, _currentUser.UserId, _currentUser.Name ?? "Employee");
			var activity = await _activityService.GetByIdAsync(id);
			if (activity != null)
				_engagement.TriggerForAttendees(
					new[] { _currentUser.UserId },
					activity.ProgramID,
					Request.Headers.Authorization.ToString());
			return Ok(new ApiResponse<AttendanceDto>(record, "Joined activity"));
		}

		// Employee unenrolls (cancel their join)
		[HttpDelete("{id}/join")]
		public async Task<ActionResult<ApiResponse<object>>> Leave(Guid id)
		{
			await _activityService.LeaveActivityAsync(id, _currentUser.UserId);
			return Ok(new ApiResponse<object>(new { activityId = id }, "Left activity"));
		}

		// Employee marks themselves as watched/attended
		[HttpPost("{id}/watch")]
		public async Task<ActionResult<ApiResponse<AttendanceDto>>> MarkWatched(Guid id)
		{
			var record   = await _activityService.MarkWatchedAsync(id, _currentUser.UserId, _currentUser.Name ?? "Employee");
			var activity = await _activityService.GetByIdAsync(id);
			if (activity != null)
				_engagement.TriggerForAttendees(
					new[] { _currentUser.UserId },
					activity.ProgramID,
					Request.Headers.Authorization.ToString());
			return Ok(new ApiResponse<AttendanceDto>(record, "Marked as completed"));
		}

		[HttpGet("{id}/attendance")]
		[Authorize(Roles = "Coach,Admin,HR")]
		public async Task<ActionResult<ApiResponse<IEnumerable<AttendanceDto>>>> GetAttendance(Guid id)
		{
			var records = await _activityService.GetAttendanceAsync(id);
			return Ok(new ApiResponse<IEnumerable<AttendanceDto>>(records));
		}

		[HttpPost("{id}/attendance")]
		[Authorize(Roles = "Coach,Admin,HR")]
		public async Task<ActionResult<ApiResponse<AttendanceDto>>> MarkAttendance(Guid id, [FromBody] MarkAttendanceDto dto)
		{
			var record = await _activityService.MarkAttendanceAsync(id, dto);
			return Ok(new ApiResponse<AttendanceDto>(record, "Attendance recorded"));
		}

		// Submit the entire roster in one shot. Once finalized, no more edits.
		[HttpPost("{id}/attendance/finalize")]
		[Authorize(Roles = "Coach,Admin,HR")]
		public async Task<ActionResult<ApiResponse<ActivityDto>>> FinalizeAttendance(Guid id, [FromBody] FinalizeAttendanceDto dto)
		{
			var activity = await _activityService.FinalizeAttendanceAsync(id, dto);

			// Fire engagement recalculation for every employee marked Present.
			// Fire-and-forget — never blocks or fails the response.
			var presentIds = dto.Entries
				.Where(e => e.IsPresent)
				.Select(e => e.EmployeeID);
			_engagement.TriggerForAttendees(
				presentIds,
				activity.ProgramID,
				Request.Headers.Authorization.ToString());

			return Ok(new ApiResponse<ActivityDto>(activity, "Attendance finalized"));
		}

	}
}
