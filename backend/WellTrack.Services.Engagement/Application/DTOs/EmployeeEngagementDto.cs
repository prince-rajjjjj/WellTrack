namespace WellTrack.Services.Engagement.Application.DTOs
{
	/// <summary>
	/// Personal engagement view returned to an employee.
	/// Shows their score, breakdown, and actionable tips to improve.
	/// </summary>
	public class EmployeeEngagementDto
	{
		public Guid ProgramID { get; set; }
		public decimal TotalScore { get; set; }

		/// <summary>"High" | "Medium" | "Low"</summary>
		public string EngagementLevel { get; set; } = string.Empty;

		/// <summary>Detailed factor scores so the employee can see where to improve</summary>
		public ScoreBreakdownDto Breakdown { get; set; } = new();

		/// <summary>
		/// Auto-generated tips for factors scoring below 50% of their maximum.
		/// Empty list means the employee is doing well on all fronts.
		/// </summary>
		public List<string> ImprovementTips { get; set; } = new();

		public DateTime  LastCalculatedAt { get; set; }

		/// <summary>Score before the most recent recalculation — null on first calc.</summary>
		public decimal?  PreviousScore    { get; set; }
	}
}
