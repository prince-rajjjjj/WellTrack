namespace WellTrack.Services.Engagement.Application.DTOs
{
    /// <summary>One row in the overall top-performers leaderboard (avg across all programs).</summary>
    public class OverallLeaderboardEntryDto
    {
        public int    Rank            { get; set; }
        public Guid   EmployeeId      { get; set; }
        public double AvgScore        { get; set; }
        public int    ProgramCount    { get; set; }  // how many programs they're enrolled in
        public string EngagementLevel { get; set; } = "Low";
        /// <summary>Positive = improving, negative = declining, null = first calculation.</summary>
        public double? ScoreDelta     { get; set; }
    }
}
