using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Application.Services
{
	public interface IParticipationService
	{
		Task<IEnumerable<ParticipationDto>> GetAllAsync();
		Task<IEnumerable<ParticipationDto>> GetByEmployeeIdAsync(Guid employeeId);
		Task<ParticipationDto> CreateParticipationAsync(CreateParticipationDto createParticipationDto);
		Task<Guid> JoinActivityAsync(Guid employeeId, JoinActivityRequest req);
		Task<PagedResult<ParticipationDto>> GetPagedMineAsync(Guid employeeId, int page, int pageSize, string? status = null);
		Task CompleteByActivityAsync(Guid activityId);
	}
}
