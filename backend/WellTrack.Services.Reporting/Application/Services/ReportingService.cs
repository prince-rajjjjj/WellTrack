using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WellTrack.Services.Reporting.Application.DTOs;
using WellTrack.Services.Reporting.Infrastructure.HttpClients;

namespace WellTrack.Services.Reporting.Application.Services;

public class ReportingService : IReportingService
{
	private readonly IHttpClientFactory _factory;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger<ReportingService> _logger;

	private static readonly JsonSerializerOptions JsonOpts =
		new() { PropertyNameCaseInsensitive = true };

	public ReportingService(
		IHttpClientFactory factory,
		IHttpContextAccessor httpContextAccessor,
		ILogger<ReportingService> logger)
	{
		_factory = factory;
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
	}

	// ── Helpers ───────────────────────────────────────────────────────────

	private HttpClient Client(string name)
	{
		var client = _factory.CreateClient(name);
		var auth = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
		if (!string.IsNullOrWhiteSpace(auth))
			client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
		return client;
	}

	private async Task<List<T>> GetList<T>(string clientName, string path)
	{
		try
		{
			var result = await Client(clientName)
				.GetFromJsonAsync<ServiceResult<List<T>>>(path, JsonOpts);
			return result?.Data ?? new();
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Downstream list call failed — {Client} {Path}", clientName, path);
			return new();
		}
	}

	private async Task<PagedData<T>> GetPaged<T>(string clientName, string path)
	{
		try
		{
			var result = await Client(clientName)
				.GetFromJsonAsync<ServiceResult<PagedData<T>>>(path, JsonOpts);
			return result?.Data ?? new();
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Downstream paged call failed — {Client} {Path}", clientName, path);
			return new();
		}
	}

	// ── IReportingService ─────────────────────────────────────────────────

	public async Task<ProgramReportDto> GetProgramReportAsync()
	{
		var (programsPaged, participations) = await FetchParallel(
			GetPaged<DProgramDto>("WellnessClient", "/api/v1/programs?pageSize=1000"),
			GetList<DParticipationDto>("ParticipationClient", "/api/v1/participation"));
		var programs = programsPaged.Items;

		int total = programs.Count;
		int active = programs.Count(p => p.Status == 1);
		int completed = programs.Count(p => p.Status == 4);
		int uniquePax = participations.Select(p => p.EmployeeID).Distinct().Count();

		decimal avgRate = participations.Count > 0
			? Math.Round(participations.Count(p => p.Status == "Completed")
						 / (decimal)participations.Count * 100, 2)
			: 0m;

		return new ProgramReportDto
		{
			TotalPrograms = total,
			ActivePrograms = active,
			CompletedPrograms = completed,
			TotalParticipants = uniquePax,
			AverageParticipationRate = avgRate
		};
	}

	public async Task<EngagementReportDto> GetEngagementReportAsync()
	{
		var engagements = await GetList<DEngagementDto>("EngagementClient", "/api/v1/engagement");

		if (engagements.Count == 0)
			return new EngagementReportDto();

		double avgScore = engagements.Average(e => (double)e.TotalScore);

		var empAvgs = engagements
			.GroupBy(e => e.EmployeeID)
			.Select(g => g.Average(x => (double)x.TotalScore))
			.ToList();

		return new EngagementReportDto
		{
			TotalFeedbacks = engagements.Count,
			AverageRating = Math.Round((decimal)avgScore / 20m, 1),
			AverageEngagementScore = Math.Round((decimal)avgScore, 2),
			HighlyEngagedEmployees = empAvgs.Count(s => s >= 70),
			LowEngagementEmployees = empAvgs.Count(s => s < 40)
		};
	}

	public async Task<ComplianceReportDto> GetComplianceReportAsync()
	{
		var (auditsPaged, recordsPaged) = await FetchParallel(
			GetPaged<DAuditDto>("ComplianceClient", "/api/v1/audits?pageSize=1000"),
			GetPaged<DComplianceRecordDto>("ComplianceClient", "/api/v1/compliance-records?pageSize=1000"));
		var audits = auditsPaged.Items;
		var records = recordsPaged.Items;

		int total = audits.Count;
		int done = audits.Count(a => a.Status is "Approved" or "Closed");
		int pending = audits.Count(a => a.Status is "Draft" or "Submitted" or "Rejected");
		decimal rate = total > 0 ? Math.Round(done / (decimal)total * 100, 2) : 0m;

		return new ComplianceReportDto
		{
			TotalAudits = total,
			CompletedAudits = done,
			PendingAudits = pending,
			ComplianceRate = rate,
			IssuesIdentified = records.Count
		};
	}

