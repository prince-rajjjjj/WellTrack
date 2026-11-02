using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Engagement.Domain.Entities;

namespace WellTrack.Services.Engagement.Infrastructure.EF;

public class EngagementDbContext : DbContext
{
	public EngagementDbContext(DbContextOptions<EngagementDbContext> options) : base(options) { }

	public DbSet<Feedback> Feedbacks { get; set; }
	public DbSet<EngagementEntity> Engagements { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// ── Feedback ─────────────────────────────────────────────────────
		modelBuilder.Entity<Feedback>(entity =>
		{
			entity.ToTable("Feedbacks", "Engagement");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Rating).IsRequired();
			entity.Property(e => e.Comments).HasMaxLength(1000);
			entity.Property(e => e.Date).HasDefaultValueSql("GETUTCDATE()");
		});

		// ── EngagementEntity ──────────────────────────────────────────────
		modelBuilder.Entity<EngagementEntity>(entity =>
		{
			entity.ToTable("Engagements", "Engagement");
			entity.HasKey(e => e.Id);

			// One record per employee+program (enforced at service layer via upsert)
			entity.HasIndex(e => new { e.EmployeeID, e.ProgramID }).IsUnique();

			// Precision for all decimal score columns
			entity.Property(e => e.TotalScore).HasPrecision(18, 2);
			entity.Property(e => e.AttendanceScore).HasPrecision(18, 2);
			entity.Property(e => e.ActivityScore).HasPrecision(18, 2);
			entity.Property(e => e.GoalScore).HasPrecision(18, 2);
			entity.Property(e => e.FeedbackScore).HasPrecision(18, 2);
			entity.Property(e => e.ConsistencyScore).HasPrecision(18, 2);
			entity.Property(e => e.PreviousScore).HasPrecision(18, 2);

			entity.Property(e => e.EngagementLevel).HasMaxLength(10).HasDefaultValue("Low");
			entity.Property(e => e.LastCalculatedAt).HasDefaultValueSql("GETUTCDATE()");
		});

		// Seed data intentionally removed.
		//
		// Engagement and feedback records are produced by:
		//   • Employee submitting feedback   → POST /api/v1/feedback
		//   • HR/Admin triggering recalc      → POST /api/v1/engagement/recalculate
		//   • Server-to-server triggers from Wellness/Participation after attendance
		//     or goal updates.
		//
		// The previous mock seeds referenced employee/program GUIDs that did not
		// exist in any other service, which made every dashboard/leaderboard read
		// like junk. Leaving this empty keeps a fresh database honest — engagement
		// records only appear once real activity has happened.
	}
}
