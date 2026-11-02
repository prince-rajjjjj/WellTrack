
namespace WellTrack.Services.Identity.Application.DTOs
{
	public class AuditLogDto
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string Action { get; set; } = string.Empty;
		public string Resource { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; }
		public string? Details { get; set; }
	}
}
