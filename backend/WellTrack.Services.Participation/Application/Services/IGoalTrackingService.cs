using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Application.Interfaces
{
	public interface IGoalTrackingService
	{
		//CREATE
		//>??
		Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto);
		//READ GET BY EMPLOYEE ID
		//"""
		Task<IEnumerable<GoalDto>> GetGoalsByEmployeeIdAsync(Guid employeeId);
		// EDIT GOAL
		//EDIt
		Task<GoalDto> UpdateGoalAsync(Guid id, UpdateGoalDto updateGoalDto);

		// PATCH GOAL PROGRESS
		// single field update
		Task UpdateGoalProgressAsync(Guid goalId, decimal progressValue);

		// DELETE GOAL
		// delete
		Task DeleteGoalAsync(Guid goalId, Guid employeeId);

		Task<PagedResult<GoalDto>> GetPagedGoalsAsync(Guid employeeId, int page, int pageSize, string? status = null);
	}
}
