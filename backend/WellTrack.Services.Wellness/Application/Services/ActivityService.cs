using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.Services.Wellness.Repositories;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Application.Services
{
	public class ActivityService : IActivityService
	{
		private readonly IActivityRepository _activityRepo;
		private readonly IProgramRepository _programRepo;
		private readonly ICurrentUserService _currentUser;
		private readonly INotificationClient _notify;
		private readonly IIdentityClient _identityClient;
		private readonly IMapper _mapper;

		public ActivityService(
			IActivityRepository activityRepo,
			IProgramRepository programRepo,
			ICurrentUserService currentUser,
			INotificationClient notify,
			IIdentityClient identityClient,
			IMapper mapper)
		{
			_activityRepo = activityRepo;
			_programRepo = programRepo;
			_currentUser = currentUser;
			_notify = notify;
			_identityClient = identityClient;
			_mapper = mapper;
		}

		// ─── Reads ──────────────────────────────────────────────────────────

		public async Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync()
		{
			var activities = await _activityRepo.GetAllAsync();
			var dtos = _mapper.Map<List<ActivityDto>>(activities);
			await EnrichAsync(dtos);
			return dtos;
		}

		public async Task<IEnumerable<ActivityDto>> GetActivitiesByProgramIdAsync(Guid programId)
		{
			var activities = await _activityRepo.GetByProgramIdAsync(programId);
			var dtos = _mapper.Map<List<ActivityDto>>(activities);
			await EnrichAsync(dtos);
			return dtos;
		}

		public async Task<ActivityDto?> GetByIdAsync(Guid id)
		{
			var activity = await _activityRepo.GetByIdAsync(id);
			if (activity == null) return null;
			return await ToEnrichedDtoAsync(activity);
		}

		// ─── Create / Update ────────────────────────────────────────────────

		public async Task<ActivityDto> createActivityAsync(CreateActivityDto dto)
		{
			ValidateModeFields(dto.Mode, dto.Location, dto.VideoUrl);

			var activity = _mapper.Map<Activity>(dto);
			activity.Id = Guid.NewGuid();
			activity.CreatedAt = DateTime.UtcNow;
			activity.UpdatedAt = DateTime.UtcNow;

			await _activityRepo.AddAsync(activity);

			// 🔔 Notify all employees about new activity
			var allEmployeeIds = await _identityClient.GetAllEmployeeIdsAsync();
			if (allEmployeeIds.Any())
			{
				await _notify.NotifyManyAsync(
					allEmployeeIds,
					$"New activity available: '{activity.Title}'",
					"Activity",
					activity.ProgramID);
			}

			return await ToEnrichedDtoAsync(activity);
		}

		public async Task<ActivityDto> UpdateActivityAsync(Guid activityId, CreateActivityDto dto)
		{
			var activity = await GetActivityOrThrowAsync(activityId);

			if (activity.Status != StatusType.Active)
				throw new InvalidOperationException(
					"Only scheduled activities can be edited. Cancel and recreate if you need to change a started or completed activity.");

			ValidateModeFields(dto.Mode, dto.Location, dto.VideoUrl);

			activity.Title = dto.Title;
			activity.Description = dto.Description;
			activity.Type = dto.Type;
			activity.Date = dto.Date;
			activity.Duration = dto.Duration;
			activity.MaxParticipants = dto.MaxParticipants;
			activity.Mode = Enum.Parse<ActivityMode>(dto.Mode, true);
			activity.Location = dto.Mode.Equals("Offline", StringComparison.OrdinalIgnoreCase) ? dto.Location : null;
			activity.VideoUrl = dto.Mode.Equals("Online", StringComparison.OrdinalIgnoreCase) ? dto.VideoUrl : null;
			activity.ProgramID = dto.ProgramID;

			if (dto.AssignedCoachID.HasValue && dto.AssignedCoachID != Guid.Empty)
			{
				activity.AssignedCoachID = dto.AssignedCoachID;
				activity.AssignedCoachName = dto.AssignedCoachName;
			}
			activity.UpdatedAt = DateTime.UtcNow;

			await _activityRepo.UpdateAsync(activity);

			// 🔔 Notify enrolled employees about update
			await NotifyEnrolledAsync(activity,
				$"Activity '{activity.Title}' has been updated. Please review the changes.",
				"Activity");

			return await ToEnrichedDtoAsync(activity);
		}

		public async Task<ActivityDto> AssignCoachAsync(Guid activityId, AssignCoachDto dto)
		{
			var activity = await GetActivityOrThrowAsync(activityId);

			if (dto.CoachID.HasValue && dto.CoachID != Guid.Empty)
			{
				activity.AssignedCoachID = dto.CoachID;
				activity.AssignedCoachName = dto.CoachName;
				activity.UpdatedAt = DateTime.UtcNow;
				await _activityRepo.UpdateAsync(activity);

				// 🔔 Notify the coach
				await _notify.NotifyAsync(
					dto.CoachID.Value,
					$"You've been assigned to coach activity: '{activity.Title}'",
					"Activity",
					activity.ProgramID);
			}

			return await ToEnrichedDtoAsync(activity);
		}

		// ─── State transitions (one-liners) ─────────────────────────────────

		public Task<ActivityDto> StartActivityAsync(Guid id) =>
			TransitionAsync(id, StatusType.Pending, "has started.");

		public Task<ActivityDto> CompleteActivityAsync(Guid id) =>
			TransitionAsync(id, StatusType.Completed, "has been completed.");

		public Task<ActivityDto> CancelActivityAsync(Guid id) =>
			TransitionAsync(id, StatusType.Cancelled, "has been cancelled.");

		// ─── Employee enrollment ────────────────────────────────────────────

		public async Task<AttendanceDto> JoinActivityAsync(Guid activityId, Guid employeeId, string employeeName)
		{
			var activity = await GetActivityOrThrowAsync(activityId);

			if (activity.Status == StatusType.Cancelled)
				throw new InvalidOperationException("This activity has been cancelled.");
			if (activity.Status == StatusType.Completed)
				throw new InvalidOperationException("This activity is already completed.");

			var existing = await _activityRepo.GetAttendanceRecordAsync(activityId, employeeId);
			if (existing != null) return _mapper.Map<AttendanceDto>(existing);

			var enrolled = await _activityRepo.CountAttendanceAsync(a => a.ActivityID == activityId);
			if (activity.MaxParticipants > 0 && enrolled >= activity.MaxParticipants)
				throw new InvalidOperationException("This activity is full.");

			var record = new ActivityAttendance
			{
				Id = Guid.NewGuid(),
				ActivityID = activityId,
				EmployeeID = employeeId,
				EmployeeName = employeeName,
				IsPresent = false,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
			await _activityRepo.AddAttendanceAsync(record);

			// 🔔 Confirm to the user
			await _notify.NotifyAsync(
				employeeId,
				$"You joined '{activity.Title}'.",
				"Activity",
				activity.ProgramID);

			return _mapper.Map<AttendanceDto>(record);
		}

		public async Task LeaveActivityAsync(Guid activityId, Guid employeeId)
		{
			var existing = await _activityRepo.GetAttendanceRecordAsync(activityId, employeeId);
			if (existing == null) return;

			await _activityRepo.DeleteAttendanceAsync(existing);

			var activity = await _activityRepo.GetByIdAsync(activityId);
			if (activity != null)
			{
				// 🔔 Confirm to the user
				await _notify.NotifyAsync(
					employeeId,
					$"You left '{activity.Title}'.",
					"Activity",
					activity.ProgramID);
			}
		}

		public async Task<AttendanceDto> MarkWatchedAsync(Guid activityId, Guid employeeId, string employeeName)
		{
			var existing = await _activityRepo.GetAttendanceRecordAsync(activityId, employeeId)
				?? throw new InvalidOperationException("You must join the activity first.");

			existing.IsPresent = true;
			existing.JoinTime ??= DateTime.UtcNow;
			existing.LeaveTime = DateTime.UtcNow;
			existing.UpdatedAt = DateTime.UtcNow;
			await _activityRepo.UpdateAttendanceAsync(existing);
			return _mapper.Map<AttendanceDto>(existing);
		}

		// ─── Coach attendance management ───────────────────────────────────

		public async Task<IEnumerable<AttendanceDto>> GetAttendanceAsync(Guid activityId)
		{
			var records = await _activityRepo.GetAttendanceByActivityIdAsync(activityId);
			return _mapper.Map<IEnumerable<AttendanceDto>>(records);
		}

		public async Task<AttendanceDto> MarkAttendanceAsync(Guid activityId, MarkAttendanceDto dto)
		{
			var activity = await GetActivityOrThrowAsync(activityId);
			EnsureAttendanceEditable(activity);

			var existing = await _activityRepo.GetAttendanceRecordAsync(activityId, dto.EmployeeID);
			if (existing != null)
			{
				_mapper.Map(dto, existing);
				existing.UpdatedAt = DateTime.UtcNow;
				await _activityRepo.UpdateAttendanceAsync(existing);
				return _mapper.Map<AttendanceDto>(existing);
			}

			var record = _mapper.Map<ActivityAttendance>(dto);
			record.Id = Guid.NewGuid();
			record.ActivityID = activityId;
			record.CreatedAt = DateTime.UtcNow;
			await _activityRepo.AddAttendanceAsync(record);
			return _mapper.Map<AttendanceDto>(record);
		}

		public async Task<ActivityDto> FinalizeAttendanceAsync(Guid activityId, FinalizeAttendanceDto dto)
		{
			var activity = await GetActivityOrThrowAsync(activityId);
			EnsureAttendanceEditable(activity);

			foreach (var entry in dto.Entries)
			{
				var row = await _activityRepo.GetAttendanceRecordAsync(activityId, entry.EmployeeID);
				if (row == null) continue;

				row.IsPresent = entry.IsPresent;
				row.UpdatedAt = DateTime.UtcNow;
				if (entry.IsPresent && row.JoinTime == null) row.JoinTime = DateTime.UtcNow;
				await _activityRepo.UpdateAttendanceAsync(row);
			}

			activity.AttendanceFinalized = true;
			activity.UpdatedAt = DateTime.UtcNow;
			await _activityRepo.UpdateAsync(activity);

			return await ToEnrichedDtoAsync(activity);
		}

		// ─── Pagination ─────────────────────────────────────────────────────

		public async Task<PagedResult<ActivityDto>> GetPagedActivitiesAsync(
			int page, int pageSize,
			Guid? programId = null,
			string? search = null,
			string? mode = null,
			string? status = null,
			string? view = null)
		{
			var all = programId.HasValue
				? (await GetActivitiesByProgramIdAsync(programId.Value)).ToList()
				: (await GetAllActivitiesAsync()).ToList();

			var filtered = all.AsEnumerable();

			if (!string.IsNullOrEmpty(search))
			{
				var s = search.ToLowerInvariant();
				filtered = filtered.Where(a =>
					a.Title.ToLowerInvariant().Contains(s) ||
					(a.Description ?? "").ToLowerInvariant().Contains(s) ||
					(a.ProgramName ?? "").ToLowerInvariant().Contains(s));
			}

			if (!string.IsNullOrEmpty(mode))
				filtered = filtered.Where(a => a.Mode == mode);

			if (!string.IsNullOrEmpty(status))
			{
				filtered = status switch
				{
					"active" => filtered.Where(a => a.Status == StatusType.Active),
					"pending" => filtered.Where(a => a.Status == StatusType.Pending),
					"completed" => filtered.Where(a => a.Status == StatusType.Completed),
					"cancelled" => filtered.Where(a => a.Status == StatusType.Cancelled),
					_ => filtered
				};
			}

			if (!string.IsNullOrEmpty(view))
			{
				var today = DateTime.UtcNow.Date;
				filtered = view switch
				{
					"today" => filtered.Where(a => a.Date.Date == today),
					"upcoming" => filtered.Where(a => a.Date.Date > today && a.Status != StatusType.Completed && a.Status != StatusType.Cancelled),
					"completed" => filtered.Where(a => a.Status == StatusType.Completed),
					_ => filtered
				};
			}

			var list = filtered.ToList();
			var total = list.Count;
			var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PagedResult<ActivityDto>
			{
				Items = items,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}

		// ─── Private helpers ────────────────────────────────────────────────

		/// <summary>Common: fetch activity or throw if missing.</summary>
		private async Task<Activity> GetActivityOrThrowAsync(Guid activityId)
		{
			return await _activityRepo.GetByIdAsync(activityId)
				?? throw new KeyNotFoundException($"Activity {activityId} not found.");
		}

		/// <summary>Common: map activity to DTO and enrich it.</summary>
		private async Task<ActivityDto> ToEnrichedDtoAsync(Activity activity)
		{
			var dto = _mapper.Map<ActivityDto>(activity);
			await EnrichAsync(new[] { dto });
			return dto;
		}

		/// <summary>Shared logic for Start/Complete/Cancel state changes.</summary>
		private async Task<ActivityDto> TransitionAsync(
			Guid activityId, StatusType newStatus, string verbMessage)
		{
			var activity = await GetActivityOrThrowAsync(activityId);
			EnsureTransition(activity.Status, newStatus);

			activity.Status = newStatus;
			activity.UpdatedAt = DateTime.UtcNow;
			await _activityRepo.UpdateAsync(activity);

			await NotifyEnrolledAsync(activity,
				$"Activity '{activity.Title}' {verbMessage}",
				"Activity");

			return await ToEnrichedDtoAsync(activity);
		}

		/// <summary>Notifies all enrolled employees of an activity event.</summary>
		private async Task NotifyEnrolledAsync(Activity activity, string message, string category)
		{
			var rows = await _activityRepo.GetAttendanceByActivityIdAsync(activity.Id);
			var employeeIds = rows.Select(r => r.EmployeeID).ToList();

			if (employeeIds.Any())
			{
				await _notify.NotifyManyAsync(
					employeeIds,
					message,
					category,
					activity.ProgramID);
			}
		}

		private static void EnsureAttendanceEditable(Activity activity)
		{
			if (activity.AttendanceFinalized)
				throw new InvalidOperationException("Attendance has been finalized for this activity. No further edits.");
			if (activity.Status == StatusType.Active)
				throw new InvalidOperationException("Start the activity before taking attendance.");
			if (activity.Status == StatusType.Cancelled)
				throw new InvalidOperationException("Activity is cancelled.");
			if (activity.Status == StatusType.Completed)
				throw new InvalidOperationException("Activity is already completed.");
		}

		private static void EnsureTransition(StatusType from, StatusType to)
		{
			bool ok = (from, to) switch
			{
				(StatusType.Active, StatusType.Pending) => true,
				(StatusType.Active, StatusType.Completed) => true,
				(StatusType.Active, StatusType.Cancelled) => true,
				(StatusType.Pending, StatusType.Completed) => true,
				(StatusType.Pending, StatusType.Cancelled) => true,
				_ => false
			};
			if (!ok)
				throw new InvalidOperationException(
					$"Cannot move activity from {from} to {to}. The current state has changed — please refresh and try again.");
		}

		private static void ValidateModeFields(string mode, string? location, string? videoUrl)
		{
			if (!Enum.TryParse<ActivityMode>(mode, true, out var parsed))
				throw new ArgumentException($"Invalid activity mode: {mode}");

			if (parsed == ActivityMode.Offline && string.IsNullOrWhiteSpace(location))
				throw new ArgumentException("Location is required for an offline activity.");
			if (parsed == ActivityMode.Online && string.IsNullOrWhiteSpace(videoUrl))
				throw new ArgumentException("Video URL is required for an online activity.");
		}

		private async Task EnrichAsync(IEnumerable<ActivityDto> dtos)
		{
			var list = dtos.ToList();
			if (list.Count == 0) return;

			var programIds = list.Select(d => d.ProgramID).Where(p => p != Guid.Empty).Distinct().ToList();
			var programs = programIds.Count == 0
				? Enumerable.Empty<programs>()
				: await _programRepo.FindAsync(p => programIds.Contains(p.Id));
			var programMap = programs.ToDictionary(p => p.Id, p => p.Title);

			var currentUserId = _currentUser.UserId;

			foreach (var dto in list)
			{
				dto.ProgramName = programMap.TryGetValue(dto.ProgramID, out var pn) ? pn : string.Empty;
				dto.EnrolledCount = await _activityRepo.CountAttendanceAsync(a => a.ActivityID == dto.Id);
				dto.AttendedCount = await _activityRepo.CountAttendanceAsync(a => a.ActivityID == dto.Id && a.IsPresent);

				if (currentUserId != Guid.Empty)
				{
					var myRow = await _activityRepo.GetAttendanceRecordAsync(dto.Id, currentUserId);
					dto.IsEnrolled = myRow != null;
					dto.HasAttended = myRow?.IsPresent ?? false;
				}
			}
		}
	}
}