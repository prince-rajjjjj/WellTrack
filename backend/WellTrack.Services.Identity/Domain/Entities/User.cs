using System;
using System.Collections.Generic;
using System.Text;
using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Identity.Domain.Entities
{
	public class User : AuditableEntity
	{
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public UserRole Role { get; set; }
		public string? Phone { get; set; }
		public StatusType Status { get; set; } = StatusType.Active;
	}
}
