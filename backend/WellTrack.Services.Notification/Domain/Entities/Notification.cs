using WellTrack.SharedKernel.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Notification.Domain.Entities;

public class NotificationEntity : BaseEntity
{
    public Guid UserID { get; set; }
    public Guid? ProgramID { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public StatusType Status { get; set; } = StatusType.Active;
}
