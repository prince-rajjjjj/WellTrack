using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Engagement.Domain.Entities
{
	public class Feedback : AuditableEntity
	{
		public Guid ProgramID { get; set; }
		/// <summary>Null for legacy program-level feedback; set for activity-level feedback.</summary>
		public Guid? ActivityID { get; set; }
		/// <summary>Denormalised title so the display doesn't need a round-trip to Wellness.</summary>
		public string? ActivityTitle { get; set; }
		public Guid EmployeeID { get; set; }
		public int Rating { get; set; }
		public string? Comments { get; set; }
		public DateTime Date { get; set; } = DateTime.UtcNow;
	}
}
