using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class ProgramReportDto
	{
	public int TotalPrograms { get; set; }
	public int ActivePrograms { get; set; }
	public int CompletedPrograms { get; set; }
	public int TotalParticipants { get; set; }
	public decimal AverageParticipationRate { get; set; }
	}
}
