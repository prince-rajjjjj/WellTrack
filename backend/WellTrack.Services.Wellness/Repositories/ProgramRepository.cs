using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.Services.Wellness.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Enum;
using WellTrack.Services.Wellness.Application.DTOs;

namespace WellTrack.Services.Wellness.Repositories
{
	public class ProgramRepository : IProgramRepository
	{
		private readonly ProgramDbContext _dbContext;

		public ProgramRepository(ProgramDbContext dbContext)
		{
			this._dbContext = dbContext;
			

		}

		public async Task<programs> AddAsync(programs entity)
		{
			await _dbContext.Programs.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

		public async Task<int> CountAsync(Expression<Func<programs, bool>>? predicate = null)
		{
			if (predicate == null)
			{
				return await _dbContext.Programs.CountAsync();
			}
			else
			{
				return await _dbContext.Programs.CountAsync(predicate);
			}
		}
		public Task DeleteAsync(programs entity)
		{
			_dbContext.Programs.Remove(entity);
			return _dbContext.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<programs, bool>> predicate)
		{
			return await _dbContext.Programs.AnyAsync(predicate);
		}

		public async Task<IEnumerable<programs>> FindAsync(Expression<Func<programs, bool>> predicate)
		{
			return await _dbContext.Programs
				.Include(p => p.Activities)
				.Where(predicate)
				.ToListAsync();

		}

		public async Task<IEnumerable<programs>> GetActivePrograms()
		{
			return await _dbContext.Programs
				.Include(p => p.Activities)
				.Where(p => p.Status == SharedKernel.Enum.StatusType.Active)
				.ToListAsync();
		}

		public async Task<IEnumerable<programs>> GetAllAsync()
		{
			return await _dbContext.Programs.ToListAsync();
		}

		

		public async Task<programs?> GetByIdAsync(Guid id)
		{
			return await _dbContext.Programs.FirstOrDefaultAsync(p => p.Id == id);
		}

		

		public async Task UpdateAsync(programs entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			_dbContext.Programs.Update(entity);
			await _dbContext.SaveChangesAsync();
		}
	}
}













