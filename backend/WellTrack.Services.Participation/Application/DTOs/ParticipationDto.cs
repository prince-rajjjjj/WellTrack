using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class ParticipationDto
	{
		public Guid Id { get; set; }
		public Guid ProgramID { get; set; }
		public Guid EmployeeID { get; set; }
		public Guid? ActivityID { get; set; }
		public string? ActivityTitle { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? Notes { get; set; }
	}
}
