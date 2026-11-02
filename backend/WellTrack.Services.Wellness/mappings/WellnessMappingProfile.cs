using System;
using AutoMapper;
using WellTrack.Services.Wellness.Application.DTOs;
using WellTrack.Services.Wellness.Domain.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Wellness.mappings
{
	public class WellnessMappingProfile : Profile
	{
		public WellnessMappingProfile()
		{
			// --- Program ---
			CreateMap<programs, ProgramDto>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

			CreateMap<CreatePragramDto, programs>();
			CreateMap<UpdateProgramDto, programs>();

			// --- Activity ---
			CreateMap<Activity, ActivityDto>()
				.ForMember(dest => dest.Mode,        opt => opt.MapFrom(src => src.Mode.ToString()))
				.ForMember(dest => dest.ProgramName, opt => opt.Ignore())   // populated by service
				.ForMember(dest => dest.EnrolledCount, opt => opt.Ignore())
				.ForMember(dest => dest.AttendedCount, opt => opt.Ignore());

			CreateMap<CreateActivityDto, Activity>()
				.ForMember(dest => dest.Id,        opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.Mode,      opt => opt.MapFrom(src => Enum.Parse<ActivityMode>(src.Mode, true)))
				.ForMember(dest => dest.Status,    opt => opt.NullSubstitute(StatusType.Active))
				.ForMember(dest => dest.Program,   opt => opt.Ignore());

			// --- Attendance ---
			CreateMap<ActivityAttendance, AttendanceDto>();
			CreateMap<MarkAttendanceDto, ActivityAttendance>()
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

		}
	}
}
