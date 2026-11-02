using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Participation.Domain.Entities
{
	public class ParticipationEntity : AuditableEntity
	{
		public Guid ProgramID { get; set; }
		public Guid EmployeeID { get; set; }
		public Guid? ActivityID { get; set; }
		public string? ActivityTitle { get; set; }
		public DateTime Date { get; set; }
		public StatusType Status { get; set; } = StatusType.Completed;
		public string? Notes { get; set; }
	}
}
