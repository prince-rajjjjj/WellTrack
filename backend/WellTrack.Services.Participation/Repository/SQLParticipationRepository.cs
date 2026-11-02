using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Infrastructure.EF;

namespace WellTrack.Services.Participation.Repository
{
	public class SQLParticipationRepository : IParticipationRepository
	{
		private readonly ParticipationDbContext _context;

		public SQLParticipationRepository(ParticipationDbContext context)
		{
			_context = context;
		}

		// --- READ OPERATIONS ---

		/// <summary>
		/// GETALL: Retrieves all participation records from the database.
		/// </summary>
		public async Task<IEnumerable<ParticipationEntity>> GetAllAsync()
		{
			return await _context.Participations.ToListAsync();
		}

		/// <summary>
		/// GETBYID: Finds a specific participation record using its unique Guid ID.
		/// </summary>
		public async Task<ParticipationEntity?> GetByIdAsync(Guid id)
		{
			return await _context.Participations.FirstOrDefaultAsync(p => p.Id == id);
		}

		/// <summary>
		/// FIND: Filters participation records based on a specific logic/predicate.
		/// </summary>
		public async Task<IEnumerable<ParticipationEntity>> FindAsync(Expression<Func<ParticipationEntity, bool>> predicate)
		{
			return await _context.Participations.Where(predicate).ToListAsync();
		}

		///<summary>
		/// GETBYEMPLOYEE Retrieves all participation records for a specific employee, ordered by date descending.
		/// </summary>
		public async Task<IEnumerable<ParticipationEntity>> GetByEmployeeIdAsync(Guid employeeId)
		{
			return await _context.Participations.Where(p => p.EmployeeID == employeeId).OrderByDescending(p => p.Date).ToListAsync();
		}
		// --- WRITE OPERATIONS ---

		/// <summary>
		/// CREATEBYENTITY: Adds a new participation record and persists it to the database.
		/// </summary>
		public async Task<ParticipationEntity> AddAsync(ParticipationEntity entity)
		{
			await _context.Participations.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		/// <summary>
		/// UPDATE: Updates an existing record and automatically sets the UpdatedAt timestamp.
		/// </summary>
		public async Task UpdateAsync(ParticipationEntity entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			_context.Participations.Update(entity);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// DELETE: Removes a participation record from the database.
		/// </summary>
		public async Task DeleteAsync(ParticipationEntity entity)
		{
			_context.Participations.Remove(entity);
			await _context.SaveChangesAsync();
		}

		// --- UTILITY / METADATA ---

		/// <summary>
		/// EXISTS: Checks if any records match the given criteria.
		/// </summary>
		public async Task<bool> ExistsAsync(Expression<Func<ParticipationEntity, bool>> predicate)
		{
			return await _context.Participations.AnyAsync(predicate);
		}

		/// <summary>
		/// COUNT: Returns the total number of records, optionally filtered by a predicate.
		/// </summary>
		public Task<int> CountAsync(Expression<Func<ParticipationEntity, bool>>? predicate = null)
		{
			return predicate == null
				? _context.Participations.CountAsync()
				: _context.Participations.CountAsync(predicate);
		}


	}
}