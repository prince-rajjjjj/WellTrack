using AutoMapper;
using WellTrack.Services.Engagement.Application.DTOs;
using WellTrack.Services.Engagement.Domain.Entities;
using WellTrack.Services.Engagement.Repositories;

namespace WellTrack.Services.Engagement.Application.Services
{
    public class EngagementService : IEngagementService
    {
        private readonly IEngagementRepository _repository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEngagementCalculationService _calculationService;
        private readonly IMapper _mapper;
        private readonly ILogger<EngagementService> _logger;

        public EngagementService(
            IEngagementRepository repository,
            IFeedbackRepository feedbackRepository,
            IEngagementCalculationService calculationService,
            IMapper mapper,
            ILogger<EngagementService> logger)
        {
            _repository          = repository;
            _feedbackRepository  = feedbackRepository;
            _calculationService  = calculationService;
            _mapper              = mapper;
            _logger              = logger;
        }

        // ── HR / Admin raw records ─────────────────────────────────────────

        public async Task<IEnumerable<HrEngagementDto>> GetAllAsync()
        {
            var records = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<HrEngagementDto>>(
                records.OrderByDescending(e => e.LastCalculatedAt));
        }

        public async Task<IEnumerable<HrEngagementDto>> GetByEmployeeIdAsync(Guid employeeId)
        {
            var records = await _repository.FindAsync(e => e.EmployeeID == employeeId);
            return _mapper.Map<IEnumerable<HrEngagementDto>>(
                records.OrderByDescending(e => e.LastCalculatedAt));
        }

        public async Task<IEnumerable<HrEngagementDto>> GetByProgramIdAsync(Guid programId)
        {
            var records = await _repository.FindAsync(e => e.ProgramID == programId);
            return _mapper.Map<IEnumerable<HrEngagementDto>>(
                records.OrderByDescending(e => e.TotalScore));
        }

        // ── HR Dashboard analytics ─────────────────────────────────────────

        public async Task<OrgSummaryDto> GetOrgSummaryAsync()
        {
            var all = (await _repository.GetAllAsync()).ToList();
            if (!all.Any())
                return new OrgSummaryDto();

            // One aggregate per employee (avg across programs)
            var perEmployee = all
                .GroupBy(e => e.EmployeeID)
                .Select(g => new
                {
                    AvgScore = (double)g.Average(e => e.TotalScore),
                    Level    = g.Average(e => e.TotalScore) >= 75 ? "High"
                             : g.Average(e => e.TotalScore) >= 40 ? "Medium" : "Low"
                })
                .ToList();

            var high   = perEmployee.Count(x => x.Level == "High");
            var medium = perEmployee.Count(x => x.Level == "Medium");
            var low    = perEmployee.Count(x => x.Level == "Low");
            var total  = perEmployee.Count;

            return new OrgSummaryDto
            {
                TotalTracked    = total,
                OrgAverageScore = Math.Round(perEmployee.Average(x => x.AvgScore), 1),
                HighCount       = high,
                MediumCount     = medium,
                LowCount        = low,
                PercentHigh     = total > 0 ? Math.Round((double)high   / total * 100, 1) : 0,
                PercentAtRisk   = total > 0 ? Math.Round((double)low    / total * 100, 1) : 0,
            };
        }

        public async Task<IEnumerable<OverallLeaderboardEntryDto>> GetOverallLeaderboardAsync(int top = 10)
        {
            var all = (await _repository.GetAllAsync()).ToList();

            return all
                .GroupBy(e => e.EmployeeID)
                .Select(g =>
                {
                    var avgScore   = (double)g.Average(e => e.TotalScore);
                    var avgPrev    = g.Any(e => e.PreviousScore.HasValue)
                        ? (double?)g.Where(e => e.PreviousScore.HasValue)
                                    .Average(e => e.PreviousScore!.Value)
                        : null;
                    var delta      = avgPrev.HasValue ? Math.Round(avgScore - avgPrev.Value, 1) : (double?)null;
                    var level      = avgScore >= 75 ? "High" : avgScore >= 40 ? "Medium" : "Low";

                    return new OverallLeaderboardEntryDto
                    {
                        EmployeeId      = g.Key,
                        AvgScore        = Math.Round(avgScore, 1),
                        ProgramCount    = g.Count(),
                        EngagementLevel = level,
                        ScoreDelta      = delta,
                    };
                })
                .OrderByDescending(x => x.AvgScore)
                .Take(top)
                .Select((x, i) => { x.Rank = i + 1; return x; })
                .ToList();
        }

        public async Task<IEnumerable<MostImprovedEntryDto>> GetMostImprovedAsync(int top = 10)
        {
            var all = (await _repository.GetAllAsync())
                .Where(e => e.PreviousScore.HasValue)
                .ToList();

            return all
                .GroupBy(e => e.EmployeeID)
                .Select(g =>
                {
                    var current  = (double)g.Average(e => e.TotalScore);
                    var previous = (double)g.Average(e => e.PreviousScore!.Value);
                    var delta    = current - previous;
                    return new MostImprovedEntryDto
                    {
                        EmployeeId      = g.Key,
                        CurrentScore    = Math.Round(current,  1),
                        PreviousScore   = Math.Round(previous, 1),
                        Delta           = Math.Round(delta,    1),
                        EngagementLevel = current >= 75 ? "High" : current >= 40 ? "Medium" : "Low",
                    };
                })
                .Where(x => x.Delta > 0)                    // only positive movers
                .OrderByDescending(x => x.Delta)
                .Take(top)
                .Select((x, i) => { x.Rank = i + 1; return x; })
                .ToList();
        }

