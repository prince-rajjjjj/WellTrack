using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Application.DTOs;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Participation.mappings
{
	public class ParticipationMappingProfile : AutoMapper.Profile
	{
	public ParticipationMappingProfile() {
			// Participation mappings
			CreateMap<ParticipationEntity, ParticipationDto>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

			CreateMap<CreateParticipationDto, ParticipationEntity>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());


			CreateMap<JoinActivityRequest, ParticipationEntity>()
		.ForMember(dest => dest.Id,            opt => opt.MapFrom(_ => Guid.NewGuid()))
		.ForMember(dest => dest.Date,          opt => opt.MapFrom(_ => DateTime.UtcNow))
		.ForMember(dest => dest.Status,        opt => opt.MapFrom(_ => StatusType.Active))
		.ForMember(dest => dest.Notes,         opt => opt.MapFrom(_ => "Self-joined activity"))
		.ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.ActivityTitle))
		.ForMember(dest => dest.CreatedAt,     opt => opt.MapFrom(_ => DateTime.UtcNow))
		.ForMember(dest => dest.UpdatedAt,     opt => opt.MapFrom(_ => DateTime.UtcNow))
		.ForMember(dest => dest.EmployeeID,    opt => opt.Ignore())
		.ForMember(dest => dest.CreatedBy,     opt => opt.Ignore())
		.ForMember(dest => dest.UpdatedBy,     opt => opt.Ignore());

		}
	}
}
