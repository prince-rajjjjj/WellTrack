using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Application.Services;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Controllers
{
	[ApiController]
	[Route("api/v1/compliance-records")]
	[Authorize]
	public class ComplianceController : ControllerBase
	{
		private readonly IComplianceService _complianceService;

		public ComplianceController(IComplianceService complianceService)
		{
			_complianceService = complianceService;
		}

		// View — Admin (oversight), HR (resolver), ComplianceOfficer (reporter)
		[HttpGet]
		[Authorize(Roles = "Admin,HR,ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<PagedResult<ComplianceRecordDto>>>> GetAll(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
		{
			var data = await _complianceService.GetPagedRecordsAsync(page, pageSize, status);
			return Ok(new ApiResponse<PagedResult<ComplianceRecordDto>>(data, "Records retrieved."));
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,HR,ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<ComplianceRecordDto>>> GetById(Guid id)
		{
			var record = await _complianceService.GetRecordByIdAsync(id);
			if (record == null) return NotFound(new ApiResponse<ComplianceRecordDto>(null, "Record not found"));
			return Ok(new ApiResponse<ComplianceRecordDto>(record));
		}

		// Report — only ComplianceOfficer raises issues
		[HttpPost]
		[Authorize(Roles = "ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<ComplianceRecordDto>>> Create([FromBody] CreateComplianceRecordDto dto)
		{
			var record = await _complianceService.CreateRecordAsync(dto);
			return Ok(new ApiResponse<ComplianceRecordDto>(record, "Issue reported successfully"));
		}

		[HttpGet("stats")]
		[Authorize(Roles = "Admin,HR,ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<object>>> GetStats()
		{
			var stats = await _complianceService.GetDashboardStatsAsync();
			return Ok(new ApiResponse<object>(stats));
		}

		// Resolve — only HR moves issues through the lifecycle
		[HttpPatch("{id}/start")]
		[Authorize(Roles = "HR")]
		public async Task<ActionResult<ApiResponse<ComplianceRecordDto>>> Start(Guid id)
		{
			var record = await _complianceService.StartRecordAsync(id);
			return Ok(new ApiResponse<ComplianceRecordDto>(record, "Record started"));
		}

		[HttpPatch("{id}/close")]
		[Authorize(Roles = "HR")]
		public async Task<ActionResult<ApiResponse<ComplianceRecordDto>>> Close(
			Guid id,
			[FromBody] CloseComplianceRecordDto? body)
		{
			var record = await _complianceService.CloseRecordAsync(id, body?.ResolutionNotes);
			return Ok(new ApiResponse<ComplianceRecordDto>(record, "Record closed"));
		}
	}
}
