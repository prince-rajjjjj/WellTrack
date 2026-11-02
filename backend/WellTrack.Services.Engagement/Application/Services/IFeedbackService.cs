using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Engagement.Application.Services
{
	public interface IFeedbackService
	{
		Task<IEnumerable<FeedbackDto>> GetAllAsync();
		Task<IEnumerable<FeedbackDto>> GetByProgramIdAsync(Guid programId);
		Task<IEnumerable<FeedbackDto>> GetByEmployeeIdAsync(Guid employeeId);
		Task<FeedbackDto?> GetByIdAsync(Guid id);
		Task<IEnumerable<FeedbackDto>> GetRecentAsync(int count = 10);

		// --- Commands ---
		Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto dto, Guid employeeId);
		Task<FeedbackDto> UpdateFeedbackAsync(Guid id, Guid employeeId, UpdateFeedbackDto dto);
		Task DeleteFeedbackAsync(Guid id, Guid employeeId);

		// --- Analytics ---
		Task<FeedbackStatsDto> GetStatsByProgramAsync(Guid programId);

		// --- Paged ---
		Task<PagedResult<FeedbackDto>> GetPagedMineAsync(Guid employeeId, int page, int pageSize);
		Task<PagedResult<FeedbackDto>> GetPagedAllAsync(int page, int pageSize);
	}
}
