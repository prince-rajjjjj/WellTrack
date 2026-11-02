using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public class CreateFeedbackDto
	{
		public Guid ProgramId { get; set; }
		public Guid? ActivityId { get; set; }
		public string? ActivityTitle { get; set; }
		public int Rating { get; set; }
		public string? Comments { get; set; }
	}
}
