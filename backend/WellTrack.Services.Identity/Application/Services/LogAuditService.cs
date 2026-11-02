using AutoMapper;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.Services.Identity.Repositories;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Application.Services
{
	public class LogAuditService : ILogAuditService
	{
		private readonly IAuditLogRepository auditLogRepository;
		private readonly IMapper mapper;

		public LogAuditService(IAuditLogRepository auditLogRepository, IMapper mapper)
		{
			this.auditLogRepository = auditLogRepository;
			this.mapper = mapper;
		}

		public async Task CreateLogAsync(Guid id,string action,string resource){
			var auditLog = new AuditLog()
			{
				UserId = id,
				Action = action,
				Resource = resource,
				Timestamp = DateTime.UtcNow
			};

			await auditLogRepository.AddAsync(auditLog);
		}

		public async Task<PagedResult<AuditLogDto>> GetPagedLogsAsync(int page, int pageSize, string? search)
		{
			var pagedEntities = await auditLogRepository.GetPagedLogsAsync(page, pageSize, search);

			var dtos = mapper.Map<List<AuditLogDto>>(pagedEntities.Items);

			return new PagedResult<AuditLogDto>
			{
				Items = dtos,
				TotalCount = pagedEntities.TotalCount,
				Page = page,
				PageSize = pageSize
			};
		}

	}
}
