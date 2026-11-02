using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Reporting.Domain.Entities;
namespace WellTrack.Services.Reporting.Infrastructure.EF
{
	public class ReportingDbContext : DbContext
	{
		public ReportingDbContext(DbContextOptions<ReportingDbContext> options)
			: base(options)
		{
		}

		public DbSet<Report> Reports { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Report>(entity =>
			{
				// Organizing under a "Reporting" schema to stay consistent with your "Wellness" and "Participation" schemasentity.ToTable("Reports", "Reporting");

				entity.HasKey(e => e.Id);
				// Setting default value for CreatedAt to ensure it's always populated
				entity.Property(e => e.CreatedAt)
			.HasDefaultValueSql("GETUTCDATE()");

				// Setting constraints to prevent NVARCHAR(MAX) and improve performance
				entity.Property(e => e.Scope)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.Metrics)
					.IsRequired()
					.HasMaxLength(2000); // Metrics JSON or descriptions can be long
	entity.Property(e => e.GeneratedBy)
					.IsRequired();
			});

			// Seed initial report dataSeedData(modelBuilder);
		}

		private void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Report>().HasData(
				new Report
				{
					Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"),
					Scope = "Monthly Wellness",
					Metrics = "ParticipationRate: 85%, AvgScore: 7.5",
					GeneratedBy = Guid.Parse("44444444-4444-4444-4444-444444444444"),
					GeneratedDate = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc)
				},
				new Report
				{
					Id = Guid.Parse("f1000000-0000-0000-0000-000000000002"),
					Scope = "Activity Engagement",
					Metrics = "MostActive: Yoga, LeastActive: Stretching",
					GeneratedBy = Guid.Parse("44444444-4444-4444-4444-444444444444"),
					GeneratedDate = new DateTime(2026, 3, 15, 15, 30, 0, DateTimeKind.Utc)
				}
			);
		}
	}
}
