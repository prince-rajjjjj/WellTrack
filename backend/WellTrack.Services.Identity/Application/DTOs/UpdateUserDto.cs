using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Identity.Application.DTOs
{
	public class UpdateUserDto
	{
		public string Name { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public UserRole Role { get; set; }
		public StatusType Status { get; set; }
	}
}
