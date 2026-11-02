using System;
using System.Collections.Generic;
using System.Text;

namespace WellTrack.Services.Participation.Application.DTOs
{
	public class JoinActivityRequest
	{
		public Guid ProgramId { get; set; }
		public Guid ActivityId { get; set; }
		public string? ActivityTitle { get; set; }
	}
}
