using WellTrack.Services.Engagement.Application.Services;
using WellTrack.Services.Engagement.Repositories;

namespace WellTrack.Services.Engagement.Extensions;

public static class EngagementExtensions
{
	public static IServiceCollection AddEngagementModule(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// ── HTTP Clients ──────────────────────────────────────────────────
		// Mirrors the Participation → Notification pattern already in the project.
		// JWT is forwarded per-request inside EngagementCalculationService.
		services.AddHttpClient("WellnessClient", c =>
			c.BaseAddress = new Uri(configuration["ServiceUrls:Wellness"]!));

		services.AddHttpClient("ParticipationClient", c =>
			c.BaseAddress = new Uri(configuration["ServiceUrls:Participation"]!));

		// ── Business Logic ────────────────────────────────────────────────
		services.AddScoped<IEngagementCalculationService, EngagementCalculationService>();
		services.AddScoped<IEngagementService, EngagementService>();
		services.AddScoped<IFeedbackService, FeedbackService>();

		// ── Repositories ──────────────────────────────────────────────────
		services.AddScoped<IEngagementRepository, EngagementRepository>();
		services.AddScoped<IFeedbackRepository, FeedbackRepository>();

		return services;
	}
}
