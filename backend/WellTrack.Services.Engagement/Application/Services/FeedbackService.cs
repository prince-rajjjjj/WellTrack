using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.Services.Engagement.Repositories;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Engagement.Application.Services
{
	public class FeedbackService : IFeedbackService
	{
		private readonly IFeedbackRepository _repository;
		private readonly IMapper _mapper;
		private readonly INotificationClient _notify;
		private readonly IIdentityClient _identityClient;

		public FeedbackService(
			IFeedbackRepository repository,
			IMapper mapper,
			INotificationClient notify,
			IIdentityClient identityClient)
		{
			_repository = repository;
			_mapper = mapper;
			_notify = notify;
			_identityClient = identityClient;
		}

		// ─── Create ────────────────────────────────────────────────────────

		public async Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto dto, Guid employeeId)
		{
			// Activity-level uniqueness (preferred): one feedback per employee per activity.
			// Program-level fallback: one per employee per program.
			if (dto.ActivityId.HasValue)
			{
				if (await _repository.ExistsAsync(f =>
						f.EmployeeID == employeeId && f.ActivityID == dto.ActivityId))
					throw new System.ComponentModel.DataAnnotations.ValidationException(
						"Feedback already submitted for this activity.");
			}
			else
			{
				if (await _repository.ExistsAsync(f =>
						f.EmployeeID == employeeId &&
						f.ProgramID == dto.ProgramId &&
						f.ActivityID == null))
					throw new System.ComponentModel.DataAnnotations.ValidationException(
						"Feedback already submitted for this program.");
			}

			var feedback = _mapper.Map<Feedback>(dto);
			feedback.Id = Guid.NewGuid();
			feedback.EmployeeID = employeeId;

			await _repository.AddAsync(feedback);

			// 🔔 Confirm to the employee
			await _notify.NotifyAsync(
				employeeId,
				$"Thanks for your feedback! Rating: {feedback.Rating}/5",
				"Engagement",
				feedback.ProgramID);

			// 🔔 Notify all HR users about the new feedback
			var hrIds = await _identityClient.GetUserIdsByRoleAsync("HR");
			if (hrIds.Any())
			{
				await _notify.NotifyManyAsync(
					hrIds,
					$"New feedback received ({feedback.Rating}/5)",
					"Engagement",
					feedback.ProgramID);
			}

			return _mapper.Map<FeedbackDto>(feedback);
		}

		// ─── Read ──────────────────────────────────────────────────────────

		public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
		{
			var items = await _repository.GetAllAsync();
			return _mapper.Map<IEnumerable<FeedbackDto>>(items.OrderByDescending(f => f.Date));
		}

		public async Task<IEnumerable<FeedbackDto>> GetByEmployeeIdAsync(Guid employeeId)
		{
			var items = await _repository.FindAsync(f => f.EmployeeID == employeeId);
			return _mapper.Map<IEnumerable<FeedbackDto>>(items.OrderByDescending(f => f.Date));
		}

		public async Task<FeedbackDto?> GetByIdAsync(Guid id)
		{
			var item = await _repository.GetByIdAsync(id);
			return _mapper.Map<FeedbackDto>(item);
		}

		public async Task<IEnumerable<FeedbackDto>> GetByProgramIdAsync(Guid programId)
		{
			var items = await _repository.FindAsync(f => f.ProgramID == programId);
			return _mapper.Map<IEnumerable<FeedbackDto>>(items.OrderByDescending(f => f.Date));
		}

		public async Task<IEnumerable<FeedbackDto>> GetRecentAsync(int count = 10)
		{
			var items = await _repository.GetAllAsync();
			return _mapper.Map<IEnumerable<FeedbackDto>>(items.OrderByDescending(f => f.Date).Take(count));
		}

		public async Task<FeedbackStatsDto> GetStatsByProgramAsync(Guid programId)
		{
			var feedbacks = (await _repository.FindAsync(f => f.ProgramID == programId)).ToList();

			return new FeedbackStatsDto
			{
				ProgramID = programId,
				TotalFeedbacks = feedbacks.Count,
				AverageRating = feedbacks.Any() ? Math.Round(feedbacks.Average(f => f.Rating), 2) : 0,
				FiveStarCount = feedbacks.Count(f => f.Rating == 5),
				FourStarCount = feedbacks.Count(f => f.Rating == 4),
				ThreeStarCount = feedbacks.Count(f => f.Rating == 3),
				TwoStarCount = feedbacks.Count(f => f.Rating == 2),
				OneStarCount = feedbacks.Count(f => f.Rating == 1)
			};
		}

		// ─── Update ────────────────────────────────────────────────────────

		public async Task<FeedbackDto> UpdateFeedbackAsync(Guid id, Guid employeeId, UpdateFeedbackDto dto)
		{
			var feedback = await _repository.GetByIdAsync(id)
				?? throw new NotFoundException("Feedback", id);

			if (feedback.EmployeeID != employeeId)
				throw new System.ComponentModel.DataAnnotations.ValidationException(
					"You can only edit your own feedback.");

			_mapper.Map(dto, feedback);
			await _repository.UpdateAsync(feedback);

			return _mapper.Map<FeedbackDto>(feedback);
		}

		// ─── Delete ────────────────────────────────────────────────────────

		public async Task DeleteFeedbackAsync(Guid id, Guid employeeId)
		{
			var feedback = await _repository.GetByIdAsync(id)
				?? throw new NotFoundException("Feedback", id);

			if (feedback.EmployeeID != employeeId)
				throw new System.ComponentModel.DataAnnotations.ValidationException("Unauthorized");

			await _repository.DeleteAsync(feedback);
		}

		// ─── Pagination ────────────────────────────────────────────────────

		public async Task<PagedResult<FeedbackDto>> GetPagedMineAsync(Guid employeeId, int page, int pageSize)
		{
			var all = (await GetByEmployeeIdAsync(employeeId)).ToList();
			var total = all.Count;
			var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
			return new PagedResult<FeedbackDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}

		public async Task<PagedResult<FeedbackDto>> GetPagedAllAsync(int page, int pageSize)
		{
			var all = (await GetAllAsync()).ToList();
			var total = all.Count;
			var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
			return new PagedResult<FeedbackDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}