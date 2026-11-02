using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Reporting.Application.DTOs;
using WellTrack.Services.Reporting.Application.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Reporting.Controllers
{


	[ApiController]
	[Route("api/v1/reports")]
	[Authorize]
	public class ReportsController: ControllerBase
	{
		private readonly IReportingService _reportingService;

		public ReportsController(IReportingService reportingService)
		{
			_reportingService = reportingService;
		}


		[HttpGet("programs")]
		[Authorize(Roles = "Admin,HR")]
		public async Task <ActionResult<ApiResponse<ProgramReportDto>>> GetProgramReport()
		{
			var report = await _reportingService.GetProgramReportAsync();
			return Ok(new ApiResponse<ProgramReportDto>(report));
		}


		[HttpGet("engagement")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<EngagementReportDto>>> GetEngagementReport()
		{
			var report = await _reportingService.GetEngagementReportAsync();
			return Ok(new ApiResponse<EngagementReportDto>(report));
		}


		[HttpGet("complaince")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ComplianceReportDto>>> GetComplianceReport()
		{
			var report = await _reportingService.GetComplianceReportAsync();
			return Ok(new ApiResponse<ComplianceReportDto>(report));
		}


		[HttpGet("participation")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<object>>> GetParticipationReport()
		{
			var report = await _reportingService.GetParticipationTrendsAsync();
			return Ok(new ApiResponse<object>(new
			{
				totalParticipants = report.TotalParticipants,
				activeParticipants = report.ActiveParticipants,
				participationRate=report.ParticipationRate,
				programWithHighestParticipation= report.TopPrograms?.Select(p => new
				{
					programName = p.ProgramName,
					participants=p.Participants,
					completionRate=p.CompletionRate
				}),
				monthlyTrends = report.MonthlyTrends?.Select(m => new
				{
					month = m.Month,
					participants = m.Participants,
					activitiesCompleted=m.ActivitiesCompleted,
				})
			}));
		}

		[HttpGet("participation-trends")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ParticipationTrendReportDto>>> GetParticipationTrends()
		{
			var report = await _reportingService.GetParticipationTrendsAsync();
			return Ok(new ApiResponse<ParticipationTrendReportDto>(report));
		}



		[HttpGet("engagement-scores")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<EngagementScoreReportDto>>> GetEngagementScores()
		{
			var report = await _reportingService.GetEngagementScoresAsync();
			return Ok(new ApiResponse<EngagementScoreReportDto>(report));
		}


		
		[HttpGet("user-distribution")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<List<UserDistributionDto>>>> GetUserDistribution()
		{
			var result = await _reportingService.GetUserDistributionAsync();
			return Ok(new ApiResponse<List<UserDistributionDto>>(result));
		}


	}
}
