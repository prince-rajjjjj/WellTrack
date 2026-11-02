using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.Services.Engagement.Infrastructure.EF;

namespace WellTrack.Services.Engagement.Repositories
{
	public class EngagementRepository : IEngagementRepository
	{
		private readonly EngagementDbContext _context;

		public EngagementRepository(EngagementDbContext context)
		{
			_context = context;
		}

		public async Task<EngagementEntity> AddAsync(EngagementEntity entity)
		{
			await _context.Engagements.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public  async Task<int> CountAsync(Expression<Func<EngagementEntity, bool>>? predicate = null)
		{

			if (predicate == null)
			{
				return await _context.Engagements.CountAsync();
			}
			else
			{
				return await _context.Engagements.CountAsync(predicate);
			}
		}

		public async Task DeleteAsync(EngagementEntity entity)
		{
			_context.Engagements.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<EngagementEntity, bool>> predicate)
		{
			return await _context.Engagements.AnyAsync(predicate);
		}

		public async Task<IEnumerable<EngagementEntity>> FindAsync(Expression<Func<EngagementEntity, bool>> predicate)
		{
			return await _context.Engagements.Where(predicate).ToListAsync(); ;
		}

		public async Task<IEnumerable<EngagementEntity>> GetAllAsync()
		{
			return await _context.Engagements.ToListAsync();
		}

		public async Task<EngagementEntity?> GetByIdAsync(Guid id)
		{
			return await _context.Engagements.FindAsync(id);
		}

		public async Task UpdateAsync(EngagementEntity entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}
	}
}
