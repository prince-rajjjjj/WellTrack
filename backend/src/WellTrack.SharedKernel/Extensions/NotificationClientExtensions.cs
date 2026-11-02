using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WellTrack.SharedKernel.HttpClients;

namespace WellTrack.SharedKernel.Extensions;

public static class NotificationClientExtensions
{
	public static IServiceCollection AddNotificationClient(this IServiceCollection services,IConfiguration configuration)
	{
		services.AddHttpContextAccessor();

		var baseUrl = configuration["Services:Notification"] ?? "http://localhost:5007";

		services.AddHttpClient<INotificationClient, NotificationClient>(client =>
		{
			client.BaseAddress = new Uri(baseUrl);
			client.Timeout = TimeSpan.FromSeconds(5);
		});

		return services;
	}

	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHttpContextAccessor();
		var baseUrl = configuration["Services:Identity"] ?? "http://localhost:5001";
		services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
		{
			client.BaseAddress = new Uri(baseUrl);
			client.Timeout = TimeSpan.FromSeconds(5);
		});
		return services;
	}
}