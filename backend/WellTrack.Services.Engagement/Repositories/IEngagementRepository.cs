using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Engagement.Repositories
{
	public interface IEngagementRepository: IRepository<EngagementEntity>
	{
	}
}
