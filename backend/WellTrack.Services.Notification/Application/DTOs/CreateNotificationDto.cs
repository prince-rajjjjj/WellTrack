namespace WellTrack.Services.Notification.Application.DTOs;

public class CreateNotificationDto
{
    public Guid UserID { get; set; }
    public Guid? ProgramID { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
