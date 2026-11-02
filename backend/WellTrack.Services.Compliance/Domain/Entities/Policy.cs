using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Compliance.Domain.Entities
{
	public enum PolicySeverity { Low = 1, Medium = 2, High = 3, Critical = 4 }
	public enum PolicyStatus { Active = 1, Inactive = 2 }
	public class Policy : AuditableEntity
	{
		public string PolicyName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public PolicySeverity Severity { get; set; } = PolicySeverity.Medium;
		public PolicyStatus Status { get; set; } = PolicyStatus.Active;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	}
}
