using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Participation.Repository
{
	public interface IParticipationRepository : IRepository<ParticipationEntity>
	{
		Task<IEnumerable<ParticipationEntity>> GetByEmployeeIdAsync(Guid employeeId);
	}
}
