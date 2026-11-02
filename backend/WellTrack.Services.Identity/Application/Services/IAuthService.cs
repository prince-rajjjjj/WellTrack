using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Application.Services
{
	public interface IAuthService
	{
		Task<LoginResponseDto> LoginAsync(LoginDto loginDto);

		Task LogoutAsync(Guid id);

		Task<UserDto> GetProfileAsync(Guid userId);

		Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
	}
}
