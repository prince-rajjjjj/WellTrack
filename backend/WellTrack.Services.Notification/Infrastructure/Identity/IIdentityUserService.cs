namespace WellTrack.Services.Notification.Infrastructure.Identity;

/// <summary>
/// Minimal user contract needed by NotificationService — resolved via HTTP call to the Identity service.
/// </summary>
public interface IIdentityUserService
{
    Task<IdentityUserDto?> GetUserByIdAsync(Guid userId);
}

public class IdentityUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
