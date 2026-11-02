namespace WellTrack.Services.Engagement.Application.DTOs
{
    /// <summary>One row in the "Most Improved" leaderboard — biggest positive score delta.</summary>
    public class MostImprovedEntryDto
    {
        public int    Rank           { get; set; }
        public Guid   EmployeeId     { get; set; }
        public double CurrentScore   { get; set; }
        public double PreviousScore  { get; set; }
        public double Delta          { get; set; }   // CurrentScore - PreviousScore
        public string EngagementLevel { get; set; } = "Low";
    }
}
