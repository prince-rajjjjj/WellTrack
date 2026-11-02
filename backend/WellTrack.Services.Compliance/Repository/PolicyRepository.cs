using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Infrastructure.EF;

namespace WellTrack.Services.Compliance.Repository
{
	public class PolicyRepository : IPolicyRepository
	{
		private readonly ComplianceDbContext _dbContext;

		public PolicyRepository(ComplianceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Policy>> GetAllAsync()
		{
			return await _dbContext.Policies.OrderByDescending(p => p.CreatedDate).ToListAsync();
		}

		public async Task<Policy?> GetByIdAsync(Guid id)
		{
			return await _dbContext.Policies.FirstOrDefaultAsync(p => p.Id == id);
			
		}

		public async Task<Policy> AddAsync(Policy entity)
		{
			await _dbContext.Policies.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

		public async Task UpdateAsync(Policy entity)
		{
			_dbContext.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<int> CountAsync(Expression<Func<Policy, bool>>? predicate = null)
		{
			return predicate == null
						? await _dbContext.Policies.CountAsync()
						: await _dbContext.Policies.CountAsync(predicate);
		}

		public async Task DeleteAsync(Policy entity)
		{
			_dbContext.Policies.Remove(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<Policy, bool>> predicate)
		{
			return await _dbContext.Policies.AnyAsync(predicate);
		}

		public async Task<IEnumerable<Policy>> FindAsync(Expression<Func<Policy, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Policy>> GetActivePoliciesAsync()
		{
			return await _dbContext.Policies.Where(p => p.Status == PolicyStatus.Active).ToListAsync();
		}

	}
}
