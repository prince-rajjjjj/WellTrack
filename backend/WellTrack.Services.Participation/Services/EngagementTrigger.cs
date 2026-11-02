using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WellTrack.Services.Participation.Services
{
    /// <summary>
    /// Fire-and-forget helper that asks the Engagement service to recalculate
    /// a score after participation events (join activity, goal update, etc.).
    /// Exceptions are swallowed — a downed Engagement service never breaks a Participation request.
    /// </summary>
    public class EngagementTrigger
    {
        private readonly IHttpClientFactory _http;
        private readonly string _baseUrl;
        private readonly ILogger<EngagementTrigger> _logger;

        public EngagementTrigger(
            IHttpClientFactory http,
            IConfiguration config,
            ILogger<EngagementTrigger> logger)
        {
            _http    = http;
            _baseUrl = config["ServiceUrls:Engagement"] ?? "http://localhost:5004";
            _logger  = logger;
        }

        public void Trigger(Guid employeeId, Guid programId, string jwt)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var client = _http.CreateClient();
                    if (!string.IsNullOrWhiteSpace(jwt))
                        client.DefaultRequestHeaders.Authorization =
                            AuthenticationHeaderValue.Parse(jwt);

                    var payload = JsonSerializer.Serialize(new { employeeId, programId });
                    var response = await client.PostAsync(
                        $"{_baseUrl}/api/v1/engagement/recalculate",
                        new StringContent(payload, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                        _logger.LogWarning(
                            "[EngagementTrigger] Recalc returned {Status} for employee {E} program {P}",
                            (int)response.StatusCode, employeeId, programId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "[EngagementTrigger] Failed recalc for employee {E} program {P}",
                        employeeId, programId);
                }
            });
        }
    }
}
