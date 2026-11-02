using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class UpdateProgramDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
