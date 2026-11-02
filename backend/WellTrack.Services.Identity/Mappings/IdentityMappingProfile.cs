using AutoMapper;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Domain.Entities;

namespace WellTrack.Services.Identity.Mappings
{
	public class IdentityMappingProfile:Profile
	{
		public IdentityMappingProfile(){

			CreateMap<User, UserDto>()
			.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

			CreateMap<CreateUserDto, User>()
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

			//tells that when u are change dto to user then dont change the below values
			CreateMap<UpdateUserDto, User>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Email, opt => opt.Ignore())
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

			CreateMap<AuditLog, AuditLogDto>();
		}
	}
}
