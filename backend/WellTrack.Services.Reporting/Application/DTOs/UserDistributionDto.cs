using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Reporting.Application.DTOs
{
	public class UserDistributionDto
	{
		public string Role { get; set; } = string.Empty;
		public int Count { get; set; }
		public double Percentage { get; set; }
	}
}
