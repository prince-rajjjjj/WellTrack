using AutoMapper;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Domain.Entities;

namespace WellTrack.Services.Engagement.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// ── Engagement ────────────────────────────────────────────────
			// Entity → HR DTO (full breakdown for HR/Admin views)
			CreateMap<EngagementEntity, HrEngagementDto>();

			// ── Feedback ──────────────────────────────────────────────────
			CreateMap<Feedback, FeedbackDto>();

			CreateMap<CreateFeedbackDto, Feedback>()
				.ForMember(dest => dest.Id,            opt => opt.Ignore())
				.ForMember(dest => dest.Date,          opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.CreatedAt,     opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt,     opt => opt.Ignore())
				.ForMember(dest => dest.ActivityID,    opt => opt.MapFrom(src => src.ActivityId))
				.ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.ActivityTitle));

			CreateMap<UpdateFeedbackDto, Feedback>()
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				// Only overwrite non-null values so partial updates work correctly
				.ForAllMembers(opt => opt.Condition((_, _, srcMember) => srcMember != null));
		}
	}
}
