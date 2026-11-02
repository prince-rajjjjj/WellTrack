using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.Services.Compliance.Domain.Entities;

namespace WellTrack.Services.Compliance.Application.DTOs
{
	public class AuditDto
	{
		public Guid Id { get; set; }
		public Guid ComplianceOfficerID { get; set; }
		public string Scope { get; set; } = string.Empty;
		public string? Findings { get; set; }
		public string? RejectionReason { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; } = string.Empty;
	}

	public class CreateAuditDto
	{	
		public Guid ComplianceOfficerID { get; set; }
		public string Scope { get; set; } = string.Empty;
		public string? Findings { get; set; }
	}

	public class UpdateAuditDto
	{
		public string Scope { get; set; } = string.Empty;
		public string? Findings { get; set; }
		public AuditStatus AuditStatus { get; set; } // To move from Draft to Finalized
	}

	public class SubmitAuditDto
	{
		public string? Findings { get; set; }
	}

	public class RejectAuditDto
	{
		public string Reason { get; set; } = string.Empty;
	}

	public class CloseAuditDto 
	{
		public string? FinalFindings { get; set; } 
	}

}
