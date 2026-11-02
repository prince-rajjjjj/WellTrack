using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Wellness.Repositories
{
	public interface IProgramRepository : IRepository<programs>{
		Task<IEnumerable<programs>> GetActivePrograms();
		


	}
}
