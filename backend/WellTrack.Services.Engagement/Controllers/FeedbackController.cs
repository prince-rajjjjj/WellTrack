using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Application.Services;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Engagement.Controllers
{
	[ApiController]
	[Route("api/v1/feedback")]
	[Authorize]
	public class FeedbackController : ControllerBase
	{
		private readonly IFeedbackService _feedbackService;
		private readonly IEngagementService _engagementService;
		private readonly ICurrentUserService _currentUserService;

		public FeedbackController(
			IFeedbackService feedbackService,
			IEngagementService engagementService,
			ICurrentUserService currentUserService)
		{
			_feedbackService    = feedbackService;
			_engagementService  = engagementService;
			_currentUserService = currentUserService;
		}

		// ════════════════════════════════════════════════════════════
		//  HR / Admin
		// ════════════════════════════════════════════════════════════

		/// <summary>All feedback across every employee and program.</summary>
		[HttpGet]
		[Authorize(Roles = "HR,Admin")]
		public async Task<ActionResult<ApiResponse<PagedResult<FeedbackDto>>>> GetAll(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 20)
		{
			var result = await _feedbackService.GetPagedAllAsync(page, pageSize);
			return Ok(new ApiResponse<PagedResult<FeedbackDto>>(result));
		}

		/// <summary>Most recent N feedback entries — useful for HR dashboard feeds.</summary>
		[HttpGet("recent")]
		[Authorize(Roles = "HR,Admin")]
		public async Task<ActionResult<ApiResponse<IEnumerable<FeedbackDto>>>> GetRecent(
			[FromQuery] int count = 10)
		{
			var result = await _feedbackService.GetRecentAsync(count);
			return Ok(new ApiResponse<IEnumerable<FeedbackDto>>(result));
		}

		/// <summary>All feedback submitted for a specific program.</summary>
		[HttpGet("program/{programId:guid}")]
		[Authorize(Roles = "HR,Admin,Coach")]
		public async Task<ActionResult<ApiResponse<IEnumerable<FeedbackDto>>>> GetByProgram(Guid programId)
		{
			var result = await _feedbackService.GetByProgramIdAsync(programId);
			return Ok(new ApiResponse<IEnumerable<FeedbackDto>>(result));
		}

		/// <summary>Star-distribution stats + average rating for a program.</summary>
		[HttpGet("program/{programId:guid}/stats")]
		[Authorize(Roles = "HR,Admin,Coach")]
		public async Task<ActionResult<ApiResponse<FeedbackStatsDto>>> GetStats(Guid programId)
		{
			var stats = await _feedbackService.GetStatsByProgramAsync(programId);
			return Ok(new ApiResponse<FeedbackStatsDto>(stats));
		}

		// ════════════════════════════════════════════════════════════
		//  Employee
		// ════════════════════════════════════════════════════════════

		/// <summary>The logged-in employee's own feedback history.</summary>
		[HttpGet("my")]
		public async Task<ActionResult<ApiResponse<PagedResult<FeedbackDto>>>> GetMine(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 6)
		{
			var result = await _feedbackService.GetPagedMineAsync(_currentUserService.UserId, page, pageSize);
			return Ok(new ApiResponse<PagedResult<FeedbackDto>>(result, "Feedback retrieved."));
		}

		/// <summary>Get a single feedback entry by ID.</summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ApiResponse<FeedbackDto>>> GetById(Guid id)
		{
			var result = await _feedbackService.GetByIdAsync(id)
				?? throw new NotFoundException("Feedback", id);
			return Ok(new ApiResponse<FeedbackDto>(result));
		}

		/// <summary>
		/// Submit feedback for a program.
		/// One submission per employee per program is enforced.
		/// Automatically refreshes the employee's FeedbackScore in their engagement record.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ApiResponse<FeedbackDto>>> Create([FromBody] CreateFeedbackDto dto)
		{
			var result = await _feedbackService.CreateFeedbackAsync(dto, _currentUserService.UserId);

			// Automatically update the FeedbackScore factor on the existing engagement record
			await _engagementService.UpdateFeedbackScoreAsync(_currentUserService.UserId, dto.ProgramId);

			return Ok(new ApiResponse<FeedbackDto>(result, "Feedback submitted successfully."));
		}

		/// <summary>Update rating or comments on your own feedback.</summary>
		[HttpPut("{id:guid}")]
		public async Task<ActionResult<ApiResponse<FeedbackDto>>> Update(
			Guid id, [FromBody] UpdateFeedbackDto dto)
		{
			var result = await _feedbackService.UpdateFeedbackAsync(id, _currentUserService.UserId, dto);
			return Ok(new ApiResponse<FeedbackDto>(result, "Feedback updated successfully."));
		}

		/// <summary>
		/// Delete your own feedback.
		/// Automatically zeroes out the FeedbackScore on the engagement record.
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
		{
			// Fetch the feedback first so we know the ProgramId for the score update
			var feedback = await _feedbackService.GetByIdAsync(id)
				?? throw new NotFoundException("Feedback", id);

			await _feedbackService.DeleteFeedbackAsync(id, _currentUserService.UserId);

			// Zero out FeedbackScore since the feedback no longer exists
			await _engagementService.UpdateFeedbackScoreAsync(_currentUserService.UserId, feedback.ProgramID);

			return Ok(new ApiResponse<object>(null, "Feedback deleted successfully."));
		}
	}
}
