using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Compliance.Repository
{
	public interface IComplianceRecordRepository : IRepository<ComplianceRecord>
	{
		Task<IEnumerable<ComplianceRecord>> GetByProgramIdAsync(Guid programId);
		Task<IEnumerable<ComplianceRecord>> GetOpenRecordsAsync();
	}
}
