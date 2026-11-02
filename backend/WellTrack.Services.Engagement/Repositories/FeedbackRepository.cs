using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.Services.Engagement.Infrastructure.EF;

namespace WellTrack.Services.Engagement.Repositories
{
	public class FeedbackRepository : IFeedbackRepository
	{
		private readonly EngagementDbContext _context;

		public FeedbackRepository(EngagementDbContext context)
		{
			_context = context;
		}

		public async Task<Feedback?> GetByIdAsync(Guid id)
		{
			return await _context.Feedbacks.FindAsync(id);
		}

		public async Task<IEnumerable<Feedback>> GetAllAsync()
		{
			return await _context.Feedbacks.ToListAsync();
		}

		public async Task<IEnumerable<Feedback>> FindAsync(Expression<Func<Feedback, bool>> predicate)
		{
			return await _context.Feedbacks.Where(predicate).ToListAsync();
		}
		public async Task<Feedback> AddAsync(Feedback entity)
		{
			await _context.Feedbacks.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<int> CountAsync(Expression<Func<Feedback, bool>>? predicate = null)
		{

			if (predicate == null)
			{
				return await _context.Feedbacks.CountAsync();
			}
			else
			{
				return await _context.Feedbacks.CountAsync(predicate);
			}
		}

		public async Task DeleteAsync(Feedback entity)
		{
			_context.Feedbacks.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<Feedback, bool>> predicate)
		{
			return await _context.Feedbacks.AnyAsync(predicate);
		} 

		public async Task UpdateAsync(Feedback entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}
	}
}
