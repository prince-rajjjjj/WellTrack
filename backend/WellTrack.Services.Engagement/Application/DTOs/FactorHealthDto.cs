namespace WellTrack.Services.Engagement.Application.DTOs
{
    /// <summary>
    /// Org-wide average for each of the 5 engagement factors,
    /// expressed both as raw points and as % of that factor's max.
    /// Lets HR see WHICH factor is dragging scores down.
    /// </summary>
    public class FactorHealthDto
    {
        // Raw averages
        public double AttendanceAvg   { get; set; }   // max 30
        public double ActivityAvg     { get; set; }   // max 25
        public double GoalAvg         { get; set; }   // max 25
        public double FeedbackAvg     { get; set; }   // max 10
        public double ConsistencyAvg  { get; set; }   // max 10

        // % of maximum (0-100) — used for the progress bars
        public double AttendancePct   { get; set; }
        public double ActivityPct     { get; set; }
        public double GoalPct         { get; set; }
        public double FeedbackPct     { get; set; }
        public double ConsistencyPct  { get; set; }
    }
}
