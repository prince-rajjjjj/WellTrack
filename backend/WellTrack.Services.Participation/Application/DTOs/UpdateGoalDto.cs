using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class UpdateGoalDto
	{
		public string Description { get; set; } = string.Empty;
		public decimal TargetValue { get; set; }
		public decimal CurrentValue { get; set; }
		public DateTime EndDate { get; set; }
	}
}
