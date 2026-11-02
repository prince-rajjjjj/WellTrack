using Microsoft.Extensions.DependencyInjection;
using WellTrack.Services.Identity.Application.Services;
using WellTrack.Services.Identity.Repositories;

namespace WellTrack.Services.Identity.Extensions;

public static class IdentityExtensions
{
	public static IServiceCollection AddIdentityModule(this IServiceCollection services)
	{
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ILogAuditService, LogAuditService>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IAuditLogRepository, AuditLogRepository>();
		return services;
	}
}