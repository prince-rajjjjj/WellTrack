using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
	Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);

	Task<PagedResult<AuditLog>> GetPagedLogsAsync(int page, int pageSize, string? search);
}
