namespace WellTrack.SharedKernel.HttpClients;

public interface IIdentityClient
{
	Task<List<Guid>> GetAllEmployeeIdsAsync();
	Task<List<Guid>> GetUserIdsByRoleAsync(string role);
}