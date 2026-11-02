using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Identity.Repositories
{
	public interface IUserRepository:IRepository<User>
	{
		Task<User?> GetByEmailAsync(string email);
	}
}
