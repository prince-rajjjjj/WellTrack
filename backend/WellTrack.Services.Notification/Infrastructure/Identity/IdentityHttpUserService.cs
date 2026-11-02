using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Notification.Infrastructure.Identity;

/// <summary>
/// Calls Identity service (GET /api/v1/users/{id}) to fetch user details.
/// Forwards the caller's JWT so the Identity service accepts the request.
/// </summary>
public class IdentityHttpUserService : IIdentityUserService
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdentityHttpUserService> _logger;

    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public IdentityHttpUserService(
        IHttpClientFactory factory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<IdentityHttpUserService> logger)
    {
        _factory             = factory;
        _httpContextAccessor = httpContextAccessor;
        _logger              = logger;
    }

    public async Task<IdentityUserDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var client = _factory.CreateClient("IdentityClient");
            var auth   = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(auth))
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);

            var result = await client.GetFromJsonAsync<ApiResponse<IdentityUserDto>>(
                $"/api/v1/users/{userId}", JsonOpts);

            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch user {UserId} from Identity service", userId);
            return null;
        }
    }
}
