using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class GoalDto
	{
		public Guid Id { get; set; }
		public Guid EmployeeID { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal TargetValue { get; set; }
		public decimal CurrentValue { get; set; }
		public decimal ProgressPercentage => TargetValue > 0 ? (CurrentValue / TargetValue) * 100 : 0;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
