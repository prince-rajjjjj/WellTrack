using System.Net.Http.Headers;

namespace WellTrack.Services.Wellness.Services
{
    public class ParticipationTrigger
    {
        private readonly IHttpClientFactory _http;
        private readonly string _baseUrl;
        private readonly ILogger<ParticipationTrigger> _logger;

        public ParticipationTrigger(
            IHttpClientFactory http,
            IConfiguration config,
            ILogger<ParticipationTrigger> logger)
        {
            _http    = http;
            _baseUrl = config["Services:Participation"] ?? "http://localhost:5003";
            _logger  = logger;
        }

        public void CompleteActivity(Guid activityId, string jwt)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var client = _http.CreateClient();
                    if (!string.IsNullOrWhiteSpace(jwt))
                        client.DefaultRequestHeaders.Authorization =
                            AuthenticationHeaderValue.Parse(jwt);

                    await client.PostAsync(
                        $"{_baseUrl}/api/v1/participation/complete-by-activity/{activityId}",
                        null);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "[ParticipationTrigger] Failed to complete participation for activity {Id}", activityId);
                }
            });
        }
    }
}
