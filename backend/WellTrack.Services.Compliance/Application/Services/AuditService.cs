using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.Services.Compliance.Repositories;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Compliance.Application.Services
{
	public class AuditService : IAuditService
	{
		private readonly IAuditRepository _repo;
		private readonly IMapper _mapper;
		private readonly INotificationClient _notify;

		public AuditService(
			IAuditRepository repo,
			IMapper mapper,
			INotificationClient notify)
		{
			_repo = repo;
			_mapper = mapper;
			_notify = notify;
		}

		// ─── Create ────────────────────────────────────────────────────────

		public async Task<AuditDto> CreateAuditAsync(CreateAuditDto dto)
		{
			var audit = _mapper.Map<Audit>(dto);
			await _repo.AddAsync(audit);

			// 🔔 Notify the assigned compliance officer
			await _notify.NotifyAsync(
				audit.ComplianceOfficerID,
				$"New audit created: '{audit.Scope}'. Status: Draft",
				"Compliance");

			return _mapper.Map<AuditDto>(audit);
		}

		// ─── Read ──────────────────────────────────────────────────────────

		public async Task<IEnumerable<AuditDto>> GetAllAuditsAsync()
		{
			var audits = await _repo.GetAllAsync();
			return _mapper.Map<List<AuditDto>>(audits);
		}

		// ─── State transitions ──────────────────────────────────────────────

		public async Task<AuditDto?> SubmitAuditAsync(Guid id)
		{
			var audit = await _repo.GetByIdAsync(id);
			if (audit == null) return null;

			if (audit.AuditStatus != AuditStatus.Draft)
				throw new InvalidOperationException("Only draft audits can be submitted.");

			audit.AuditStatus = AuditStatus.Submitted;
			audit.Status = StatusType.Active;
			audit.UpdatedAt = DateTime.UtcNow;
			await _repo.UpdateAsync(audit);

			// 🔔 Notify the officer that audit was submitted for review
			await _notify.NotifyAsync(
				audit.ComplianceOfficerID,
				$"Audit '{audit.Scope}' has been submitted for review.",
				"Compliance");

			return _mapper.Map<AuditDto>(audit);
		}

		public async Task<AuditDto?> ApproveAuditAsync(Guid id)
		{
			var audit = await _repo.GetByIdAsync(id);
			if (audit == null) return null;

			if (audit.AuditStatus != AuditStatus.Submitted)
				throw new InvalidOperationException("Only submitted audits can be approved.");

			audit.AuditStatus = AuditStatus.Approved;
			audit.Status = StatusType.Completed;
			audit.UpdatedAt = DateTime.UtcNow;
			await _repo.UpdateAsync(audit);

			// 🔔 Notify the audit owner
			await _notify.NotifyAsync(
				audit.ComplianceOfficerID,
				$"✅ Your audit '{audit.Scope}' has been approved.",
				"Compliance");

			return _mapper.Map<AuditDto>(audit);
		}

		public async Task<AuditDto?> RejectAuditAsync(Guid id, RejectAuditDto dto)
		{
			var audit = await _repo.GetByIdAsync(id);
			if (audit == null) return null;

			if (audit.AuditStatus != AuditStatus.Submitted)
				throw new InvalidOperationException("Only submitted audits can be rejected.");

			audit.AuditStatus = AuditStatus.Closed;
			audit.Status = StatusType.Inactive;
			audit.RejectionReason = dto.Reason;
			audit.UpdatedAt = DateTime.UtcNow;
			await _repo.UpdateAsync(audit);

			// 🔔 Notify the audit owner with the reason
			await _notify.NotifyAsync(
				audit.ComplianceOfficerID,
				$"❌ Your audit '{audit.Scope}' was rejected. Reason: {dto.Reason}",
				"Compliance");

			return _mapper.Map<AuditDto>(audit);
		}

		public async Task<AuditDto?> CloseAuditAsync(Guid id, CloseAuditDto dto)
		{
			var audit = await _repo.GetByIdAsync(id);
			if (audit == null) return null;

			if (audit.AuditStatus != AuditStatus.Approved)
				throw new InvalidOperationException("Audit must be Approved before it can be Closed.");

			audit.AuditStatus = AuditStatus.Closed;
			audit.Status = StatusType.Completed;

			if (!string.IsNullOrWhiteSpace(dto.FinalFindings))
				audit.Findings = dto.FinalFindings;

			audit.UpdatedAt = DateTime.UtcNow;
			await _repo.UpdateAsync(audit);

			// 🔔 Notify the audit owner that it's officially closed
			await _notify.NotifyAsync(
				audit.ComplianceOfficerID,
				$"Audit '{audit.Scope}' has been closed.",
				"Compliance");

			return _mapper.Map<AuditDto>(audit);
		}

		// ─── Pagination ────────────────────────────────────────────────────

		public async Task<PagedResult<AuditDto>> GetPagedAuditsAsync(int page, int pageSize)
		{
			var all = (await _repo.GetAllAsync()).ToList();
			var total = all.Count;
			var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<AuditDto>
			{
				Items = _mapper.Map<List<AuditDto>>(items),
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}