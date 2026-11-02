using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.Services.Identity.Infrastructure.EF;

namespace WellTrack.Services.Identity.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IdentityDbContext context;

		public UserRepository(IdentityDbContext context){
			this.context = context;
		}

		public async Task<User> AddAsync(User entity)
		{
			await context.Users.AddAsync(entity);
			await context.SaveChangesAsync();
			return entity;
		}

		public async Task<int> CountAsync(Expression<Func<User, bool>>? predicate = null)
		{
			return predicate==null ? await context.Users.CountAsync(): await context.Users.CountAsync(predicate);
		}

		public async Task DeleteAsync(User entity)
		{
			context.Users.Remove(entity);
			await context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate)
		{
			return await context.Users.AnyAsync(predicate);
		}

		public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
		{
			return await context.Users.Where(predicate).ToListAsync();
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await context.Users.ToListAsync();
		}

		public async Task<User?> GetByEmailAsync(string email)
		{
			return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			return await context.Users.FindAsync(id);
		}

		public async Task UpdateAsync(User entity)
		{
			var existingUser = await context.Users.FindAsync(entity.Id);

			if (existingUser == null) return;

			existingUser.Name = entity.Name;
			existingUser.Phone = entity.Phone;
			existingUser.Role = entity.Role;          // ✅ ADDED — was missing
			existingUser.Status = entity.Status;
			existingUser.UpdatedAt = DateTime.UtcNow;

			// Only update password if it was changed (avoid overwriting on profile edits)
			if (!string.IsNullOrEmpty(entity.PasswordHash) &&
				entity.PasswordHash != existingUser.PasswordHash)
			{
				existingUser.PasswordHash = entity.PasswordHash;
			}

			await context.SaveChangesAsync();
		}
	}
}
