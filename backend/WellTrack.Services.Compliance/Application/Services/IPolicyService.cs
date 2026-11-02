using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public interface IPolicyService
	{
		Task<IEnumerable<PolicyDto>> GetAllPoliciesAsync();
		Task<PolicyDto?> GetPolicyByIdAsync(Guid id);
		Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto dto);
		Task<PolicyDto?> UpdatePolicyAsync(Guid id, UpdatePolicyDto dto);
		Task<bool> DeletePolicyAsync(Guid id);
		Task<PagedResult<PolicyDto>> GetPagedPoliciesAsync(int page, int pageSize);
	}
}
