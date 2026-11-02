using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public class UpdateFeedbackDto
	{
		public int Rating { get; set; }
		public string? Comments { get; set; }
	}
}
