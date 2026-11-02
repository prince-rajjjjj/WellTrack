using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Wellness.Services;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		this.httpContextAccessor = httpContextAccessor;
	}

	public Guid UserId
	{
		get
		{
			var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
					 ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

			return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
		}
	}

	public string? Name => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

	public string? Email => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

	public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

	public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
