using WellTrack.Services.Engagement.Application.DTOs;

namespace WellTrack.Services.Engagement.Application.Services
{
    public interface IEngagementService
    {
        // ── HR / Admin — raw records ──────────────────────────────────────
        Task<IEnumerable<HrEngagementDto>> GetAllAsync();
        Task<IEnumerable<HrEngagementDto>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<HrEngagementDto>> GetByProgramIdAsync(Guid programId);

        // ── HR Dashboard analytics ────────────────────────────────────────
        /// <summary>Four KPI cards: total tracked, org avg, % high, % at-risk.</summary>
        Task<OrgSummaryDto> GetOrgSummaryAsync();

        /// <summary>Top-N employees ranked by avg score across ALL programs.</summary>
        Task<IEnumerable<OverallLeaderboardEntryDto>> GetOverallLeaderboardAsync(int top = 10);

        /// <summary>Top-N employees with the biggest positive score delta since last calc.</summary>
        Task<IEnumerable<MostImprovedEntryDto>> GetMostImprovedAsync(int top = 10);

        /// <summary>Org-wide avg per factor as % of max — shows WHERE engagement is weak.</summary>
        Task<FactorHealthDto> GetFactorHealthAsync();

        /// <summary>Per-program High/Medium/Low split + avg score + trend.</summary>
        Task<IEnumerable<ProgramHealthDto>> GetProgramHealthAsync();

        // ── Employee (personal view) ──────────────────────────────────────
        Task<IEnumerable<EmployeeEngagementDto>> GetMyEngagementAsync(Guid employeeId);

        // ── Recalculate ───────────────────────────────────────────────────
        /// <summary>Single employee + program — called by per-row recalculate button or other services.</summary>
        Task<HrEngagementDto> RecalculateAsync(Guid employeeId, Guid programId, string jwtToken);

        /// <summary>Recalculates every existing record. Called by the "Sync Scores" button.</summary>
        Task<int> RecalculateAllAsync(string jwtToken);

        /// <summary>Lightweight feedback-only patch — no HTTP calls needed.</summary>
        Task UpdateFeedbackScoreAsync(Guid employeeId, Guid programId);

        // ── Legacy analytics (kept for backwards compat) ──────────────────
        Task<EngagementSummaryDto> GetProgramSummaryAsync(Guid programId);
        Task<double> GetAverageScoreByProgramAsync(Guid programId);
        Task<IEnumerable<object>> GetLeaderboardAsync(Guid programId, int top = 10);
    }
}
