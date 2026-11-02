namespace WellTrack.Services.Engagement.Application.DTOs
{
	/// <summary>
	/// Full engagement view returned to HR / Admin.
	/// Includes every score factor so HR can see exactly why an employee scored what they did.
	/// </summary>
	public class HrEngagementDto
	{
		public Guid Id { get; set; }
		public Guid EmployeeID { get; set; }
		public Guid ProgramID { get; set; }

		// ── Overall ──────────────────────────────────────────────────
		public decimal TotalScore { get; set; }
		public string EngagementLevel { get; set; } = string.Empty;

		// ── Breakdown ────────────────────────────────────────────────
		/// <summary>Out of 30</summary>
		public decimal AttendanceScore { get; set; }

		/// <summary>Out of 25</summary>
		public decimal ActivityScore { get; set; }

		/// <summary>Out of 25</summary>
		public decimal GoalScore { get; set; }

		/// <summary>Out of 10</summary>
		public decimal FeedbackScore { get; set; }

		/// <summary>Out of 10</summary>
		public decimal ConsistencyScore { get; set; }

		public DateTime LastCalculatedAt { get; set; }
	}
}
