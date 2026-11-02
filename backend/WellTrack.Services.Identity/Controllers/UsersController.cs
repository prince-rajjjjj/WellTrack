using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Application.Services;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
	private readonly IUserService userService;
	private readonly ICurrentUserService currentUserService;

	public UsersController(IUserService userService,ICurrentUserService currentUserService)
	{
		this.userService = userService;
		this.currentUserService = currentUserService;
	}

	
	[HttpPatch("me")]
	public async Task<ActionResult<ApiResponse<UserDto>>> UpdateMe([FromBody] UpdateProfileDto dto)
	{

		var user = await userService.UpdateProfileAsync(currentUserService.UserId, dto);
		return Ok(new ApiResponse<UserDto>(user, "Profile updated successfully"));
	}

	
	[HttpGet("me")]
	public async Task<ActionResult<ApiResponse<UserDto>>> GetMe()
	{
		var user = await userService.GetUserByIdAsync(currentUserService.UserId);
		return Ok(new ApiResponse<UserDto>(user));
	}


	
	[HttpPost("me/change-password")]
	public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
	{
		await userService.ChangePasswordAsync(currentUserService.UserId, dto);

		return Ok(new ApiResponse<object>("Password changed successfully"));
	}


	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers([FromQuery] int page = 1,[FromQuery] int pageSize = 10,[FromQuery] string? role = null)
	{

		var users = await userService.GetUsersAsync(page, pageSize, role);

		return Ok(new ApiResponse<PagedResult<UserDto>>(users));
	}


	// Lightweight, role-restricted lookup that HR / Admin use to populate
	// "Assign Coach" dropdowns. Returns only Active users with the Coach role,
	// so HR doesn't gain visibility into the full user table.
	[HttpGet("coaches")]
	[Authorize(Roles = "Admin,HR")]
	public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetCoaches()
	{
		// Page through "Coach" filter — pageSize 1000 covers any realistic org size.
		var paged = await userService.GetUsersAsync(1, 1000, "Coach");
		var activeCoaches = paged.Items
			.Where(u => string.Equals(u.Status, "Active", StringComparison.OrdinalIgnoreCase))
			.ToList();
		return Ok(new ApiResponse<List<UserDto>>(activeCoaches));
	}


	[HttpGet("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<UserDto>>> GetUser([FromRoute] Guid id)
	{

		var user = await userService.GetUserByIdAsync(id);

		return Ok(new ApiResponse<UserDto>(user));
	}


	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
	{

		var user = await userService.CreateUserAsync(createUserDto);

		return Ok(new ApiResponse<UserDto>(user, "User created successfully"));
	}


	[HttpPut("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser([FromRoute]Guid id,[FromBody] UpdateUserDto updateUserDto)
	{
		var user = await userService.UpdateUserAsync(id, updateUserDto);
		return Ok(new ApiResponse<UserDto>(user, "User updated successfully"));
	}


	[HttpPatch("{id}/deactivate")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<object>>> DeactivateUser([FromRoute] Guid id)
	{

		await userService.DeactivateUserAsync(id);
		return Ok(new ApiResponse<object>("User deactivated successfully"));
	}


	[HttpPatch("{id}/activate")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<object>>> ActivateUser([FromRoute] Guid id)
	{
		await userService.ActiveUserAsync(id);

		return Ok(new ApiResponse<object>("User Activated successfully"));

	}


	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<object>>> DeleteUser([FromRoute] Guid id)
	{

		await userService.DeleteUserAsync(id);

		return Ok(new ApiResponse<object>("User deleted successfully"));
	}


	[HttpGet("ids-by-role")]
	[Authorize]
	public async Task<IActionResult> GetUserIdsByRole([FromQuery] string role)
	{
		if (!Enum.TryParse<UserRole>(role, out var roleEnum))
			return BadRequest("Invalid role");

		var users = await userService.GetUsersAsync(1, 10000, role);
		var ids = users.Items.Select(u => u.Id).ToList();
		return Ok(new ApiResponse<List<Guid>>(ids));
	}
}
