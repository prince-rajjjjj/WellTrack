using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Participation.Application.Services;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;
using WellTrack.Services.Participation.Application.Interfaces;

namespace WellTrack.Services.Participation.Controllers
{
	[ApiController]
	[Route("api/v1/goals")]
	[Authorize]
	public class GoalsController : ControllerBase
	{
		private readonly IGoalTrackingService _goalService;
		private readonly ICurrentUserService _currentUserService;

		public GoalsController(IGoalTrackingService goalTrackingService, ICurrentUserService currentUserService)
		{
			_goalService        = goalTrackingService;
			_currentUserService = currentUserService;
		}

		[HttpPost]
		public async Task<ActionResult<ApiResponse<GoalDto>>> Create([FromBody] CreateGoalDto createGoalDto)
		{
			var goal = await _goalService.CreateGoalAsync(createGoalDto);
			return Ok(new ApiResponse<GoalDto>(goal, "Goal created successfully"));
		}

		[HttpGet]
		public async Task<ActionResult<ApiResponse<PagedResult<GoalDto>>>> GetGoalsByEmpId(
			[FromQuery] Guid? employeeId,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 9,
			[FromQuery] string? status = null)
		{
			// If an explicit employeeId is supplied (e.g. by the Engagement service calling
			// on behalf of a specific employee), use it. Otherwise fall back to the logged-in user.
			var targetId = employeeId ?? _currentUserService.UserId;
			var data = await _goalService.GetPagedGoalsAsync(targetId, page, pageSize, status);
			return Ok(new ApiResponse<PagedResult<GoalDto>>(data));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ApiResponse<GoalDto>>> Update(Guid id, [FromBody] UpdateGoalDto dto)
		{
			var goal = await _goalService.UpdateGoalAsync(id, dto);
			return Ok(new ApiResponse<GoalDto>(goal, "Goal updated successfully"));
		}

		[HttpPatch("{id}/progress")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateProgress(Guid id, [FromBody] UpdateProgressRequest request)
		{
			await _goalService.UpdateGoalProgressAsync(id, request.CurrentValue);
			var goals   = await _goalService.GetGoalsByEmployeeIdAsync(_currentUserService.UserId);
			var updated = goals.FirstOrDefault(g => g.Id == id);
			return Ok(new ApiResponse<object>(updated ?? (object)new { id }, "Progress updated successfully"));
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
		{
			await _goalService.DeleteGoalAsync(id, _currentUserService.UserId);
			return Ok(new ApiResponse<object>(null, "Goal deleted successfully"));
		}
	}

	public class UpdateProgressRequest
	{
		public decimal CurrentValue { get; set; }
	}
}
