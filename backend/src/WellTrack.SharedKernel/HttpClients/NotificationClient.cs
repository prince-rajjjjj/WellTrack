using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WellTrack.SharedKernel.DTOs;

namespace WellTrack.SharedKernel.HttpClients;

public class NotificationClient : INotificationClient
{
	private readonly HttpClient _client;
	private readonly IHttpContextAccessor _accessor;
	private readonly ILogger<NotificationClient> _logger;

	public NotificationClient(
		HttpClient client,
		IHttpContextAccessor accessor,
		ILogger<NotificationClient> logger)
	{
		_client = client;
		_accessor = accessor;
		_logger = logger;
	}

	public async Task NotifyAsync(Guid userId, string message, string category, Guid? programId = null)
	{
		try
		{
			ForwardJwt();

			var dto = new CreateNotificationDto
			{
				UserID = userId,
				ProgramID = programId,
				Message = message,
				Category = category
			};

			var response = await _client.PostAsJsonAsync("/api/v1/notifications", dto);

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning(
					"Notification failed for user {UserId}: {StatusCode}",
					userId, response.StatusCode);
			}
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Could not send notification to {UserId}", userId);
		}
	}

	public async Task NotifyManyAsync(
		IEnumerable<Guid> userIds, string message, string category, Guid? programId = null)
	{
		var tasks = userIds.Select(id => NotifyAsync(id, message, category, programId));
		await Task.WhenAll(tasks);
	}

	private void ForwardJwt()
	{
		var auth = _accessor.HttpContext?.Request.Headers.Authorization.ToString();
		if (!string.IsNullOrWhiteSpace(auth))
			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
	}
}