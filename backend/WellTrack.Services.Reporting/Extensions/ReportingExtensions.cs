using Microsoft.Extensions.DependencyInjection;
using WellTrack.Services.Reporting.Application.Services;

namespace WellTrack.Services.Reporting.Extensions;

public static class ReportingExtensions
{
	public static IServiceCollection AddReportingModule(this IServiceCollection services)
	{
		services.AddScoped<IReportingService, ReportingService>();
		return services;
	}
}