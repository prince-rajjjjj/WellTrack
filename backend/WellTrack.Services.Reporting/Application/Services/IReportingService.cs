using WellTrack.Services.Reporting.Application.DTOs;

namespace WellTrack.Services.Reporting.Application.Services
{
	public interface IReportingService
	{
		Task<ProgramReportDto> GetProgramReportAsync();
		Task<EngagementReportDto> GetEngagementReportAsync();
		Task<ComplianceReportDto> GetComplianceReportAsync();
		Task<ParticipationTrendReportDto> GetParticipationTrendsAsync();
		Task<EngagementScoreReportDto> GetEngagementScoresAsync();

		Task<List<UserDistributionDto>> GetUserDistributionAsync();
	}
}