using System;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class ActivityDto
	{
		public Guid Id { get; set; }
		public Guid ProgramID { get; set; }
		public string ProgramName { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;

		public DateTime Date { get; set; }
		public int Duration { get; set; }
		public int MaxParticipants { get; set; }

		public string Mode { get; set; } = "Offline";   // "Offline" | "Online"
		public string? Location { get; set; }
		public string? VideoUrl { get; set; }

		public StatusType Status { get; set; } = StatusType.Active;
		public Guid? AssignedCoachID { get; set; }
		public string? AssignedCoachName { get; set; }

		public bool AttendanceFinalized { get; set; }

		// Aggregate counts populated by service layer
		public int EnrolledCount { get; set; }
		public int AttendedCount { get; set; }

		// Per-current-user state — populated when the request is authenticated
		public bool IsEnrolled { get; set; }
		public bool HasAttended { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
