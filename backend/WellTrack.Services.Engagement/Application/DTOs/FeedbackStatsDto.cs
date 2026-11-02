using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Engagement.Application.DTOs
{
	public class FeedbackStatsDto
	{
		public Guid ProgramID { get; set; }
		public int TotalFeedbacks { get; set; }
		public double AverageRating { get; set; }
		public int FiveStarCount { get; set; }
		public int FourStarCount { get; set; }
		public int ThreeStarCount { get; set; }
		public int TwoStarCount { get; set; }
		public int OneStarCount { get; set; }
	}
}
