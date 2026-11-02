using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class EngagementScoreReportDto
	{
	public double AverageScore { get; set; }
	public int HighlyEngaged { get; set; }
	public int ModeratelyEngaged { get; set; }
	public int LowEngaged { get; set; }
	public List<EmployeeEngagementPoint> TopEmployees { get; set; } = new();
	public List<EngagementTrendPoint> Trend{  get; set; } = new();

	}

	public class EngagementTrendPoint
	{
	public string Label { get; set; }
	public double Score { get; set; }
	public int ActiveUsers{  get; set; }
	}

	public class EmployeeEngagementPoint
	{
	public string Name { get; set; }
	public double Score { get; set; }
	}
}
