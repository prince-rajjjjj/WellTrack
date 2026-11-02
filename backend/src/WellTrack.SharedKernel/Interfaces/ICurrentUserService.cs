namespace WellTrack.SharedKernel.Interfaces;

public interface ICurrentUserService
{
	Guid UserId { get; }
	string? Name { get; }
	string? Email { get; }
	string? Role { get; }
	bool IsAuthenticated { get; }
}
