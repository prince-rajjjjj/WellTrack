using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Application.Services
{
	public interface ILogAuditService
	{
		Task CreateLogAsync(Guid id, string action, string resource);

		Task<PagedResult<AuditLogDto>> GetPagedLogsAsync(int page, int pageSize, string? search);
	}
}
