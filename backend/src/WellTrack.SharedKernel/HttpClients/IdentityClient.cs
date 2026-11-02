using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace WellTrack.SharedKernel.HttpClients;

public class IdentityClient : IIdentityClient
{
	private readonly HttpClient _client;
	private readonly IHttpContextAccessor _accessor;

	public IdentityClient(HttpClient client, IHttpContextAccessor accessor)
	{
		_client = client;
		_accessor = accessor;
	}

	public async Task<List<Guid>> GetAllEmployeeIdsAsync()
		=> await GetUserIdsByRoleAsync("Employee");

	public async Task<List<Guid>> GetUserIdsByRoleAsync(string role)
	{
		try
		{
			ForwardJwt();
			var response = await _client.GetFromJsonAsync<EmployeeIdsResponse>(
				$"/api/v1/users/ids-by-role?role={role}");
			return response?.Data ?? new List<Guid>();
		}
		catch
		{
			return new List<Guid>();
		}
	}

	private void ForwardJwt()
	{
		var auth = _accessor.HttpContext?.Request.Headers.Authorization.ToString();
		if (!string.IsNullOrWhiteSpace(auth))
			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
	}

	private class EmployeeIdsResponse { public List<Guid> Data { get; set; } = new(); }
}