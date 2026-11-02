using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public class FeedbackDto
	{
		public Guid Id { get; set; }
		public Guid ProgramID { get; set; }
		public Guid? ActivityID { get; set; }
		public string? ActivityTitle { get; set; }
		public Guid EmployeeID { get; set; }
		public int Rating { get; set; }
		public string? Comments { get; set; }
		public DateTime Date { get; set; }
	}
}
