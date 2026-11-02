using System;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Wellness.Domain.Entities
{
	public class ActivityAttendance : AuditableEntity
	{
		public Guid ActivityID { get; set; }
		public Guid EmployeeID { get; set; }
		public string EmployeeName { get; set; } = string.Empty;
		public bool IsPresent { get; set; }
		public DateTime? JoinTime { get; set; }
		public DateTime? LeaveTime { get; set; }
		public string? Notes { get; set; }
	}
}
