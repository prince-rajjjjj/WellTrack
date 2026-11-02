using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public class RecordEngagementDto
	{
		public Guid ProgramID { get; set; }
		public Guid? ActivityID { get; set; }
		public decimal Score { get; set; }
	}
}
