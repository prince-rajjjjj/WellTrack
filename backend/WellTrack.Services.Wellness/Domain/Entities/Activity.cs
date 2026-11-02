using System;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Wellness.Domain.Entities
{
	// Activity is the scheduled thing under a Program.
	// Mode picks Offline (Location) vs Online (VideoUrl).
	public class Activity : AuditableEntity
	{
		public Guid ProgramID { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;

		public DateTime Date { get; set; }
		public int Duration { get; set; }            // minutes
		public int MaxParticipants { get; set; } = 30;

		public ActivityMode Mode { get; set; } = ActivityMode.Offline;
		public string? Location { get; set; }        // e.g. "SDB1, 4th floor, ODC 5" — Offline only
		public string? VideoUrl { get; set; }        // YouTube URL — Online only

		// Lifecycle: Active=upcoming/scheduled, Pending=in-progress (coach started),
		// Completed=done, Cancelled=killed.
		public StatusType Status { get; set; } = StatusType.Active;

		public Guid? AssignedCoachID { get; set; }
		public string? AssignedCoachName { get; set; }

		// Once the coach submits attendance, no further edits are allowed.
		public bool AttendanceFinalized { get; set; } = false;

		public programs? Program { get; set; }
	}

	public enum ActivityMode
	{
		Offline = 1,
		Online  = 2
	}
}