        public async Task<FactorHealthDto> GetFactorHealthAsync()
        {
            var all = (await _repository.GetAllAsync()).ToList();
            if (!all.Any())
                return new FactorHealthDto();

            var attendance  = (double)all.Average(e => e.AttendanceScore);
            var activity    = (double)all.Average(e => e.ActivityScore);
            var goal        = (double)all.Average(e => e.GoalScore);
            var feedback    = (double)all.Average(e => e.FeedbackScore);
            var consistency = (double)all.Average(e => e.ConsistencyScore);

            return new FactorHealthDto
            {
                AttendanceAvg  = Math.Round(attendance,  1),
                ActivityAvg    = Math.Round(activity,    1),
                GoalAvg        = Math.Round(goal,        1),
                FeedbackAvg    = Math.Round(feedback,    1),
                ConsistencyAvg = Math.Round(consistency, 1),
                AttendancePct  = Math.Round(attendance  / 30  * 100, 1),
                ActivityPct    = Math.Round(activity    / 25  * 100, 1),
                GoalPct        = Math.Round(goal        / 25  * 100, 1),
                FeedbackPct    = Math.Round(feedback    / 10  * 100, 1),
                ConsistencyPct = Math.Round(consistency / 10  * 100, 1),
            };
        }

        public async Task<IEnumerable<ProgramHealthDto>> GetProgramHealthAsync()
        {
            var all = (await _repository.GetAllAsync()).ToList();

            return all
                .GroupBy(e => e.ProgramID)
                .Select(g =>
                {
                    var trend = g.Any(e => e.PreviousScore.HasValue)
                        ? (double?)Math.Round(
                            g.Where(e => e.PreviousScore.HasValue)
                             .Average(e => (double)(e.TotalScore - e.PreviousScore!.Value)), 1)
                        : null;

                    return new ProgramHealthDto
                    {
                        ProgramId      = g.Key,
                        TotalEmployees = g.Count(),
                        AvgScore       = Math.Round((double)g.Average(e => e.TotalScore), 1),
                        HighCount      = g.Count(e => e.EngagementLevel == "High"),
                        MediumCount    = g.Count(e => e.EngagementLevel == "Medium"),
                        LowCount       = g.Count(e => e.EngagementLevel == "Low"),
                        Trend          = trend,
                    };
                })
                .OrderByDescending(x => x.AvgScore)
                .ToList();
        }

        // ── Employee personal view ─────────────────────────────────────────

        public async Task<IEnumerable<EmployeeEngagementDto>> GetMyEngagementAsync(Guid employeeId)
        {
            var records = await _repository.FindAsync(e => e.EmployeeID == employeeId);
            return records
                .OrderByDescending(e => e.TotalScore)
                .Select(e => new EmployeeEngagementDto
                {
                    ProgramID        = e.ProgramID,
                    TotalScore       = e.TotalScore,
                    EngagementLevel  = e.EngagementLevel,
                    PreviousScore    = e.PreviousScore,
                    Breakdown = new ScoreBreakdownDto
                    {
                        AttendanceScore  = e.AttendanceScore,
                        ActivityScore    = e.ActivityScore,
                        GoalScore        = e.GoalScore,
                        FeedbackScore    = e.FeedbackScore,
                        ConsistencyScore = e.ConsistencyScore
                    },
                    ImprovementTips  = BuildTips(e),
                    LastCalculatedAt = e.LastCalculatedAt
                })
                .ToList();
        }

        // ── Recalculate ────────────────────────────────────────────────────

        public async Task<HrEngagementDto> RecalculateAsync(Guid employeeId, Guid programId, string jwtToken)
        {
            var calculated = await _calculationService.CalculateAsync(employeeId, programId, jwtToken);

            var existing = (await _repository.FindAsync(
                e => e.EmployeeID == employeeId && e.ProgramID == programId))
                .FirstOrDefault();

            if (existing is null)
            {
                calculated.Id        = Guid.NewGuid();
                calculated.CreatedAt = DateTime.UtcNow;
                await _repository.AddAsync(calculated);
                return _mapper.Map<HrEngagementDto>(calculated);
            }

            // Snapshot current score as the new baseline before overwriting
            existing.PreviousScore    = existing.TotalScore;

            existing.TotalScore       = calculated.TotalScore;
            existing.EngagementLevel  = calculated.EngagementLevel;
            existing.AttendanceScore  = calculated.AttendanceScore;
            existing.ActivityScore    = calculated.ActivityScore;
            existing.GoalScore        = calculated.GoalScore;
            existing.FeedbackScore    = calculated.FeedbackScore;
            existing.ConsistencyScore = calculated.ConsistencyScore;
            existing.LastCalculatedAt = calculated.LastCalculatedAt;
            existing.UpdatedAt        = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<HrEngagementDto>(existing);
        }

