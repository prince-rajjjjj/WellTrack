using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Repository;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public class ComplianceService : IComplianceService
	{
		private readonly IComplianceRecordRepository _recordRepo;
		private readonly IPolicyRepository _policyRepo;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUser;
		private readonly INotificationClient _notify;
		private readonly IIdentityClient _identityClient;

		public ComplianceService(
			IComplianceRecordRepository recordRepo,
			IPolicyRepository policyRepo,
			IMapper mapper,
			ICurrentUserService currentUser,
			INotificationClient notify,
			IIdentityClient identityClient)
		{
			_recordRepo = recordRepo;
			_policyRepo = policyRepo;
			_mapper = mapper;
			_currentUser = currentUser;
			_notify = notify;
			_identityClient = identityClient;
		}

		// ─── Reads ─────────────────────────────────────────────────────────

		public async Task<IEnumerable<ComplianceRecordDto>> GetAllRecordsAsync()
		{
			var records = (await _recordRepo.GetAllAsync()).ToList();
			var policies = (await _policyRepo.GetAllAsync()).ToDictionary(p => p.Id);

			var dtos = _mapper.Map<IEnumerable<ComplianceRecordDto>>(records).ToList();

			for (int i = 0; i < dtos.Count; i++)
				ResolveDisplayNames(dtos[i], records[i], policies);

			return dtos;
		}

		public async Task<ComplianceRecordDto?> GetRecordByIdAsync(Guid id)
		{
			var record = await _recordRepo.GetByIdAsync(id);
			if (record == null) return null;

			var dto = _mapper.Map<ComplianceRecordDto>(record);
			var policy = await _policyRepo.GetByIdAsync(record.PolicyID);
			var policies = policy != null
				? new Dictionary<Guid, Policy> { [policy.Id] = policy }
				: new Dictionary<Guid, Policy>();

			ResolveDisplayNames(dto, record, policies);
			return dto;
		}

		// ─── Create ────────────────────────────────────────────────────────

		public async Task<ComplianceRecordDto> CreateRecordAsync(CreateComplianceRecordDto dto)
		{
			var entity = _mapper.Map<ComplianceRecord>(dto);

			var policy = await _policyRepo.GetByIdAsync(dto.PolicyID);
			if (policy != null)
				entity.PolicyNameSnapshot = policy.PolicyName;

			entity.ProgramNameSnapshot = dto.ProgramName;
			entity.ActivityNameSnapshot = dto.ActivityName;
			entity.CreatedBy = _currentUser.UserId.ToString();

			var saved = await _recordRepo.AddAsync(entity);
			var savedDto = _mapper.Map<ComplianceRecordDto>(saved);

			savedDto.PolicyName = policy?.PolicyName ?? "(deleted policy)";
			savedDto.ProgramName = entity.ProgramNameSnapshot ?? string.Empty;
			savedDto.ActivityName = entity.ActivityNameSnapshot ?? string.Empty;
			savedDto.CreatedBy = entity.CreatedBy;

			// 🔔 Confirm to the reporter
			await _notify.NotifyAsync(
				_currentUser.UserId,
				$"Compliance issue reported: '{policy?.PolicyName ?? "Unknown Policy"}'",
				"Compliance");

			// 🔔 Notify all Compliance Officers about the new issue
			var officerIds = await _identityClient.GetUserIdsByRoleAsync("ComplianceOfficer");
			if (officerIds.Any())
			{
				await _notify.NotifyManyAsync(
					officerIds,
					$"New compliance issue reported (Severity: {entity.Severity})",
					"Compliance");
			}

			return savedDto;
		}

		// ─── State transitions ──────────────────────────────────────────────

		public async Task<ComplianceRecordDto> StartRecordAsync(Guid id)
		{
			var record = await _recordRepo.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Compliance record {id} not found.");

			if (record.Status != ComplianceRecordStatus.Reported)
				throw new InvalidOperationException("Only Reported records can be started.");

			record.Status = ComplianceRecordStatus.InProgress;
			record.UpdatedAt = DateTime.UtcNow;
			await _recordRepo.UpdateAsync(record);

			// 🔔 Notify the reporter their issue is being worked on
			if (Guid.TryParse(record.CreatedBy, out var reporterId))
			{
				await _notify.NotifyAsync(
					reporterId,
					$"Your compliance issue is now being investigated.",
					"Compliance");
			}

			return _mapper.Map<ComplianceRecordDto>(record);
		}

		public async Task<ComplianceRecordDto> CloseRecordAsync(Guid id, string? resolutionNotes = null)
		{
			var record = await _recordRepo.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Compliance record {id} not found.");

			if (record.Status != ComplianceRecordStatus.InProgress)
				throw new InvalidOperationException("Only InProgress records can be closed.");

			record.Status = ComplianceRecordStatus.Closed;
			record.ResolutionNotes = resolutionNotes;
			record.UpdatedAt = DateTime.UtcNow;
			await _recordRepo.UpdateAsync(record);

			// 🔔 Notify the reporter that their issue has been resolved
			if (Guid.TryParse(record.CreatedBy, out var reporterId))
			{
				var msg = !string.IsNullOrWhiteSpace(resolutionNotes)
					? $"Your compliance issue has been resolved: {resolutionNotes}"
					: "Your compliance issue has been resolved.";

				await _notify.NotifyAsync(reporterId, msg, "Compliance");
			}

			// Re-resolve display names on the way out so the UI gets a complete row back.
			var dto = _mapper.Map<ComplianceRecordDto>(record);
			var policy = await _policyRepo.GetByIdAsync(record.PolicyID);
			var policies = policy != null
				? new Dictionary<Guid, Policy> { [policy.Id] = policy }
				: new Dictionary<Guid, Policy>();
			ResolveDisplayNames(dto, record, policies);
			return dto;
		}

		// ─── Stats ─────────────────────────────────────────────────────────

		public async Task<object> GetDashboardStatsAsync()
		{
			var records = (await _recordRepo.GetAllAsync()).ToList();
			var policies = (await _policyRepo.GetAllAsync()).ToList();

			var openCount = records.Count(r => r.Status == ComplianceRecordStatus.Reported ||
													 r.Status == ComplianceRecordStatus.InProgress);
			var closedCount = records.Count(r => r.Status == ComplianceRecordStatus.Closed);
			var activePolicies = policies.Count(p => p.Status == PolicyStatus.Active);

			var bySeverity = records
				.GroupBy(r => r.Severity.ToString())
				.Select(g => new { severity = g.Key, count = g.Count() });

			var byCategory = records
				.GroupBy(r => r.IssueType)
				.Select(g => new { category = g.Key, count = g.Count() })
				.OrderByDescending(x => x.count)
				.Take(5);

			return new
			{
				totalPolicies = policies.Count,
				activePolicies,
				activeViolations = openCount,
				resolvedViolations = closedCount,
				totalRecords = records.Count,
				violationsBySeverity = bySeverity,
				violationsByCategory = byCategory
			};
		}

		// ─── Pagination ────────────────────────────────────────────────────

		public async Task<PagedResult<ComplianceRecordDto>> GetPagedRecordsAsync(int page, int pageSize, string? status = null)
		{
			var all = (await GetAllRecordsAsync()).ToList();

			var filtered = string.IsNullOrEmpty(status) || status == "All"
				? all
				: all.Where(r => r.Status == status).ToList();

			var total = filtered.Count;
			var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<ComplianceRecordDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}

		// ─── Helpers ───────────────────────────────────────────────────────

		private static void ResolveDisplayNames(ComplianceRecordDto dto,ComplianceRecord rec,IReadOnlyDictionary<Guid, Policy> policies)
		{
			if (policies.TryGetValue(dto.PolicyID, out var policy))
				dto.PolicyName = policy.PolicyName;
			else if (!string.IsNullOrWhiteSpace(rec.PolicyNameSnapshot))
				dto.PolicyName = rec.PolicyNameSnapshot + " (deleted)";
			else
				dto.PolicyName = "(deleted policy)";

			dto.ProgramName = !string.IsNullOrWhiteSpace(rec.ProgramNameSnapshot)
				? rec.ProgramNameSnapshot
				: string.Empty;

			dto.ActivityName = !string.IsNullOrWhiteSpace(rec.ActivityNameSnapshot)
				? rec.ActivityNameSnapshot
				: string.Empty;
		}
	}
}