using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Wellness.Application.DTOs
{
	public class ProgramDto
	{

		public Guid Id { get; set; }	 
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;

		public Guid HRID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public StatusType Status { get; set; } = StatusType.Active;

		public DateTime CreatedAt { get; set; }
	}
}
