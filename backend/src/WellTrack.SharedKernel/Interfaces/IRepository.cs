using System.Linq.Expressions;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.SharedKernel.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
	// Basic CRUD operations
	Task<T?> GetByIdAsync(Guid id);
	Task<IEnumerable<T>> GetAllAsync();

	// Filtering and Searching
	Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

	// Data Modification
	Task<T> AddAsync(T entity);
	Task UpdateAsync(T entity);
	Task DeleteAsync(T entity);

	// Utility and Performance checks
	Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
	Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}