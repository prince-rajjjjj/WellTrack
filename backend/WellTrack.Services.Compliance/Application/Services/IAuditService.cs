using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public interface IAuditService
	{
		Task<IEnumerable<AuditDto>>GetAllAuditsAsync();
		Task<AuditDto> CreateAuditAsync(CreateAuditDto dto);
		Task<AuditDto?> SubmitAuditAsync(Guid id);
		Task<AuditDto?> ApproveAuditAsync(Guid id);
		Task<AuditDto?> RejectAuditAsync(Guid id, RejectAuditDto dto);
		Task<AuditDto?> CloseAuditAsync(Guid id, CloseAuditDto dto);
		Task<PagedResult<AuditDto>> GetPagedAuditsAsync(int page, int pageSize);
	}
}
