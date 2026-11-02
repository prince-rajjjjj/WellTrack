using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Reporting.Domain.Entities
{
	public class Report : BaseEntity
	{
		public string Scope { get; set; } = string.Empty;
		public string Metrics { get; set; } = string.Empty;
		public DateTime GeneratedDate { get; set; } 
		public Guid GeneratedBy { get; set; }
	}
}
