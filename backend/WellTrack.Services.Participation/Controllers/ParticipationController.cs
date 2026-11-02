using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Participation.Application.Services;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.Services.Participation.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Controllers
{
	[ApiController]
	[Route("api/v1/participation")]
	[Authorize]
	public class ParticipationController : ControllerBase
	{
		private readonly IParticipationService _services;
		private readonly ICurrentUserService _currentUserService;
		private readonly EngagementTrigger _engagement;

		public ParticipationController(
			IParticipationService services,
			ICurrentUserService currentUserService,
			EngagementTrigger engagement)
		{
			_services           = services;
			_currentUserService = currentUserService;
			_engagement         = engagement;
		}

		[HttpPost("join-activity")]
		public async Task<ActionResult<ApiResponse<object>>> JoinActivity([FromBody] JoinActivityRequest req)
		{
			var employeeId      = _currentUserService.UserId;
			var participationId = await _services.JoinActivityAsync(employeeId, req);

			// Trigger engagement recalc — joining an activity boosts activity + consistency scores
			_engagement.Trigger(
				employeeId,
				req.ProgramId,
				Request.Headers.Authorization.ToString());

			return Ok(new ApiResponse<object>(new { participationId }, "Activity joined successfully"));
		}

		[HttpGet]
		public async Task<ActionResult<ApiResponse<IEnumerable<ParticipationDto>>>> GetAll([FromQuery] Guid? employeeId = null)
		{
			IEnumerable<ParticipationDto> participations;

			if (employeeId.HasValue)
				participations = await _services.GetByEmployeeIdAsync(employeeId.Value);
			else
				participations = await _services.GetAllAsync();

			return Ok(new ApiResponse<IEnumerable<ParticipationDto>>(participations));
		}

		[HttpGet("my")]
		public async Task<ActionResult<ApiResponse<PagedResult<ParticipationDto>>>> GetMine(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 9,
			[FromQuery] string? status = null)
		{
			var data = await _services.GetPagedMineAsync(_currentUserService.UserId, page, pageSize, status);
			return Ok(new ApiResponse<PagedResult<ParticipationDto>>(data));
		}

		[HttpPost("complete-by-activity/{activityId}")]
		public async Task<ActionResult<ApiResponse<object>>> CompleteByActivity(Guid activityId)
		{
			await _services.CompleteByActivityAsync(activityId);
			return Ok(new ApiResponse<object>(null, "Participation records completed"));
		}

		[HttpPost]
		public async Task<ActionResult<ApiResponse<ParticipationDto>>> Create([FromBody] CreateParticipationDto createParticipationDto)
		{
			var participation = await _services.CreateParticipationAsync(createParticipationDto);
			return CreatedAtAction(nameof(GetAll), new ApiResponse<ParticipationDto>(participation, "Participation recorded successfully"));
		}
	}
}
