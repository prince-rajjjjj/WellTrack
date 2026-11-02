using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Application.Services;
using WellTrack.SharedKernel.Interfaces;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Engagement.Controllers
{
    [ApiController]
    [Route("api/v1/engagement")]
    [Authorize]
    public class EngagementController : ControllerBase
    {
        private readonly IEngagementService _engagementService;
        private readonly ICurrentUserService _currentUserService;

        public EngagementController(
            IEngagementService engagementService,
            ICurrentUserService currentUserService)
        {
            _engagementService  = engagementService;
            _currentUserService = currentUserService;
        }

        // ════════════════════════════════════════════════════════════
        //  HR Dashboard — new analytics endpoints
        // ════════════════════════════════════════════════════════════

        /// <summary>Four KPI cards: total tracked, org avg, % high, % at-risk.</summary>
        [HttpGet("summary/overall")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<OrgSummaryDto>>> GetOrgSummary()
        {
            var summary = await _engagementService.GetOrgSummaryAsync();
            return Ok(new ApiResponse<OrgSummaryDto>(summary));
        }

        /// <summary>Top-N employees by avg score across ALL programs.</summary>
        [HttpGet("leaderboard/overall")]
        [Authorize(Roles = "HR,Admin,Coach")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OverallLeaderboardEntryDto>>>> GetOverallLeaderboard(
            [FromQuery] int top = 10)
        {
            var result = await _engagementService.GetOverallLeaderboardAsync(top);
            return Ok(new ApiResponse<IEnumerable<OverallLeaderboardEntryDto>>(result));
        }

        /// <summary>Top-N employees with the biggest positive score delta since last calculation.</summary>
        [HttpGet("leaderboard/most-improved")]
        [Authorize(Roles = "HR,Admin,Coach")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MostImprovedEntryDto>>>> GetMostImproved(
            [FromQuery] int top = 10)
        {
            var result = await _engagementService.GetMostImprovedAsync(top);
            return Ok(new ApiResponse<IEnumerable<MostImprovedEntryDto>>(result));
        }

        /// <summary>Org-wide avg per factor as % of max — shows WHERE engagement is weak.</summary>
        [HttpGet("factor-health")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<FactorHealthDto>>> GetFactorHealth()
        {
            var result = await _engagementService.GetFactorHealthAsync();
            return Ok(new ApiResponse<FactorHealthDto>(result));
        }

        /// <summary>Per-program High/Medium/Low split, avg score and trend.</summary>
        [HttpGet("program-health")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProgramHealthDto>>>> GetProgramHealth()
        {
            var result = await _engagementService.GetProgramHealthAsync();
            return Ok(new ApiResponse<IEnumerable<ProgramHealthDto>>(result));
        }

        // ════════════════════════════════════════════════════════════
        //  HR / Admin — raw record access
        // ════════════════════════════════════════════════════════════

        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<HrEngagementDto>>>> GetAll()
        {
            var records = await _engagementService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<HrEngagementDto>>(records));
        }

        [HttpGet("employee/{employeeId:guid}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<HrEngagementDto>>>> GetByEmployee(Guid employeeId)
        {
            var records = await _engagementService.GetByEmployeeIdAsync(employeeId);
            return Ok(new ApiResponse<IEnumerable<HrEngagementDto>>(records));
        }

        [HttpGet("program/{programId:guid}")]
        [Authorize(Roles = "HR,Admin,Coach")]
        public async Task<ActionResult<ApiResponse<IEnumerable<HrEngagementDto>>>> GetByProgram(Guid programId)
        {
            var records = await _engagementService.GetByProgramIdAsync(programId);
            return Ok(new ApiResponse<IEnumerable<HrEngagementDto>>(records));
        }

        // ════════════════════════════════════════════════════════════
        //  Employee — personal view
        // ════════════════════════════════════════════════════════════

        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeEngagementDto>>>> GetMine()
        {
            var records = await _engagementService.GetMyEngagementAsync(_currentUserService.UserId);
            return Ok(new ApiResponse<IEnumerable<EmployeeEngagementDto>>(records));
        }

        // ════════════════════════════════════════════════════════════
        //  Recalculate
        // ════════════════════════════════════════════════════════════

        /// <summary>
        /// Per-row recalculate — one employee + one program.
        /// Also called by Wellness/Participation services after relevant events
        /// (FinalizeAttendance forwards the Coach JWT, so Coach must be allowed).
        /// </summary>
        [HttpPost("recalculate")]
        [Authorize(Roles = "HR,Admin,Coach")]
        public async Task<ActionResult<ApiResponse<HrEngagementDto>>> Recalculate(
            [FromBody] RecalculateEngagementDto dto)
        {
            var jwt    = Request.Headers.Authorization.ToString();
            var result = await _engagementService.RecalculateAsync(dto.EmployeeId, dto.ProgramId, jwt);
            return Ok(new ApiResponse<HrEngagementDto>(result, "Engagement score recalculated successfully."));
        }

        /// <summary>
        /// Bulk "Sync Scores" — recalculates every existing record.
        /// Runs sequentially to avoid hammering downstream services.
        /// Returns count of records updated.
        /// </summary>
        [HttpPost("recalculate/all")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<object>>> RecalculateAll()
        {
            var jwt     = Request.Headers.Authorization.ToString();
            var updated = await _engagementService.RecalculateAllAsync(jwt);
            return Ok(new ApiResponse<object>(
                new { updated },
                $"{updated} engagement record(s) recalculated successfully."));
        }

        // ════════════════════════════════════════════════════════════
        //  Legacy — kept for backwards compat
        // ════════════════════════════════════════════════════════════

        [HttpGet("program/{programId:guid}/summary")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<ApiResponse<EngagementSummaryDto>>> GetProgramSummary(Guid programId)
        {
            var summary = await _engagementService.GetProgramSummaryAsync(programId);
            return Ok(new ApiResponse<EngagementSummaryDto>(summary));
        }

        [HttpGet("program/{programId:guid}/leaderboard")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetLeaderboard(
            Guid programId, [FromQuery] int top = 10)
        {
            var leaderboard = await _engagementService.GetLeaderboardAsync(programId, top);
            return Ok(new ApiResponse<IEnumerable<object>>(leaderboard));
        }
    }
}
