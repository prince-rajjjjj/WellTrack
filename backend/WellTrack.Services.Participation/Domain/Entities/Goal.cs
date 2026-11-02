using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Participation.Domain.Entities
{
	public class Goal : AuditableEntity
	{
		public Guid EmployeeID { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal TargetValue { get; set; }
		public decimal CurrentValue { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public StatusType Status { get; set; } = StatusType.Active;
	}

}
