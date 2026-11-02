using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WellTrack.SharedKernel.Entities;

namespace WellTrack.Services.Identity.Domain.Entities
{
	public class AuditLog : BaseEntity
	{
		public Guid UserId { get; set; }
		public string Action { get; set; } = string.Empty;
		public string Resource { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public string? Details { get; set; }
	}
}
