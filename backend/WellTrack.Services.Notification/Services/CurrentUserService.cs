using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Notification.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }

    public string? Name  => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? Role  => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
