using System.Net.Http.Headers;
using System.Text.Json;
using WellTrack.Services.Engagement.Application.ExternalDTOs;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.Services.Engagement.Repositories;

namespace WellTrack.Services.Engagement.Application.Services
{
	/// <summary>
	/// Implements the 5-factor engagement score formula:
	///
	///   Factor              Weight  Source
	///   ─────────────────── ──────  ──────────────────────────────────
	///   Session Attendance    30    Wellness  → /activities + /activities/{id}/attendance
	///   Activity Completion   25    Wellness + Participation → activities joined
	///   Goal Progress         25    Participation → goals ProgressPercentage
	///   Feedback Submitted    10    Local DB  → Feedback table
	///   Consistency (30-day)  10    Wellness  → attendance on activities ≤30 days old
	///   ─────────────────── ──────
	///   Total               100
	///
	/// Each factor is isolated in its own method with a try/catch so a single
	/// unavailable downstream service returns 0 for that factor without failing
	/// the entire calculation (mirrors the Participation ↔ Notification pattern).
	///
	/// Note: an "activity" in this codebase = the entity formerly modeled as a
	/// "session". There is only one Wellness endpoint family (/api/v1/activities).
	/// </summary>
	public class EngagementCalculationService : IEngagementCalculationService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IFeedbackRepository _feedbackRepository;
		private readonly ILogger<EngagementCalculationService> _logger;

		private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

		public EngagementCalculationService(
			IHttpClientFactory httpClientFactory,
			IFeedbackRepository feedbackRepository,
			ILogger<EngagementCalculationService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_feedbackRepository = feedbackRepository;
			_logger = logger;
		}

		// ── Public entry point ─────────────────────────────────────────────
		public async Task<EngagementEntity> CalculateAsync(Guid employeeId, Guid programId, string jwtToken)
		{
			// Run independent HTTP calls in parallel where possible
			var attendanceTask    = CalculateAttendanceScoreAsync(employeeId, programId, jwtToken);
			var activityTask      = CalculateActivityScoreAsync(employeeId, programId, jwtToken);
			var goalTask          = CalculateGoalScoreAsync(employeeId, jwtToken);
			var feedbackTask      = CalculateFeedbackScoreAsync(employeeId, programId);
			var consistencyTask   = CalculateConsistencyScoreAsync(employeeId, programId, jwtToken);

			await Task.WhenAll(attendanceTask, activityTask, goalTask, feedbackTask, consistencyTask);

			var attendance  = attendanceTask.Result;
			var activity    = activityTask.Result;
			var goal        = goalTask.Result;
			var feedback    = feedbackTask.Result;
			var consistency = consistencyTask.Result;

			var total = Math.Round(attendance + activity + goal + feedback + consistency, 2);
			var level = total >= 75 ? "High" : total >= 40 ? "Medium" : "Low";

			return new EngagementEntity
			{
				EmployeeID        = employeeId,
				ProgramID         = programId,
				TotalScore        = total,
				EngagementLevel   = level,
				AttendanceScore   = attendance,
				ActivityScore     = activity,
				GoalScore         = goal,
				FeedbackScore     = feedback,
				ConsistencyScore  = consistency,
				LastCalculatedAt  = DateTime.UtcNow
			};
		}

