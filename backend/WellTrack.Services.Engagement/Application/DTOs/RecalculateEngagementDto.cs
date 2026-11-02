using System.ComponentModel.DataAnnotations;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	/// <summary>
	/// Payload for the internal recalculate trigger.
	/// Called by HR/Admin or by other services (Wellness, Participation) when relevant events occur.
	/// </summary>
	public class RecalculateEngagementDto
	{
		[Required]
		public Guid EmployeeId { get; set; }

		[Required]
		public Guid ProgramId { get; set; }
	}
}
