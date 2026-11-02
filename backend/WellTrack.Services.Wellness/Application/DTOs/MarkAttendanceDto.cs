using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class MarkAttendanceDto
	{
		public Guid EmployeeID { get; set; }
		public string EmployeeName { get; set; } = string.Empty;
		public bool IsPresent { get; set; }
		public DateTime? JoinTime { get; set; }
		public DateTime? LeaveTime { get; set; }
		public string? Notes { get; set; }
	}
}
