using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Compliance.Application.DTOs
{
	public class ComplianceRecordDto
	{
		public Guid Id { get; set; }
		public Guid ProgramID { get; set; }
		public string ProgramName { get; set; } = string.Empty;
		public Guid? ActivityID { get; set; }
		public string ActivityName { get; set; } = string.Empty;
		public Guid PolicyID { get; set; }
		public string PolicyName { get; set; } = string.Empty;
		public string IssueType { get; set; } = string.Empty;
		public string Severity { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string? AssignedTo { get; set; }
		public string? AssignedToId { get; set; }
		public string? ResolutionNotes { get; set; }
		// Who reported this issue — populated from JWT on create.
		// Lets CO see "issues I personally reported" on their dashboard.
		public string? CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}

	public class CreateComplianceRecordDto
	{
		public Guid ProgramID { get; set; }
		public Guid? ActivityID { get; set; }
		public Guid PolicyID { get; set; }
		// Optional snapshots — sent by the UI so the record preserves the
		// program / activity name even after the source is deleted.
		public string? ProgramName  { get; set; }
		public string? ActivityName { get; set; }
		public string IssueType { get; set; } = string.Empty;
		public string Severity { get; set; } = "Medium";
		public string Notes { get; set; } = string.Empty;
	}

	/// <summary>Body for closing a compliance record — HR captures how it was resolved.</summary>
	public class CloseComplianceRecordDto
	{
		public string? ResolutionNotes { get; set; }
	}

}
