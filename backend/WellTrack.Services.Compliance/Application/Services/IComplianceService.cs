using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public interface IComplianceService
	{
		Task<IEnumerable<ComplianceRecordDto>> GetAllRecordsAsync();
		Task<ComplianceRecordDto?> GetRecordByIdAsync(Guid id);
		Task<ComplianceRecordDto> CreateRecordAsync(CreateComplianceRecordDto dto);
		Task<object> GetDashboardStatsAsync();
		Task<ComplianceRecordDto> StartRecordAsync(Guid id);
		Task<ComplianceRecordDto> CloseRecordAsync(Guid id, string? resolutionNotes = null);
		Task<PagedResult<ComplianceRecordDto>> GetPagedRecordsAsync(int page, int pageSize, string? status = null);
	}
}
