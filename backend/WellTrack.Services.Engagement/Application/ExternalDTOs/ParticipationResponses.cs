namespace WellTrack.Services.Engagement.Application.ExternalDTOs
{
	/// <summary>
	/// Minimal models that mirror the Participation service (port 5003) API response shapes.
	/// Only the fields we actually use for engagement calculation are mapped.
	/// </summary>

	internal class ParticipationResponse
	{
		public Guid ProgramID { get; set; }
		public Guid EmployeeID { get; set; }

		/// <summary>Nullable — only set when employee joined a specific activity</summary>
		public Guid? ActivityID { get; set; }

		public string Status { get; set; } = string.Empty;
	}

	internal class GoalResponse
	{
		public Guid Id { get; set; }
		public Guid EmployeeID { get; set; }

		/// <summary>Pre-calculated by Participation service: (CurrentValue / TargetValue) * 100</summary>
		public decimal ProgressPercentage { get; set; }

		public string Status { get; set; } = string.Empty;
	}
}
