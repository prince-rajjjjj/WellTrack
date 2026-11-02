namespace WellTrack.Services.Identity.Application.DTOs
{
	public class ForgotPasswordDto
	{
		public string Email { get; set; } = string.Empty;
	}

	public class ResetPasswordRequestDto
	{
		public string Email { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
	}
}
