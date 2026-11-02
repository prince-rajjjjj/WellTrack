using WellTrack.Services.Compliance.Domain;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Compliance.Repositories;

public interface IAuditRepository : IRepository<Audit>
{
	Task<IEnumerable<Audit>> GetOpenAuditsAsync();
}
