using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Infrastructure.EF;

namespace WellTrack.Services.Compliance.Repository
{
	public class ComplianceRecordRepository : IComplianceRecordRepository
	{
		private readonly ComplianceDbContext _dbContext;

		public ComplianceRecordRepository(ComplianceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<ComplianceRecord>> GetAllAsync()
		{
			return await _dbContext.ComplianceRecords.OrderByDescending(r => r.CreatedDate).ToListAsync();
		}

		public async Task<ComplianceRecord?> GetByIdAsync(Guid id)
		{
			return await _dbContext.ComplianceRecords.FirstOrDefaultAsync(r => r.Id == id);
		}

		public async Task<ComplianceRecord> AddAsync(ComplianceRecord entity)
		{
			entity.Id = Guid.NewGuid();
			entity.CreatedAt = DateTime.UtcNow;
			entity.CreatedDate = DateTime.UtcNow;
			entity.Date = DateTime.UtcNow;

			await _dbContext.ComplianceRecords.AddAsync(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task UpdateAsync(ComplianceRecord entity)
		{
			entity.UpdatedAt = DateTime.UtcNow; // Set the metadata timestamp
			_dbContext.ComplianceRecords.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<int> CountAsync(Expression<Func<ComplianceRecord, bool>>? predicate = null)
		{
			return predicate == null
			? await _dbContext.ComplianceRecords.CountAsync()
			: await _dbContext.ComplianceRecords.CountAsync(predicate);

		}

		public Task DeleteAsync(ComplianceRecord entity)
		{
			throw new NotImplementedException();
		}

		public Task<bool> ExistsAsync(Expression<Func<ComplianceRecord, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ComplianceRecord>> FindAsync(Expression<Func<ComplianceRecord, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ComplianceRecord>> GetByProgramIdAsync(Guid programId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ComplianceRecord>> GetOpenRecordsAsync()
		{
			throw new NotImplementedException();
		}

		
	}
}
