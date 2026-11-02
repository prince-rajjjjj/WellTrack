using Microsoft.Extensions.DependencyInjection;
using WellTrack.Services.Wellness.Application.Service;
using WellTrack.Services.Wellness.Application.Services;
using WellTrack.Services.Wellness.Repositories;

namespace WellTrack.Services.Wellness.Extensions;

public static class WellnessExtensions
{
	public static IServiceCollection AddWellnessModule(this IServiceCollection services)
	{
		services.AddScoped<IProgramService, ProgramService>();
		services.AddScoped<IActivityService, ActivityService>();
		services.AddScoped<IProgramRepository, ProgramRepository>();
		services.AddScoped<IActivityRepository, ActivityRepository>();
		services.AddHttpClient();
		return services;
	}
}
