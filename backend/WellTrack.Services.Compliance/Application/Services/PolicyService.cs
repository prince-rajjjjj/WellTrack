using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Repository;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public class PolicyService : IPolicyService
	{
		private readonly IPolicyRepository _repo;
		private readonly IMapper _mapper;

		public PolicyService(IPolicyRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<IEnumerable<PolicyDto>> GetAllPoliciesAsync()
		{
			var policies = await _repo.GetAllAsync();
			return _mapper.Map<IEnumerable<PolicyDto>>(policies);
		}

		public async Task<PolicyDto?> GetPolicyByIdAsync(Guid id)
		{
			var policy = await _repo.GetByIdAsync(id);
			if (policy == null) return null;

			return _mapper.Map<PolicyDto>(policy);
		}

		public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto dto)
		{
			var policy = _mapper.Map<Policy>(dto);
			policy.Status = PolicyStatus.Active;
			policy.CreatedAt = DateTime.UtcNow;

			var createdPolicy = await _repo.AddAsync(policy);
			return _mapper.Map<PolicyDto>(createdPolicy);
		}

		public async  Task<PolicyDto?> UpdatePolicyAsync(Guid id, UpdatePolicyDto dto)
		{
			var existingPolicy = await _repo.GetByIdAsync(id);
			if (existingPolicy == null) return null;

			_mapper.Map(dto, existingPolicy);
			existingPolicy.UpdatedAt = DateTime.UtcNow;

			await _repo.UpdateAsync(existingPolicy);
			return _mapper.Map<PolicyDto>(existingPolicy);
		}

		public async Task<bool> DeletePolicyAsync(Guid id)
		{
			var policy = await _repo.GetByIdAsync(id);
			if (policy == null) return false;

			await _repo.DeleteAsync(policy);
			return true;
		}

		public async Task<PagedResult<PolicyDto>> GetPagedPoliciesAsync(int page, int pageSize)
		{
			var all   = (await _repo.GetAllAsync()).ToList();
			var total = all.Count;
			var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
			return new PagedResult<PolicyDto>
			{
				Items      = _mapper.Map<List<PolicyDto>>(items),
				TotalCount = total,
				Page       = page,
				PageSize   = pageSize
			};
		}
	}
}
