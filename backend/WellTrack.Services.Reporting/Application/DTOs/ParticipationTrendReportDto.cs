using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class ParticipationTrendReportDto
	{
		public int TotalParticipants { get; set; }
		public int ActiveParticipants { get; set; }
		public double ParticipationRate { get; set; }

		public List<MonthlyTrendPoint> MonthlyTrends { get; set; } = new();
		public List<ProgramParticipationStat> TopPrograms { get; set; } = new();

		// ── Month-over-month comparison ───────────────────────────────────────
		public string CurrentMonth { get; set; } = string.Empty;
		public int CurrentMonthParticipants { get; set; }
		public int CurrentMonthCompleted { get; set; }
		public string LastMonth { get; set; } = string.Empty;
		public int LastMonthParticipants { get; set; }
		public int LastMonthCompleted { get; set; }
	}

	public class ProgramParticipationStat
	{
		public string ProgramName { get; set; } = string.Empty;
		public int Participants { get; set; }
		public double CompletionRate { get; set; }
	}

	public class MonthlyTrendPoint
	{
		public string Month { get; set; } = string.Empty;
		public int Participants { get; set; }
		public double ActivitiesCompleted { get; set; }
	}
}
