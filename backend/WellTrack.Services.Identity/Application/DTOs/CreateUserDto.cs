using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Identity.Application.DTOs
{

	public class CreateUserDto
	{
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public UserRole Role { get; set; }
		public string Password { get; set; } = string.Empty;
	}
}
