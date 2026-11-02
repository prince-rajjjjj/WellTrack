namespace WellTrack.Services.Engagement.Application.DTOs
{
	/// <summary>
	/// HR-level summary of engagement distribution across a program.
	/// Helps HR spot at-risk employees at a glance.
	/// </summary>
	public class EngagementSummaryDto
	{
		public Guid ProgramID { get; set; }
		public int TotalEmployees { get; set; }

		/// <summary>Employees with TotalScore ≥ 75</summary>
		public int HighEngagement { get; set; }

		/// <summary>Employees with TotalScore between 40 and 74</summary>
		public int MediumEngagement { get; set; }

		/// <summary>Employees with TotalScore &lt; 40 — at risk</summary>
		public int LowEngagement { get; set; }

		public double AverageScore { get; set; }
	}
}
