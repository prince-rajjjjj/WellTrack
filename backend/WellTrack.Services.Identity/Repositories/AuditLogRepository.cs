using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.Services.Identity.Infrastructure.EF;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Repositories
{
	public class AuditLogRepository:IAuditLogRepository
	{
		private readonly IdentityDbContext context;

		public AuditLogRepository(IdentityDbContext context)
		{
			this.context = context;
		}

		public async Task<AuditLog> AddAsync(AuditLog entity)
		{
			await context.AuditLogs.AddAsync(entity);
			await context.SaveChangesAsync();

			return entity;
		}

		public async Task<int> CountAsync(Expression<Func<AuditLog, bool>>? predicate = null)
		{
			return predicate == null ? await context.AuditLogs.CountAsync() : await context.AuditLogs.CountAsync(predicate);
		}

		public async Task DeleteAsync(AuditLog entity)
		{
			context.AuditLogs.Remove(entity);
			await context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<AuditLog, bool>> predicate)
		{
			return await context.AuditLogs.AnyAsync(predicate);
		}

		public async Task<IEnumerable<AuditLog>> FindAsync(Expression<Func<AuditLog, bool>> predicate)
		{
			return await context.AuditLogs.Where(predicate).OrderBy(a=>a.Timestamp).ToListAsync();
		}

		public async Task<IEnumerable<AuditLog>> GetAllAsync()
		{
			return await context.AuditLogs.ToListAsync();
		}

		public async Task<AuditLog?> GetByIdAsync(Guid id)
		{
			return await context.AuditLogs.FindAsync(id);
		}

		public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
		{
			return await context.AuditLogs.Where(x => x.UserId == userId).ToListAsync();
		}

		public async Task UpdateAsync(AuditLog entity)
		{
			context.AuditLogs.Update(entity);
			await context.SaveChangesAsync();
		}

		public async Task<PagedResult<AuditLog>> GetPagedLogsAsync(int page,int pageSize,string? search)
		{
			var query = context.AuditLogs.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				var s = search.ToLower();
				query = query.Where(a =>
					a.Action.ToLower().Contains(s) ||
					a.Resource.ToLower().Contains(s) ||
					(a.Details != null && a.Details.ToLower().Contains(s)));
			}

			var totalCount = await query.CountAsync();
			var items = await query
				.OrderByDescending(a => a.Timestamp)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<AuditLog>
			{
				Items = items,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

	}
}
