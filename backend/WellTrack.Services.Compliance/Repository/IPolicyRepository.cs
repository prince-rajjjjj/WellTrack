using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Compliance.Repository
{
	public interface IPolicyRepository : IRepository<Policy>
	{
		Task<IEnumerable<Policy>> GetActivePoliciesAsync();
	}
}