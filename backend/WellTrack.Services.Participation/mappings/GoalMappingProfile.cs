using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Participation.Domain.Entities;
using WellTrack.Services.Participation.Application.DTOs;

namespace WellTrack.Services.Participation.mappings
{
	public class GoalMappingProfile : AutoMapper.Profile
	{
		
		public GoalMappingProfile() {
			CreateMap<Goal, GoalDto>()
			 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

			CreateMap<CreateGoalDto, Goal>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CurrentValue, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

			CreateMap<UpdateGoalDto, Goal>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.EmployeeID, opt => opt.Ignore())
				.ForMember(dest => dest.StartDate, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

		}
	}
}
