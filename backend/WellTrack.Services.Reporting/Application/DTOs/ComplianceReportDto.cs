using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class ComplianceReportDto
	{
	public int TotalAudits { get; set; }
	public int CompletedAudits { get; set; }
	 public int PendingAudits{  get; set; }
	 public decimal ComplianceRate { get; set; }
	 public int IssuesIdentified { get; set; }
	}
}
