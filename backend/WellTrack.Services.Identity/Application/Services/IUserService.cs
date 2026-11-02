using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Application.Services
{
	public interface IUserService
	{
		Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

		Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);

		Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileDto dto);

		Task ChangePasswordAsync(Guid id, ChangePasswordDto dto);

		Task DeleteUserAsync(Guid id);

		Task DeactivateUserAsync(Guid id);

		Task ActiveUserAsync(Guid id);

		Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? role);

		Task<UserDto> GetUserByIdAsync(Guid id);

	}
}