		// ── Factor 1: Session Attendance (max 30) ──────────────────────────
		// Counts activities where the Coach marked this employee as Present
		// (IsPresent = true) after finalising the attendance roster.
		private async Task<decimal> CalculateAttendanceScoreAsync(Guid employeeId, Guid programId, string jwt)
		{
			try
			{
				var client = WellnessClient(jwt);
				var res    = await client.GetAsync($"/api/v1/activities?programId={programId}&pageSize=1000");
				if (!res.IsSuccessStatusCode) return 0;

				var activities        = await ParsePagedAsync<ActivityResponse>(res);
				var programActivities = activities?.ToList() ?? new();
				if (programActivities.Count == 0) return 0;

				int attended = await CountAttendedSessionsAsync(client, programActivities, employeeId);
				_logger.LogInformation(
					"[Engagement] Attendance {Attended}/{Total} for employee {Id} program {PId}",
					attended, programActivities.Count, employeeId, programId);

				return Math.Round((decimal)attended / programActivities.Count * 30, 2);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] Attendance score unavailable for employee {Id}", employeeId);
				return 0;
			}
		}

		// ── Factor 2: Activity Engagement (max 25) ─────────────────────────
		// Counts activities where the employee has ANY attendance record
		// (joined = enrolled, regardless of Present/Absent). Distinct from Factor 1
		// which requires the Coach to mark them Present.
		// Uses Wellness attendance as the single source of truth — avoids
		// the Participation service duplication that caused scores to always be 0.
		private async Task<decimal> CalculateActivityScoreAsync(Guid employeeId, Guid programId, string jwt)
		{
			try
			{
				var client     = WellnessClient(jwt);
				var activitiesRes = await client.GetAsync($"/api/v1/activities?programId={programId}&pageSize=1000");
				if (!activitiesRes.IsSuccessStatusCode) return 0;

				var activities    = await ParsePagedAsync<ActivityResponse>(activitiesRes);
				var totalActivities = activities?.Count() ?? 0;
				if (totalActivities == 0) return 0;

				// Count activities where the employee has joined (has an attendance record)
				int joined = 0;
				foreach (var activity in activities!)
				{
					try
					{
						var attRes   = await client.GetAsync($"/api/v1/activities/{activity.Id}/attendance");
						if (!attRes.IsSuccessStatusCode) continue;
						var attendees = await ParseAsync<IEnumerable<AttendanceResponse>>(attRes);
						if (attendees?.Any(a => a.EmployeeID == employeeId) == true)
							joined++;
					}
					catch { /* skip individual activity failures */ }
				}

				_logger.LogInformation(
					"[Engagement] Activity engagement {Joined}/{Total} for employee {Id} program {PId}",
					joined, totalActivities, employeeId, programId);

				return Math.Round((decimal)joined / totalActivities * 25, 2);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] Activity score unavailable for employee {Id}", employeeId);
				return 0;
			}
		}

		// ── Factor 3: Goal Progress (max 25) ───────────────────────────────
		private async Task<decimal> CalculateGoalScoreAsync(Guid employeeId, string jwt)
		{
			try
			{
				var client = ParticipationClient(jwt);
				var res = await client.GetAsync($"/api/v1/goals?employeeId={employeeId}&pageSize=1000");
				if (!res.IsSuccessStatusCode) return 0;

				var goals = await ParsePagedAsync<GoalResponse>(res);
				if (goals == null || !goals.Any()) return 0;

				// Cap each goal at 100% to avoid inflating the score
				var avgProgress = goals.Average(g => Math.Min(g.ProgressPercentage, 100m));
				return Math.Round(avgProgress / 100m * 25m, 2);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] Goal score unavailable for employee {Id}", employeeId);
				return 0;
			}
		}

		// ── Factor 4: Feedback Submitted (max 10) ─────────────────────────
		// Pure local DB — no HTTP call needed
		private async Task<decimal> CalculateFeedbackScoreAsync(Guid employeeId, Guid programId)
		{
			try
			{
				var exists = await _feedbackRepository.ExistsAsync(
					f => f.EmployeeID == employeeId && f.ProgramID == programId);
				return exists ? 10m : 0m;
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] Feedback score unavailable for employee {Id}", employeeId);
				return 0;
			}
		}

		// ── Factor 5: Consistency – last 30 days (max 10) ─────────────────
		// Same fix as attendance — query /api/v1/activities, not /Sessions.
		private async Task<decimal> CalculateConsistencyScoreAsync(Guid employeeId, Guid programId, string jwt)
		{
			try
			{
				var client = WellnessClient(jwt);
				var res = await client.GetAsync($"/api/v1/activities?programId={programId}&pageSize=1000");
				if (!res.IsSuccessStatusCode) return 0;

				var activities = await ParsePagedAsync<ActivityResponse>(res);
				var cutoff = DateTime.UtcNow.AddDays(-30);

				var recentActivities = activities?
					.Where(a => a.Date >= cutoff
						&& a.Status != 5)   // 5 = StatusType.Cancelled
					.ToList() ?? new();

				if (recentActivities.Count == 0) return 0;

				int attended = await CountAttendedSessionsAsync(client, recentActivities, employeeId);
				return Math.Round((decimal)attended / recentActivities.Count * 10m, 2);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] Consistency score unavailable for employee {Id}", employeeId);
				return 0;
			}
		}

		// ── Bootstrap scan ─────────────────────────────────────────────────────
		/// <summary>
		/// GET /api/v1/activities (all programs) → for each activity GET attendance →
		/// returns every unique (EmployeeID, ProgramID) pair found in any roster.
		/// Allows RecalculateAll to bootstrap records for employees who have never
		/// had an event-driven trigger fire.
		/// </summary>
		public async Task<IEnumerable<(Guid EmployeeId, Guid ProgramId)>> ScanAllEmployeeProgramPairsAsync(string jwt)
		{
			var pairs = new HashSet<(Guid, Guid)>();
			try
			{
				var client = WellnessClient(jwt);

				// Fetch every activity (no programId filter)
				var res = await client.GetAsync("/api/v1/activities?pageSize=1000");
				if (!res.IsSuccessStatusCode) return pairs;

				var activities = await ParsePagedAsync<ActivityResponse>(res);
				if (activities == null) return pairs;

				foreach (var activity in activities)
				{
					try
					{
						var attRes = await client.GetAsync($"/api/v1/activities/{activity.Id}/attendance");
						if (!attRes.IsSuccessStatusCode) continue;

						var attendees = await ParseAsync<IEnumerable<AttendanceResponse>>(attRes);
						if (attendees == null) continue;

						foreach (var attendee in attendees)
							pairs.Add((attendee.EmployeeID, activity.ProgramID));
					}
					catch { /* skip individual activity */ }
				}

				_logger.LogInformation("[Engagement] Bootstrap scan found {Count} employee-program pairs", pairs.Count);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "[Engagement] ScanAllEmployeeProgramPairs failed — bootstrap skipped");
			}
			return pairs;
		}

		// ── Shared helpers ─────────────────────────────────────────────────

		/// <summary>
		/// For each activity, calls GET /api/v1/activities/{id}/attendance and
		/// counts the ones where this employee was marked Present.
		/// </summary>
		private async Task<int> CountAttendedSessionsAsync(
			HttpClient client,
			IEnumerable<ActivityResponse> activities,
			Guid employeeId)
		{
			int attended = 0;
			foreach (var activity in activities)
			{
				try
				{
					var attRes = await client.GetAsync($"/api/v1/activities/{activity.Id}/attendance");
					if (!attRes.IsSuccessStatusCode) continue;

					var attendees = await ParseAsync<IEnumerable<AttendanceResponse>>(attRes);
					if (attendees?.Any(a => a.EmployeeID == employeeId && a.IsPresent) == true)
						attended++;
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "[Engagement] Could not fetch attendance for activity {ActivityId}", activity.Id);
				}
			}
			return attended;
		}

		private HttpClient WellnessClient(string jwt)
		{
			var client = _httpClientFactory.CreateClient("WellnessClient");
			SetJwt(client, jwt);
			return client;
		}

		private HttpClient ParticipationClient(string jwt)
		{
			var client = _httpClientFactory.CreateClient("ParticipationClient");
			SetJwt(client, jwt);
			return client;
		}

		private static void SetJwt(HttpClient client, string jwt)
		{
			if (!string.IsNullOrWhiteSpace(jwt))
				client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(jwt);
		}

		private static async Task<T?> ParseAsync<T>(HttpResponseMessage response)
		{
			var json    = await response.Content.ReadAsStringAsync();
			var wrapper = JsonSerializer.Deserialize<ExternalApiResponse<T>>(json, _json);
			return wrapper is { Succeeded: true } ? wrapper.Data : default;
		}

		// Used for endpoints that return ApiResponse<PagedResult<T>> (activities, goals).
		private static async Task<IEnumerable<TItem>?> ParsePagedAsync<TItem>(HttpResponseMessage response)
		{
			var json    = await response.Content.ReadAsStringAsync();
			var wrapper = JsonSerializer.Deserialize<ExternalApiResponse<ExternalPagedResult<TItem>>>(json, _json);
			return wrapper is { Succeeded: true } ? wrapper.Data?.Items : default;
		}
	}
}
