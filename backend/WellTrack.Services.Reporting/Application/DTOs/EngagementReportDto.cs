using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class EngagementReportDto
	{
	public int TotalFeedbacks{ get; set; }
	public decimal AverageRating { get; set; }
	public decimal AverageEngagementScore{  get; set; }
	public int	HighlyEngagedEmployees{  get; set; }
	public int LowEngagementEmployees{ get; set; }


	}
}
