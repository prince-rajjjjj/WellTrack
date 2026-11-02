namespace WellTrack.Services.Engagement.Application.DTOs
{
	/// <summary>
	/// Score breakdown shown to the employee so they understand what contributed to their total.
	/// </summary>
	public class ScoreBreakdownDto
	{
		/// <summary>Out of 30 — based on session attendance rate</summary>
		public decimal AttendanceScore { get; set; }

		/// <summary>Out of 25 — based on activities joined vs total in program</summary>
		public decimal ActivityScore { get; set; }

		/// <summary>Out of 25 — based on average wellness goal progress</summary>
		public decimal GoalScore { get; set; }

		/// <summary>Out of 10 — 10 if feedback submitted, 0 otherwise</summary>
		public decimal FeedbackScore { get; set; }

		/// <summary>Out of 10 — based on attendance consistency in last 30 days</summary>
		public decimal ConsistencyScore { get; set; }
	}
}
