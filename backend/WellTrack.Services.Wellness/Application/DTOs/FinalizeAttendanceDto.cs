using System;
using System.Collections.Generic;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	// Coach submits the whole roster in one shot. After this lands the
	// activity's AttendanceFinalized flag is set and no further edits are allowed.
	public class FinalizeAttendanceDto
	{
		public List<AttendanceEntry> Entries { get; set; } = new();
	}

	public class AttendanceEntry
	{
		public Guid EmployeeID { get; set; }
		public bool IsPresent { get; set; }
	}
}
