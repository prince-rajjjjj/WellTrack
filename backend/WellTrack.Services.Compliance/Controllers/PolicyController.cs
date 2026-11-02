using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Application.Services;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Controllers
{
	[ApiController]
	[Route("api/v1/policies")]
	[Authorize]
	public class PolicyController : ControllerBase
	{
		private readonly IPolicyService _policyService;

		public PolicyController(IPolicyService policyService)
		{
			_policyService = policyService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,ComplianceOfficer,HR")]
		public async Task<ActionResult<ApiResponse<PagedResult<PolicyDto>>>> GetAll(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var data = await _policyService.GetPagedPoliciesAsync(page, pageSize);
			return Ok(new ApiResponse<PagedResult<PolicyDto>>(data, "Policies retrieved."));
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,ComplianceOfficer,HR")]
		public async Task<ActionResult<ApiResponse<PolicyDto>>> GetById(Guid id)
		{
			var policy = await _policyService.GetPolicyByIdAsync(id);
			if (policy == null)
			{
				return NotFound(new ApiResponse<PolicyDto>(null, $"Policy with ID {id} was not found."));
			}

			return Ok(new ApiResponse<PolicyDto>(policy));
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<PolicyDto>>> Create([FromBody] CreatePolicyDto dto)
		{
			var policy = await _policyService.CreatePolicyAsync(dto);
			return Ok(new ApiResponse<PolicyDto>(policy, "Policy created successfully."));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<PolicyDto>>> Update(Guid id, [FromBody] UpdatePolicyDto dto)
		{
			var policy = await _policyService.UpdatePolicyAsync(id, dto);
			if (policy == null)
			{
				return NotFound(new ApiResponse<PolicyDto>(null, $"Policy with ID {id} not found."));
			}

			return Ok(new ApiResponse<PolicyDto>(policy, "Policy updated successfully"));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
		{
			var success = await _policyService.DeletePolicyAsync(id);
			if (!success)
			{
				return NotFound(new ApiResponse<object>(null, $"Policy with ID {id} not found."));
			}

			return Ok(new ApiResponse<object>(null, "Policy deleted successfully."));
		}

	}
}