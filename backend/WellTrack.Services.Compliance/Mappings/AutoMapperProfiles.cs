using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using WellTrack.Services.Compliance.Application.DTOs;
using WellTrack.Services.Compliance.Domain.Entities;

namespace WellTrack.Services.Compliance.Mappings
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			// Direction 1: Entity to Dto (For GET requests)
			// Map the REAL audit status (AuditStatus enum) to the DTO Status string,
			// not the legacy StatusType.Status field which always defaults to "Pending".
			CreateMap<Audit, AuditDto>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.AuditStatus.ToString()))
				.ReverseMap();

			// Direction 2: CreateDto to Entity (For POST requests)
			CreateMap<CreateAuditDto, Audit>()
				//.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.AuditStatus, opt => opt.MapFrom(src => AuditStatus.Draft))
				.ReverseMap();

			// Direction 3: UpdateDto to Entity (The one you are missing!)
			CreateMap<UpdateAuditDto, Audit>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


			// -------- this is policy mapping --------
			CreateMap<Policy, PolicyDto>().ReverseMap();
			CreateMap<CreatePolicyDto, Policy>()
				.ForMember(dest => dest.Status,      opt => opt.MapFrom(_ => PolicyStatus.Active))
				.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.CreatedAt,   opt => opt.MapFrom(_ => DateTime.UtcNow));
			CreateMap<UpdatePolicyDto, Policy>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


			// -------- this is compliance record mapping --------

			// Entity -> DTO (Used for Get/GetAll/Create Response)
			CreateMap<ComplianceRecord, ComplianceRecordDto>()
				.ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

			// DTO -> Entity (Used for Create)
			CreateMap<CreateComplianceRecordDto, ComplianceRecord>()
				.ForMember(dest => dest.Severity, opt => opt.MapFrom(src =>
					Enum.Parse<ComplianceSeverity>(src.Severity, true)))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => ComplianceRecordStatus.Reported))
				.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));


		}
	}
}
