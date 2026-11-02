using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Application.Services;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Controllers
{
	[ApiController]
	[Route("api/v1/employees")]
	[Authorize(Roles = "Admin,HR")]
	public class EmployeesController:ControllerBase
	{
		private readonly IUserService userService;

		public EmployeesController(IUserService userService)
		{
			this.userService = userService;
		}

		[HttpGet]
		public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetEmployees([FromQuery] int page = 1,[FromQuery] int pageSize=10){
			var users = await userService.GetUsersAsync(page, pageSize, "Employee");

			return Ok(new ApiResponse<PagedResult<UserDto>>(users));
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<UserDto>>> GetEmployee([FromRoute] Guid id){
			var user = await userService.GetUserByIdAsync(id);

			return Ok(new ApiResponse<UserDto>(user));
		}

		


	}
}
