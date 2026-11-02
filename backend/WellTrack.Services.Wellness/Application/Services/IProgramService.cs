using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Wellness.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using WellTrack.SharedKernel.Wrappers;


namespace WellTrack.Services.Wellness.Application.Service {


	public interface IProgramService
	{
		Task<IEnumerable<ProgramDto>> GetAllProgramsAsync();
		Task<ProgramDto> GetProgramByIdAsync(Guid id);
		Task<ProgramDto> CreateProgramAsync(CreatePragramDto addDto);

		Task<ProgramDto> UpdateProgramAsync(Guid id, UpdateProgramDto updateDto);

		Task DeleteProgramAsync(Guid id);

		Task<PagedResult<ProgramDto>> GetPagedProgramsAsync(int page, int pageSize, string? search = null, string? status = null);



	}

}
