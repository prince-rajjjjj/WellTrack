using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Identity.Application.Services;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.SharedKernel.Wrappers;
using WellTrack.SharedKernel.Interfaces;

namespace WellTrack.Services.Identity.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public class AdminController : ControllerBase
{
	private readonly IUserService userService;
	private readonly ILogAuditService logAuditService;
	private readonly ICurrentUserService currentUserService;

	public AdminController(IUserService userService, ILogAuditService auditService, ICurrentUserService currentUserService)
	{
		this.userService = userService;
		this.logAuditService = auditService;
		this.currentUserService = currentUserService;
	}

	[HttpGet("audit-logs")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<ApiResponse<PagedResult<AuditLogDto>>>> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
	{
		var result = await logAuditService.GetPagedLogsAsync(page, pageSize, search);
		return Ok(new ApiResponse<PagedResult<AuditLogDto>>(result));
	}
}
