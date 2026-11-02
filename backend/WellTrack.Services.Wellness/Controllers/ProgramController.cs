using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Application.Service;
using WellTrack.Services.Wellness.Application.Services;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Controllers
{
	[ApiController]
	[Route("api/v1/programs")]
	
	public class ProgramController : ControllerBase // Inheriting from ControllerBase is better for APIs
	{
		// Removed _programDbContext as Controllers should only talk to Services, not the DB directly.
		private readonly IProgramService _programService;
		private readonly IMapper _mapper;

		public ProgramController(IProgramService programService, IMapper mapper)
		{
			_programService = programService;
			_mapper = mapper;
		}




		[HttpGet]
		// Removed redundant [Route("GetAllPrograms")] to follow REST standards (GET api/v1/programs)
		public async Task<ActionResult<ApiResponse<PagedResult<ProgramDto>>>> GetAll(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 9,
			[FromQuery] string? search = null,
			[FromQuery] string? status = null)
		{
			var data = await _programService.GetPagedProgramsAsync(page, pageSize, search, status);
			return Ok(new ApiResponse<PagedResult<ProgramDto>>(data));
		}

		[HttpGet("{id:guid}", Name = "GetProgram")]
		public async Task<ActionResult<ApiResponse<ProgramDto?>>> GetById([FromRoute] Guid id)
		{
			var program = await _programService.GetProgramByIdAsync(id);

			if (program == null)
			{
				// ApiResponse<ProgramDto?> allows for a null data property safely
				return NotFound(new ApiResponse<ProgramDto?>(null, "Program not found"));
			}

			return Ok(new ApiResponse<ProgramDto>(program));
		}

		[HttpPost]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ProgramDto>>> Add([FromBody] CreatePragramDto addDto)
		{

			var program = await _programService.CreateProgramAsync(addDto);

			// Correctly returns 201 Created with the location header
			return CreatedAtRoute(
				"GetProgram",
				new { id = program.Id },
				new ApiResponse<ProgramDto>(program, "Program created successfully")
			);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<ProgramDto?>>> Update([FromRoute] Guid id, [FromBody] UpdateProgramDto updateDto)
		{
			var updatedProgram = await _programService.UpdateProgramAsync(id, updateDto);

			if (updatedProgram == null)
			{
				return NotFound(new ApiResponse<ProgramDto?>(null, "Program not found"));
			}

			return Ok(new ApiResponse<ProgramDto>(updatedProgram, "Program updated successfully"));
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<string?>>> Delete([FromRoute] Guid id)
		{
			await _programService.DeleteProgramAsync(id);
			return Ok(new ApiResponse<string?>(null, "Program deleted successfully"));
		}

		[HttpPatch("{id:guid}/deactivate")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<ActionResult<ApiResponse<object>>> Deactivate([FromRoute] Guid id)
		{
			// Renamed from 'Deactive' to 'Deactivate' (Verb)
			// Currently a mock response as per your design
			var result = new { programId = id, deactivated = true };
			return Ok(new ApiResponse<object>(result, "Program deactivated successfully"));
		}

		

	}
}