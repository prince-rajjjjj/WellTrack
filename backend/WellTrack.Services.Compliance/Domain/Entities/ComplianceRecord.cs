using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Compliance.Domain.Entities
{
	public enum ComplianceSeverity { Low = 1, Medium = 2, High = 3, Critical = 4 }

	public enum ComplianceRecordStatus
	{
		Reported   = 1,
		InProgress = 3,
		Closed     = 6
	}
	public class ComplianceRecord : AuditableEntity
	{
		public Guid ProgramID { get; set; }
		public Guid? ActivityID { get; set; }
		public Guid PolicyID { get; set; }
		// Name snapshots captured at record-creation time. They preserve
		// historical meaning if the related Program / Activity / Policy is
		// later deleted or renamed.  Programs and Activities live in the
		// Wellness service, so we cannot resolve them via DB join.
		public string? ProgramNameSnapshot  { get; set; }
		public string? ActivityNameSnapshot { get; set; }
		public string? PolicyNameSnapshot   { get; set; }
		public string IssueType { get; set; } = string.Empty;
		public ComplianceSeverity Severity { get; set; } = ComplianceSeverity.Medium;
		public string Notes { get; set; } = string.Empty;
		public ComplianceRecordStatus Status { get; set; } = ComplianceRecordStatus.Reported;
		public string? AssignedTo { get; set; }   // "HR" or "Coach"
		public string? AssignedToId { get; set; } // user id
		public string? ResolutionNotes { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		// Legacy fields kept for migration compatibility
		public string Type { get; set; } = string.Empty;
		public string Result { get; set; } = string.Empty;
		public DateTime Date { get; set; } = DateTime.UtcNow;
	}
}
