using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Engagement.Domain.Entities
{
	/// <summary>
	/// One record per Employee+Program. Upserted on every recalculation.
	/// Score breakdown mirrors the 5-factor formula (total = 100 pts).
	/// </summary>
	public class EngagementEntity : BaseEntity
	{
		public Guid EmployeeID { get; set; }
		public Guid ProgramID { get; set; }

		// ── Computed total (0–100) ─────────────────────────────────────
		public decimal TotalScore { get; set; }

		/// <summary>"High" ≥75 | "Medium" ≥40 | "Low" &lt;40</summary>
		public string EngagementLevel { get; set; } = "Low";

		// ── Score breakdown (stored for HR transparency) ───────────────
		/// <summary>Sessions attended ÷ total sessions × 30  (max 30)</summary>
		public decimal AttendanceScore { get; set; }

		/// <summary>Activities joined ÷ total activities × 25  (max 25)</summary>
		public decimal ActivityScore { get; set; }

		/// <summary>Avg goal progress % ÷ 100 × 25  (max 25)</summary>
		public decimal GoalScore { get; set; }

		/// <summary>10 if feedback submitted, else 0  (max 10)</summary>
		public decimal FeedbackScore { get; set; }

		/// <summary>Sessions attended (last 30 days) ÷ sessions held × 10  (max 10)</summary>
		public decimal ConsistencyScore { get; set; }

		public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Score captured before the most recent recalculation.
		/// Null on the very first calculation — no baseline yet.
		/// Used for trend arrows, Most Improved leaderboard and At-Risk detection.
		/// </summary>
		public decimal? PreviousScore { get; set; }
	}
}
