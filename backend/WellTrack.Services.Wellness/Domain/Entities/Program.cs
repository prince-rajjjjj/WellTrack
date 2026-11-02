using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Wellness.Domain.Entities
{	
	public class programs: AuditableEntity
	{
		[Key]
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;

		public Guid HRID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public StatusType Status { get; set; } = StatusType.Active;

		public ICollection<Activity> Activities { get; set; } = new List<Activity>();
	}
}
