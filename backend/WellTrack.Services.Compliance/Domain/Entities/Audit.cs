using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;


namespace WellTrack.Services.Compliance.Domain.Entities
{
	public enum AuditStatus { Draft = 1, Submitted = 2, Approved = 3, Rejected = 4, Closed = 5 }
	public class Audit: AuditableEntity
	{
		public Guid ComplianceOfficerID { get; set; }
		public string Scope { get; set; } = string.Empty;
		public string? Findings { get; set; }
		public string? RejectionReason { get; set; }
		public DateTime Date { get; set; } = DateTime.UtcNow;
		public AuditStatus AuditStatus { get; set; } = AuditStatus.Draft;
		// Legacy — kept for EF compatibility
		public StatusType Status { get; set; } = StatusType.Pending;
	}
}
