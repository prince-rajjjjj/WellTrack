using System;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class CreateActivityDto
	{
		public Guid ProgramID { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;

		public DateTime Date { get; set; }
		public int Duration { get; set; }
		public int MaxParticipants { get; set; } = 30;

		public string Mode { get; set; } = "Offline";   // "Offline" | "Online"
		public string? Location { get; set; }            // required for Offline
		public string? VideoUrl { get; set; }            // required for Online

		public Guid? AssignedCoachID { get; set; }
		public string? AssignedCoachName { get; set; }
	}

	public class AssignCoachDto
	{
		public Guid? CoachID { get; set; }
		public string? CoachName { get; set; }
	}
}