        public async Task<int> RecalculateAllAsync(string jwtToken)
        {
            // ── Step 1: recalculate every record that already exists ──────────
            var existing     = (await _repository.GetAllAsync()).ToList();
            var existingKeys = existing
                .Select(e => (e.EmployeeID, e.ProgramID))
                .ToHashSet();

            var updated = 0;

            foreach (var record in existing)
            {
                try
                {
                    await RecalculateAsync(record.EmployeeID, record.ProgramID, jwtToken);
                    updated++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "[Engagement] RecalculateAll skipped {EmployeeID}/{ProgramID}",
                        record.EmployeeID, record.ProgramID);
                }
            }

            // ── Step 2: bootstrap records for any new pairs found in Wellness ─
            // Scans all activities + attendance rosters so employees who have
            // never had an event-driven trigger fire (e.g. first time setup)
            // also get a record created.
            var allPairs = await _calculationService.ScanAllEmployeeProgramPairsAsync(jwtToken);
            foreach (var (empId, progId) in allPairs)
            {
                if (existingKeys.Contains((empId, progId))) continue;   // already processed above
                try
                {
                    await RecalculateAsync(empId, progId, jwtToken);
                    updated++;
                    existingKeys.Add((empId, progId));  // avoid duplicates within this run
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "[Engagement] RecalculateAll bootstrap skipped {EmployeeID}/{ProgramID}",
                        empId, progId);
                }
            }

            _logger.LogInformation("[Engagement] RecalculateAll finished — {Updated} records updated/created", updated);
            return updated;
        }

        public async Task UpdateFeedbackScoreAsync(Guid employeeId, Guid programId)
        {
            var existing = (await _repository.FindAsync(
                e => e.EmployeeID == employeeId && e.ProgramID == programId))
                .FirstOrDefault();

            if (existing is null) return;

            var feedbackExists    = await _feedbackRepository.ExistsAsync(
                f => f.EmployeeID == employeeId && f.ProgramID == programId);

            existing.PreviousScore    = existing.TotalScore;
            existing.FeedbackScore    = feedbackExists ? 10m : 0m;
            existing.TotalScore       = Math.Round(
                existing.AttendanceScore + existing.ActivityScore +
                existing.GoalScore + existing.FeedbackScore + existing.ConsistencyScore, 2);
            existing.EngagementLevel  = existing.TotalScore >= 75 ? "High"
                : existing.TotalScore >= 40 ? "Medium" : "Low";
            existing.LastCalculatedAt = DateTime.UtcNow;
            existing.UpdatedAt        = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
        }

        // ── Legacy analytics ───────────────────────────────────────────────

        public async Task<EngagementSummaryDto> GetProgramSummaryAsync(Guid programId)
        {
            var records = (await _repository.FindAsync(e => e.ProgramID == programId)).ToList();
            return new EngagementSummaryDto
            {
                ProgramID        = programId,
                TotalEmployees   = records.Count,
                HighEngagement   = records.Count(e => e.EngagementLevel == "High"),
                MediumEngagement = records.Count(e => e.EngagementLevel == "Medium"),
                LowEngagement    = records.Count(e => e.EngagementLevel == "Low"),
                AverageScore     = records.Any()
                    ? Math.Round((double)records.Average(e => e.TotalScore), 2) : 0
            };
        }

        public async Task<double> GetAverageScoreByProgramAsync(Guid programId)
        {
            var records = await _repository.FindAsync(e => e.ProgramID == programId);
            return records.Any()
                ? Math.Round((double)records.Average(e => e.TotalScore), 2) : 0;
        }

        public async Task<IEnumerable<object>> GetLeaderboardAsync(Guid programId, int top = 10)
        {
            var records = await _repository.FindAsync(e => e.ProgramID == programId);
            return records
                .OrderByDescending(e => e.TotalScore)
                .Take(top)
                .Select((e, i) => (object)new
                {
                    rank            = i + 1,
                    employeeId      = e.EmployeeID,
                    totalScore      = e.TotalScore,
                    engagementLevel = e.EngagementLevel
                })
                .ToList();
        }

        // ── Helpers ────────────────────────────────────────────────────────

        private static List<string> BuildTips(EngagementEntity e)
        {
            var tips = new List<string>();
            if (e.AttendanceScore  < 15m)  tips.Add("Attend more scheduled sessions to improve your attendance score (max 30 pts).");
            if (e.ActivityScore    < 12.5m) tips.Add("Join more program activities to boost your activity score (max 25 pts).");
            if (e.GoalScore        < 12.5m) tips.Add("Keep logging your wellness goal progress regularly (max 25 pts).");
            if (e.FeedbackScore    == 0m)   tips.Add("Submit feedback for this program to earn your full feedback points (10 pts).");
            if (e.ConsistencyScore < 5m)    tips.Add("Try to attend sessions more consistently over the past 30 days (max 10 pts).");
            return tips;
        }
    }
}
