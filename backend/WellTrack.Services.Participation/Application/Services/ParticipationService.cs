using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Repository;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Application.Services
{
	public class ParticipationService : IParticipationService
	{
		private readonly IParticipationRepository _participationRepository;
		private readonly IMapper _mapper;
		private readonly INotificationClient _notify;

		public ParticipationService(
			IParticipationRepository participationRepository,
			IMapper mapper,
			INotificationClient notify)
		{
			_participationRepository = participationRepository;
			_mapper = mapper;
			_notify = notify;
		}

		// ─── Create ────────────────────────────────────────────────────────

		public async Task<ParticipationDto> CreateParticipationAsync(CreateParticipationDto dto)
		{
			var participation = _mapper.Map<ParticipationEntity>(dto);
			await _participationRepository.AddAsync(participation);
			return _mapper.Map<ParticipationDto>(participation);
		}

		// ─── Read ──────────────────────────────────────────────────────────

		public async Task<IEnumerable<ParticipationDto>> GetAllAsync()
		{
			var participants = await _participationRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ParticipationDto>>(participants);
		}

		public async Task<IEnumerable<ParticipationDto>> GetByEmployeeIdAsync(Guid employeeId)
		{
			var participations = await _participationRepository.GetByEmployeeIdAsync(employeeId);
			return _mapper.Map<IEnumerable<ParticipationDto>>(participations);
		}

		// ─── Join activity (the main user action) ──────────────────────────

		public async Task<Guid> JoinActivityAsync(Guid employeeId, JoinActivityRequest req)
		{
			var entity = _mapper.Map<ParticipationEntity>(req);
			entity.EmployeeID = employeeId;
			entity.ActivityTitle = req.ActivityTitle;
			await _participationRepository.AddAsync(entity);

			// 🔔 Confirm to the user
			await _notify.NotifyAsync(
				employeeId,
				$"You joined '{req.ActivityTitle}'.",
				"Activity",
				req.ProgramId);

			return entity.Id;
		}

		// ─── Pagination ────────────────────────────────────────────────────

		public async Task CompleteByActivityAsync(Guid activityId)
		{
			var records = (await _participationRepository.FindAsync(
				p => p.ActivityID == activityId && p.Status == SharedKernel.Enum.StatusType.Active))
				.ToList();
			foreach (var r in records)
			{
				r.Status = SharedKernel.Enum.StatusType.Completed;
				await _participationRepository.UpdateAsync(r);
			}
		}

		public async Task<PagedResult<ParticipationDto>> GetPagedMineAsync(
			Guid employeeId, int page, int pageSize, string? status = null)
		{
			var all = (await GetByEmployeeIdAsync(employeeId)).ToList();

			var filtered = string.IsNullOrEmpty(status) || status == "All"
				? all
				: all.Where(p => p.Status == status).ToList();

			var total = filtered.Count;
			var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<ParticipationDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}