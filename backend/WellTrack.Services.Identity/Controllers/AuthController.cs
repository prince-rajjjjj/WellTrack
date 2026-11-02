using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Application.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Authorize]
public class AuthController : ControllerBase
{
	private readonly IAuthService authService;
	private readonly ICurrentUserService currentUserService;

	public AuthController(IAuthService authService,ICurrentUserService currentUserService)
	{
		this.authService = authService;
		this.currentUserService = currentUserService;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
	{
		var result = await authService.LoginAsync(loginDto);
		return Ok(new ApiResponse<LoginResponseDto>(result, "Login successful"));
	}

	[HttpPost("logout")]
	public async Task<ActionResult<ApiResponse<object>>> Logout(){
		var userId = currentUserService.UserId;

		await authService.LogoutAsync(userId);

		return new ApiResponse<object>("Logout Successfully");
	}

	[HttpGet("profile")]
	public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
	{
		var userId = currentUserService.UserId;
		var name = currentUserService.Name;
		var result = await authService.GetProfileAsync(userId);
		result.Name = name;
		return Ok(new ApiResponse<UserDto>(result));
	}

	[HttpPut("change-password")]
	public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
	{
		var userId = currentUserService.UserId;

		await authService.ChangePasswordAsync(userId, changePasswordDto);

		return Ok(new ApiResponse<object>("Password changed successfully"));
	}

	
}

