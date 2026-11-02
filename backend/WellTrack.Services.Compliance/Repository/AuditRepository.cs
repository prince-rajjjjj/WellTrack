using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Infrastructure.EF;
using WellTrack.Services.Compliance.Repositories;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Compliance.Repository
{
	public class AuditRepository : IAuditRepository
	{
		private readonly ComplianceDbContext _dbContext;

		public AuditRepository(ComplianceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Audit>> GetAllAsync()
		{
			return await _dbContext.Audits.OrderByDescending(a => a.Date).ToListAsync();
		}

		public async Task<Audit?> GetByIdAsync(Guid id)
		{
			return await _dbContext.Audits.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<Audit> AddAsync(Audit entity) 
		{
			// it was created in mapping automatically 
			//entity.Id = Guid.NewGuid();
			//entity.CreatedAt = DateTime.UtcNow;
			//entity.Date = DateTime.UtcNow;
			await _dbContext.Audits.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

		public async Task UpdateAsync(Audit entity)
		{
			_dbContext.Audits.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(Audit entity)
		{
			_dbContext.Audits.Remove(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<int> CountAsync(Expression<Func<Audit, bool>>? predicate = null)
		{
			return predicate == null
			? await _dbContext.Audits.CountAsync()
			: await _dbContext.Audits.CountAsync(predicate);
		}

		public async Task<bool> ExistsAsync(Expression<Func<Audit, bool>> predicate)
		{
			return await _dbContext.Audits.AnyAsync(predicate);
		}

		public async Task<IEnumerable<Audit>> FindAsync(Expression<Func<Audit, bool>> predicate)
		{
			return await _dbContext.Audits.Where(predicate).ToListAsync();
		}

		public async Task<IEnumerable<Audit>> GetOpenAuditsAsync()
		{
			return await _dbContext.Audits.Where(a => a.Status == StatusType.Pending || a.Status == StatusType.Active).ToListAsync();
		}

	}
}
