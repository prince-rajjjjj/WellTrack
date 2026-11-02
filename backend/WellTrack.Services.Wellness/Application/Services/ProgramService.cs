using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Application.Service;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.Services.Wellness.Repositories;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Application.Services
{
	public class ProgramService : IProgramService
	{
		private readonly IProgramRepository _programRepository;
		private readonly IMapper _mapper;
		private readonly IIdentityClient _identityClient;
		private readonly INotificationClient _notify;

		public ProgramService(
			IProgramRepository programRepository,
			IMapper mapper,
			IIdentityClient identityClient,
			INotificationClient notify)
		{
			_programRepository = programRepository;
			_mapper = mapper;
			this._identityClient = identityClient;
			_notify = notify;
		}

		public async Task<IEnumerable<ProgramDto>> GetAllProgramsAsync()
		{
			var programs = await _programRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ProgramDto>>(programs);
		}

		public async Task<ProgramDto> GetProgramByIdAsync(Guid id)
		{
			var program = await _programRepository.GetByIdAsync(id);
			if (program == null)
				throw new NotFoundException(nameof(programs), id);

			return _mapper.Map<ProgramDto>(program);
		}

		public async Task<ProgramDto> CreateProgramAsync(CreatePragramDto addDto)
		{
			var programEntity = _mapper.Map<programs>(addDto);
			await _programRepository.AddAsync(programEntity);

			// 🔔 Notify HR creator
			await _notify.NotifyAsync(
				programEntity.HRID,
				$"Program '{programEntity.Title}' has been created successfully",
				"Program",
				programEntity.Id);

			// 🔔 Notify all employees about the new program
			var allEmployeeIds = await _identityClient.GetAllEmployeeIdsAsync();
			if (allEmployeeIds.Any())
			{
				await _notify.NotifyManyAsync(
					allEmployeeIds,
					$"New wellness program available: '{programEntity.Title}'",
					"Program",
					programEntity.Id);
			}

			return _mapper.Map<ProgramDto>(programEntity);
		}

		public async Task<ProgramDto> UpdateProgramAsync(Guid id, UpdateProgramDto updateDto)
		{
			var existingProgram = await _programRepository.GetByIdAsync(id);
			if (existingProgram == null)
				throw new NotFoundException(nameof(programs), id);

			_mapper.Map(updateDto, existingProgram);
			await _programRepository.UpdateAsync(existingProgram);

			// 🔔 Notify HR that program was updated
			await _notify.NotifyAsync(
				existingProgram.HRID,
				$"Program '{existingProgram.Title}' has been updated",
				"Program",
				existingProgram.Id);

			return _mapper.Map<ProgramDto>(existingProgram);
		}

		public async Task DeleteProgramAsync(Guid id)
		{
			var existingProgram = await _programRepository.GetByIdAsync(id);
			if (existingProgram == null)
				throw new NotFoundException(nameof(programs), id);

			await _programRepository.DeleteAsync(existingProgram);

			// 🔔 Notify HR that program was deleted
			await _notify.NotifyAsync(
				existingProgram.HRID,
				$"Program '{existingProgram.Title}' has been deleted",
				"Program");
		}

		public async Task<PagedResult<ProgramDto>> GetPagedProgramsAsync(
			int page, int pageSize, string? search = null, string? status = null)
		{
			var dtos = (await GetAllProgramsAsync()).ToList();
			var filtered = dtos.AsEnumerable();

			if (!string.IsNullOrEmpty(search))
			{
				var s = search.ToLowerInvariant();
				filtered = filtered.Where(p =>
					p.Title.ToLowerInvariant().Contains(s) ||
					(p.Description ?? "").ToLowerInvariant().Contains(s));
			}

			if (!string.IsNullOrEmpty(status))
			{
				filtered = status.ToLower() switch
				{
					"active" => filtered.Where(p => p.Status == StatusType.Active),
					"pending" => filtered.Where(p => p.Status == StatusType.Pending),
					"completed" => filtered.Where(p => p.Status == StatusType.Completed),
					"inactive" => filtered.Where(p => p.Status == StatusType.Inactive),
					"cancelled" => filtered.Where(p => p.Status == StatusType.Cancelled),
					_ => filtered
				};
			}

			var list = filtered.ToList();
			var total = list.Count;
			var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<ProgramDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}