	public async Task<ParticipationTrendReportDto> GetParticipationTrendsAsync()
	{
		var (participations, programsPaged) = await FetchParallel(
			GetList<DParticipationDto>("ParticipationClient", "/api/v1/participation"),
			GetPaged<DProgramDto>("WellnessClient", "/api/v1/programs?pageSize=1000"));
		var programs = programsPaged.Items;

		var programMap = programs.ToDictionary(p => p.Id, p => p.Title);

		int totalPax = participations.Select(p => p.EmployeeID).Distinct().Count();
		int activePax = participations
			.Where(p => p.Status is "Active" or "Joined")
			.Select(p => p.EmployeeID).Distinct().Count();
		double rate = totalPax > 0
			? Math.Round(activePax / (double)totalPax * 100, 2)
			: 0;

		// ✅ Filter BEFORE OrderBy/Take so we don't drop valid programs
		var topPrograms = participations
			.GroupBy(p => p.ProgramID)
			.Select(g => new
			{
				ProgramId = g.Key,
				Participants = g.Select(x => x.EmployeeID).Distinct().Count(),
				DoneCount = g.Count(x => x.Status is "Active" or "Joined" or "Completed"),
				TotalCount = g.Count()
			})
			.Where(x => programMap.ContainsKey(x.ProgramId))   // filter first
			.OrderByDescending(x => x.Participants)
			.Take(5)
			.Select(x => new ProgramParticipationStat
			{
				ProgramName = programMap[x.ProgramId],
				Participants = x.Participants,
				CompletionRate = x.TotalCount > 0
					? Math.Round(x.DoneCount / (double)x.TotalCount * 100, 1)
					: 0.0
			})
			.ToList();

		var currentMonthLabel = DateTime.UtcNow.ToString("MMM yyyy", CultureInfo.InvariantCulture);

		var monthlyTrends = participations
			.GroupBy(p => p.Date.ToString("MMM yyyy", CultureInfo.InvariantCulture))
			.Select(g => new MonthlyTrendPoint
			{
				Month = g.Key,
				Participants = g.Select(x => x.EmployeeID).Distinct().Count(),
				ActivitiesCompleted = g.Count(x => x.Status is "Active" or "Joined" or "Completed")
			})
			.Where(m => m.Month != currentMonthLabel)
			.OrderBy(m => DateTime.ParseExact(m.Month, "MMM yyyy", CultureInfo.InvariantCulture))
			.ToList();

		var current = monthlyTrends.Count >= 1 ? monthlyTrends[^1] : null;
		var previous = monthlyTrends.Count >= 2 ? monthlyTrends[^2] : null;

		return new ParticipationTrendReportDto
		{
			TotalParticipants = totalPax,
			ActiveParticipants = activePax,
			ParticipationRate = rate,
			TopPrograms = topPrograms,
			MonthlyTrends = monthlyTrends,

			CurrentMonth = current?.Month ?? string.Empty,
			CurrentMonthParticipants = current?.Participants ?? 0,
			CurrentMonthCompleted = (int)(current?.ActivitiesCompleted ?? 0),
			LastMonth = previous?.Month ?? string.Empty,
			LastMonthParticipants = previous?.Participants ?? 0,
			LastMonthCompleted = (int)(previous?.ActivitiesCompleted ?? 0)
		};
	}

	public async Task<EngagementScoreReportDto> GetEngagementScoresAsync()
	{
		var (engagements, usersPaged) = await FetchParallel(
			GetList<DEngagementDto>("EngagementClient", "/api/v1/engagement"),
			GetPaged<DUserDto>("IdentityClient", "/api/v1/users?pageSize=1000"));

		if (engagements.Count == 0)
			return new EngagementScoreReportDto();

		var userMap = usersPaged.Items.ToDictionary(u => u.Id, u => u.Name);

		double avgScore = engagements.Average(e => (double)e.TotalScore);

		var empAvgs = engagements
			.GroupBy(e => e.EmployeeID)
			.Select(g => new { Id = g.Key, Avg = g.Average(x => (double)x.TotalScore) })
			.ToList();

		var topEmployees = empAvgs
			.OrderByDescending(x => x.Avg)
			.Take(3)
			.Select(x => new EmployeeEngagementPoint
			{
				Name = userMap.GetValueOrDefault(x.Id, x.Id.ToString("N")[..8]),
				Score = Math.Round(x.Avg, 1)
			})
			.ToList();

		var trend = engagements
			.GroupBy(e => e.LastCalculatedAt.ToString("MMM yyyy", CultureInfo.InvariantCulture))
			.Select(g => new EngagementTrendPoint
			{
				Label = g.Key,
				Score = Math.Round(g.Average(x => (double)x.TotalScore), 1),
				ActiveUsers = g.Select(x => x.EmployeeID).Distinct().Count()
			})
			.OrderBy(t => DateTime.ParseExact(t.Label, "MMM yyyy", CultureInfo.InvariantCulture))
			.ToList();

		return new EngagementScoreReportDto
		{
			AverageScore = Math.Round(avgScore, 2),
			HighlyEngaged = empAvgs.Count(x => x.Avg >= 70),
			ModeratelyEngaged = empAvgs.Count(x => x.Avg >= 40 && x.Avg < 70),
			LowEngaged = empAvgs.Count(x => x.Avg < 40),
			TopEmployees = topEmployees,
			Trend = trend
		};
	}

	public async Task<List<UserDistributionDto>> GetUserDistributionAsync()
	{
		var paged = await GetPaged<DUserDto>("IdentityClient", "/api/v1/users?pageSize=1000");
		var users = paged.Items;

		if (users.Count == 0)
			return new List<UserDistributionDto>();

		double total = users.Count;
		return users
			.GroupBy(u => u.Role)
			.Select(g => new UserDistributionDto
			{
				Role = g.Key,
				Count = g.Count(),
				Percentage = Math.Round(g.Count() / total * 100, 2)
			})
			.OrderByDescending(x => x.Count)
			.ToList();
	}

	// ── Private helpers ───────────────────────────────────────────────────

	private static async Task<(T1, T2)> FetchParallel<T1, T2>(Task<T1> t1, Task<T2> t2)
	{
		await Task.WhenAll(t1, t2);
		return (await t1, await t2);
	}
}