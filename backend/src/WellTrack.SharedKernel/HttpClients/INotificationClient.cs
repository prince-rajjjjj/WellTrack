namespace WellTrack.SharedKernel.HttpClients;

public interface INotificationClient
{
	Task NotifyAsync(Guid userId, string message, string category, Guid? programId = null);
	Task NotifyManyAsync(IEnumerable<Guid> userIds, string message, string category, Guid? programId = null);
}