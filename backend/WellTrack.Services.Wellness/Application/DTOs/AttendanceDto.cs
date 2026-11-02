using System;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class AttendanceDto
	{
		public Guid Id { get; set; }
		public Guid ActivityID { get; set; }
		public Guid EmployeeID { get; set; }
		public string EmployeeName { get; set; } = string.Empty;
		public bool IsPresent { get; set; }
		public DateTime? JoinTime { get; set; }
		public DateTime? LeaveTime { get; set; }
		public string? Notes { get; set; }
	}
}
