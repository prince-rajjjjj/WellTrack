using Microsoft.Extensions.DependencyInjection;
using WellTrack.Services.Participation.Application;
using WellTrack.Services.Participation.Application.Interfaces;
using WellTrack.Services.Participation.Application.Services;
using WellTrack.Services.Participation.Repository;

namespace WellTrack.Api.Extensions;

public static class ParticipationExtensions
{
	public static IServiceCollection AddParticipationModules(this IServiceCollection services)
	{
		// Services
		services.AddScoped<IParticipationService, ParticipationService>();
		services.AddScoped<IGoalTrackingService, GoalTrackingService>();

		// Repositories
		services.AddScoped<IParticipationRepository, SQLParticipationRepository>();
		services.AddScoped<IGoalRepository, SQLGoalRepository>();

		return services;
	}
}