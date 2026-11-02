using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public  class EngagementDto
	{
		public Guid Id { get; set; }
		public Guid EmployeeID { get; set; }
		public Guid ProgramID { get; set; }
		public Guid? ActivityID { get; set; }
		public decimal Score { get; set; }
		public DateTime Date { get; set; }
	}
}
