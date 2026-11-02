using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WellTrack.Services.Participation.Extensions;

public static class JwtExtensions
{
	public static IServiceCollection AddJwtAuthentication(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var key = configuration["Jwt:Key"]
			?? throw new InvalidOperationException("Jwt:Key not configured");
		var issuer = configuration["Jwt:Issuer"] ?? "WellTrack";
		var audience = configuration["Jwt:Audience"] ?? "WellTrackUsers";

		services
			.AddAuthentication(o =>
			{
				o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(o =>
			{
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = issuer,
					ValidAudience = audience,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(key))
				};
			});

		services.AddAuthorization();
		return services;
	}
}
