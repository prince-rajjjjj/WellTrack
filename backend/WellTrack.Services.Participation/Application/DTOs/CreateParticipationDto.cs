using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class CreateParticipationDto
	{
		public Guid ProgramID { get; set; }
		public Guid EmployeeID { get; set; }
		public Guid? ActivityID { get; set; }
		public DateTime Date { get; set; }
		public string? Notes { get; set; }
	}
}
