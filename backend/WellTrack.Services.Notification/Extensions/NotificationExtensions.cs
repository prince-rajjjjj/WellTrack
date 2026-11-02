using WellTrack.Services.Notification.Application.Services;
using WellTrack.Services.Notification.Repositories;
using WellTrack.Services.Notification.Infrastructure.Identity;

namespace WellTrack.Services.Notification.Extensions;

public static class NotificationExtensions
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IIdentityUserService, IdentityHttpUserService>();
        return services;
    }
}
