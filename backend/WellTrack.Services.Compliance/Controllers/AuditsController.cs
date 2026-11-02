using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Application.Services;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Controllers
{
	[ApiController]
	[Route("api/v1/audits")]
	[Authorize]
	public class AuditsController : ControllerBase
	{
		private readonly IAuditService _auditService;

		public AuditsController(IAuditService auditService)
		{
			_auditService = auditService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,HR,ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<PagedResult<AuditDto>>>> GetAll(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 9)
		{
			var data = await _auditService.GetPagedAuditsAsync(page, pageSize);
			return Ok(new ApiResponse<PagedResult<AuditDto>>(data, "Audits retrieved."));
		}

		[HttpPost]
		[Authorize(Roles = "ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<AuditDto>>> Create([FromBody] CreateAuditDto dto)
		{

			if (dto == null)
			{
				return BadRequest(new ApiResponse<AuditDto>(null, "Audit data is required."));
			}

			var audit = await _auditService.CreateAuditAsync(dto);
			return Ok(new ApiResponse<AuditDto>(audit, "created as draft"));
		}

		[HttpPatch("{id}/submit")]
		[Authorize(Roles = "ComplianceOfficer")]
		public async Task<ActionResult<ApiResponse<AuditDto>>> Submit(Guid id)
		{
			var audit = await _auditService.SubmitAuditAsync(id);
			return Ok(new ApiResponse<AuditDto>(audit, "Audit submitted for approval"));
		}

		[HttpPatch("{id}/approve")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<AuditDto>>> Approve(Guid id)
		{
			var audit = await _auditService.ApproveAuditAsync(id);
			return Ok(new ApiResponse<AuditDto>(audit, "Audit approved"));
		}

		// Reject auto-closes the audit (terminal state) and stores the rejection reason.
		[HttpPatch("{id}/reject")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<AuditDto>>> Reject(Guid id, [FromBody] RejectAuditDto dto)
		{
			var audit = await _auditService.RejectAuditAsync(id, dto);
			return Ok(new ApiResponse<AuditDto>(audit, "Audit rejected and closed"));
		}
	}
}