using WellTrack.Services.Engagement.Domain.Entities;

namespace WellTrack.Services.Engagement.Application.Services
{
	/// <summary>
	/// Fetches data from Wellness (port 5002) and Participation (port 5003) via HTTP,
	/// then computes the 5-factor engagement score for a given employee+program pair.
	/// </summary>
	public interface IEngagementCalculationService
	{
		/// <param name="jwtToken">The caller's full "Bearer xxx" token — forwarded to downstream services.</param>
		Task<EngagementEntity> CalculateAsync(Guid employeeId, Guid programId, string jwtToken);

		/// <summary>
		/// Scans ALL Wellness activities and their attendance rosters to discover every
		/// (employeeId, programId) pair that exists. Used by RecalculateAll to bootstrap
		/// records for employees who have never had a trigger fire.
		/// </summary>
		Task<IEnumerable<(Guid EmployeeId, Guid ProgramId)>> ScanAllEmployeeProgramPairsAsync(string jwtToken);
	}
}
