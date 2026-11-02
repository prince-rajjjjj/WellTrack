using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Notification.Application.DTOs;
using WellTrack.Services.Notification.Domain.Entities;
using WellTrack.Services.Notification.Infrastructure.Identity;
using WellTrack.Services.Notification.Repositories;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Exceptions;

namespace WellTrack.Services.Notification.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly INotificationRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly IIdentityUserService _identityUserService;

    public NotificationService(
        IMapper mapper,
        INotificationRepository repository,
        IConfiguration configuration,
        IIdentityUserService identityUserService)
    {
        _mapper              = mapper;
        _repository          = repository;
        _configuration       = configuration;
        _identityUserService = identityUserService;
    }

	public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto)
	{
		var entity = _mapper.Map<NotificationEntity>(createDto);
		entity.CreatedAt = DateTime.UtcNow;        // ← add this line
		entity.Status = StatusType.Active;      // ← also good practice
		entity = await _repository.AddAsync(entity);
		return _mapper.Map<NotificationDto>(entity);
	}

	public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(NotificationEntity), id);
        await _repository.DeleteAsync(entity);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<NotificationDto>>(entities);
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var entities = await _repository.FindAsync(n => n.UserID == userId)
            ?? throw new NotFoundException(nameof(NotificationEntity), userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(entities);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(NotificationEntity), id);

        entity.Status    = StatusType.Inactive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
    }

	public async Task<int> MarkAllAsReadAsync(Guid userId)
	{
		return await _repository.MarkAllAsReadAsync(userId);
	}
}
