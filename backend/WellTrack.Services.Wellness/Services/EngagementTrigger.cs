using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WellTrack.Services.Wellness.Services
{
    /// <summary>
    /// Fire-and-forget helper that asks the Engagement service to recalculate
    /// a score after attendance or activity events in the Wellness service.
    ///
    /// All calls are fully async and swallow exceptions so a downed Engagement
    /// service never breaks a Wellness request.
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
            _baseUrl = config["Services:Engagement"] ?? "http://localhost:5004";
            _logger  = logger;
        }

        /// <summary>
        /// Triggers recalculation for every employee in <paramref name="employeeIds"/>
        /// for the given program. Runs all calls in parallel, fire-and-forget.
        /// </summary>
        public void TriggerForAttendees(
            IEnumerable<Guid> employeeIds,
            Guid programId,
            string jwt)
        {
            // Detach completely — caller does not await this
            _ = Task.Run(async () =>
            {
                var tasks = employeeIds.Select(empId =>
                    PostRecalculateAsync(empId, programId, jwt));
                await Task.WhenAll(tasks);
            });
        }

        private async Task PostRecalculateAsync(Guid employeeId, Guid programId, string jwt)
        {
            try
            {
                var client = _http.CreateClient();
                if (!string.IsNullOrWhiteSpace(jwt))
                    client.DefaultRequestHeaders.Authorization =
                        AuthenticationHeaderValue.Parse(jwt);

                var payload = JsonSerializer.Serialize(
                    new { employeeId, programId });

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
                    "[EngagementTrigger] Failed to trigger recalc for employee {E} program {P}",
                    employeeId, programId);
            }
        }
    }
}
