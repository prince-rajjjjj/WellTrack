using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class CreateGoalDto
	{
		public Guid EmployeeID { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal TargetValue { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
