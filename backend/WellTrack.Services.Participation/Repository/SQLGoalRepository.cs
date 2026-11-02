using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Infrastructure.EF;

namespace WellTrack.Services.Participation.Repository
{
	public class SQLGoalRepository : IGoalRepository
	{
		private readonly ParticipationDbContext _context;

		public SQLGoalRepository(ParticipationDbContext _dbContext)
		{
			_context = _dbContext;
		}
		//CREATE operation for Goal entity
		// ADD
		public async Task<Goal> AddAsync(Goal entity)
		{
			await _context.Goals.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;			
		}
		// READ operation for Goal entity
		//get all goals
		public async Task<IEnumerable<Goal>> GetAllAsync()
		{
			return await _context.Goals.AsNoTracking().ToListAsync();
		}

		// GET GOAL BY ID
		//get goal by id
		public Task<Goal?> GetByIdAsync(Guid id)
		{
			return _context.Goals.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		}

		// DELETE operation for Goal entity
		//delete goal
		public async Task DeleteAsync(Goal entity)
		{
			_context.Goals.Remove(entity);
			await _context.SaveChangesAsync();
		}

		// CHECK IF GOAL EXISTS
		// check if goal exists
		public async Task<bool> ExistsAsync(Expression<Func<Goal, bool>> predicate)
		{
			// 1. Ask the database: "Does ANY record match this rule (predicate)?"
			var isFound = await _context.Goals.AnyAsync(predicate);

			// 2. Return True if it exists, False if it doesn't
			return isFound;
		}

		// COUNT GOALS
		// count goals with optional filter
		public async Task<int> CountAsync(Expression<Func<Goal, bool>>? predicate = null)
		{
			// 1. Start with the whole table
			var query = _context.Goals.AsQueryable();

			// 2. If a rule (predicate) was provided, apply it
			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			// 3. Ask the database for the final count
			var totalCount = await query.CountAsync();

			return totalCount;
		}

		// UPDATE operation for Goal entity
		// update goal
		public async Task UpdateAsync(Goal entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			_context.Goals.Update(entity);
			await _context.SaveChangesAsync();
		}
		// FIND GOALS WITH FILTER
		// find goals based on a filter
		public async Task<IEnumerable<Goal>> FindAsync(Expression<Func<Goal, bool>> predicate)
		{
			return await _context.Goals.Where(predicate).ToListAsync();
		}

		//Delete goal by id and employee id

	}
}
