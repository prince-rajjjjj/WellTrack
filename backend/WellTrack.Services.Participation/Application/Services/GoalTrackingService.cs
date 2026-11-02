using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.Services.Participation.Application.Interfaces;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Repository;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Application.Services
{
	public class GoalTrackingService : IGoalTrackingService
	{
		private readonly IGoalRepository _goalRepository;
		private readonly IMapper _mapper;
		private readonly INotificationClient _notify;

		public GoalTrackingService(
			IGoalRepository goalRepository,
			IMapper mapper,
			INotificationClient notify)
		{
			_goalRepository = goalRepository;
			_mapper = mapper;
			_notify = notify;
		}

		// ─── Create ────────────────────────────────────────────────────────

		public async Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto)
		{
			var today = DateTime.UtcNow.Date;

			if (createGoalDto.StartDate.Date < today)
				throw new ValidationException(new Dictionary<string, string[]>
				{
					{ "startDate", new[] { "Start date cannot be in the past." } }
				});

			if (createGoalDto.EndDate.Date <= createGoalDto.StartDate.Date)
				throw new ValidationException(new Dictionary<string, string[]>
				{
					{ "endDate", new[] { "End date must be after the start date." } }
				});

			var goal = _mapper.Map<Goal>(createGoalDto);
			goal.CurrentValue = 0;
			await _goalRepository.AddAsync(goal);

			// 🔔 Notify the goal owner
			await _notify.NotifyAsync(
				createGoalDto.EmployeeID,
				$"New goal created: '{createGoalDto.Description}'",
				"Goal");

			return _mapper.Map<GoalDto>(goal);
		}

		// ─── Read ──────────────────────────────────────────────────────────

		public async Task<IEnumerable<GoalDto>> GetGoalsByEmployeeIdAsync(Guid employeeId)
		{
			var goals = await _goalRepository.FindAsync(x => x.EmployeeID == employeeId);
			return _mapper.Map<IEnumerable<GoalDto>>(goals);
		}

		// ─── Update ────────────────────────────────────────────────────────

		public async Task<GoalDto> UpdateGoalAsync(Guid id, UpdateGoalDto updateGoalDto)
		{
			var goal = await _goalRepository.GetByIdAsync(id)
				?? throw new NotFoundException(nameof(Goal), id);

			var wasComplete = goal.Status == StatusType.Completed;

			_mapper.Map(updateGoalDto, goal);

			if (goal.CurrentValue >= goal.TargetValue)
				goal.Status = StatusType.Completed;

			await _goalRepository.UpdateAsync(goal);

			// 🔔 Notify on completion (only when crossing the threshold)
			if (!wasComplete && goal.Status == StatusType.Completed)
			{
				await _notify.NotifyAsync(
					goal.EmployeeID,
					$"🎉 Goal completed: '{goal.Description}'",
					"Goal");
			}
			else
			{
				// Regular update notification
				await _notify.NotifyAsync(
					goal.EmployeeID,
					$"Goal updated: '{goal.Description}'",
					"Goal");
			}

			return _mapper.Map<GoalDto>(goal);
		}

		public async Task UpdateGoalProgressAsync(Guid goalId, decimal progressValue)
		{
			var goal = await _goalRepository.GetByIdAsync(goalId)
				?? throw new NotFoundException(nameof(Goal), goalId);

			var wasComplete = goal.Status == StatusType.Completed;

			goal.CurrentValue = progressValue;
			goal.Status = goal.CurrentValue >= goal.TargetValue
				? StatusType.Completed
				: StatusType.Active;

			await _goalRepository.UpdateAsync(goal);

			// 🔔 Notify on completion (only when crossing the threshold)
			if (!wasComplete && goal.Status == StatusType.Completed)
			{
				await _notify.NotifyAsync(
					goal.EmployeeID,
					$"🎉 Congrats! You completed your goal: '{goal.Description}'",
					"Goal");
			}
		}

		// ─── Delete ────────────────────────────────────────────────────────

		public async Task DeleteGoalAsync(Guid goalId, Guid employeeId)
		{
			var goal = await _goalRepository.GetByIdAsync(goalId)
				?? throw new NotFoundException(nameof(Goal), goalId);

			if (goal.Status == StatusType.Completed)
				throw new ValidationException(new Dictionary<string, string[]>
				{
					{ "status", new[] { "Completed goals cannot be deleted." } }
				});

			await _goalRepository.DeleteAsync(goal);

			// 🔔 Confirm deletion
			await _notify.NotifyAsync(
				employeeId,
				$"Goal deleted: '{goal.Description}'",
				"Goal");
		}

		// ─── Pagination ────────────────────────────────────────────────────

		public async Task<PagedResult<GoalDto>> GetPagedGoalsAsync(
			Guid employeeId, int page, int pageSize, string? status = null)
		{
			var all = (await GetGoalsByEmployeeIdAsync(employeeId)).ToList();

			var filtered = string.IsNullOrEmpty(status) || status == "All"
				? all
				: all.Where(g => g.Status == status).ToList();

			var total = filtered.Count;
			var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<GoalDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}