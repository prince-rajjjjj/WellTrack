namespace WellTrack.Services.Engagement.Application.DTOs
{
    /// <summary>Four KPI cards shown at the very top of the HR dashboard.</summary>
    public class OrgSummaryDto
    {
        public int    TotalTracked     { get; set; }   // employees with ≥1 engagement record
        public double OrgAverageScore  { get; set; }
        public int    HighCount        { get; set; }
        public int    MediumCount      { get; set; }
        public int    LowCount         { get; set; }
        public double PercentHigh      { get; set; }
        public double PercentAtRisk    { get; set; }   // % with Low level
    }
}
