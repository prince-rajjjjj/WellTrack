using Microsoft.Extensions.DependencyInjection;
using WellTrack.Services.Compliance.Application.Services;
using WellTrack.Services.Compliance.Repositories;
using WellTrack.Services.Compliance.Repository;

namespace WellTrack.Services.Compliance.Extensions;

public static class ComplianceExtensions
{
	public static IServiceCollection AddComplianceModule(this IServiceCollection services)
	{
		services.AddScoped<IAuditService, AuditService>();
		services.AddScoped<IPolicyService, PolicyService>();
		services.AddScoped<IComplianceService, ComplianceService>();
		services.AddScoped<IAuditRepository, AuditRepository>();
		services.AddScoped<IPolicyRepository, PolicyRepository>();
		services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
		return services;
	}
}