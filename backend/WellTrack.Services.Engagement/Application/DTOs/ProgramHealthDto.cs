namespace WellTrack.Services.Engagement.Application.DTOs
{
    /// <summary>
    /// One row in the Program Health table — shows High/Medium/Low split
    /// and avg score for every program so HR can spot at-risk programs at a glance.
    /// </summary>
    public class ProgramHealthDto
    {
        public Guid    ProgramId      { get; set; }
        public int     TotalEmployees { get; set; }
        public double  AvgScore       { get; set; }
        public int     HighCount      { get; set; }
        public int     MediumCount    { get; set; }
        public int     LowCount       { get; set; }
        /// <summary>Avg (TotalScore - PreviousScore) across employees in this program. Null if no baselines exist yet.</summary>
        public double? Trend          { get; set; }
    }
}
