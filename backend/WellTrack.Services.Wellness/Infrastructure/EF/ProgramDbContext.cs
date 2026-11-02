using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Wellness.Domain.Entities;

namespace WellTrack.Services.Wellness.Infrastructure.EF;

public class ProgramDbContext : DbContext
{
	public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options) { }

	public DbSet<programs> Programs { get; set; }
	public DbSet<Activity> Activities { get; set; }
	public DbSet<ActivityAttendance> ActivityAttendances { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<programs>(entity =>
		{
			entity.ToTable("Programs", "Wellness");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.HasMany(e => e.Activities)
				.WithOne(a => a.Program)
				.HasForeignKey(a => a.ProgramID)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Activity>(entity =>
		{
			entity.ToTable("Activities", "Wellness");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.Property(e => e.AssignedCoachName).HasMaxLength(200);
			entity.Property(e => e.Location).HasMaxLength(300);
			entity.Property(e => e.VideoUrl).HasMaxLength(1000);
			entity.Property(e => e.Mode).HasConversion<string>();
			entity.Property(e => e.Status).HasConversion<string>();
		});

		modelBuilder.Entity<ActivityAttendance>(entity =>
		{
			entity.ToTable("ActivityAttendances", "Wellness");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.EmployeeName).HasMaxLength(200);
			entity.Property(e => e.Notes).HasMaxLength(1000);
			entity.HasIndex(e => e.ActivityID);
		});

		SeedData(modelBuilder);
	}

	private void SeedData(ModelBuilder modelBuilder)
	{
		var hrId       = Guid.Parse("22222222-2222-2222-2222-222222222222");
		var coachId    = Guid.Parse("44444444-4444-4444-4444-444444444444");
		var program1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
		var program2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
		var seedDate   = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

		modelBuilder.Entity<programs>().HasData(
			new programs
			{
				Id = program1Id,
				Title = "Fitness Challenge 2026",
				Description = "A 12-week fitness challenge to improve overall health and wellness",
				HRID = hrId,
				StartDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
				EndDate   = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
				Status    = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new programs
			{
				Id = program2Id,
				Title = "Mental Health Awareness",
				Description = "Monthly sessions focused on mental health and stress management",
				HRID = hrId,
				StartDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
				EndDate   = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
				Status    = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			}
		);

		modelBuilder.Entity<Activity>().HasData(
			// ── Fitness Challenge 2026 ──────────────────────────
			new Activity
			{
				Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
				ProgramID = program1Id,
				Title = "Morning Yoga", Type = "Yoga",
				Description = "Energizing yoga at the office gym.",
				Date = new DateTime(2026, 4, 5, 7, 0, 0, DateTimeKind.Utc),
				Duration = 60, MaxParticipants = 25,
				Mode = ActivityMode.Offline,
				Location = "Chennai > SDB1 > 4th floor > ODC 5",
				AssignedCoachID = coachId, AssignedCoachName = "Coach Alex",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new Activity
			{
				Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
				ProgramID = program1Id,
				Title = "Running Form 101", Type = "Running",
				Description = "Watch this video on your own time to improve your running form.",
				Date = new DateTime(2026, 4, 10, 6, 30, 0, DateTimeKind.Utc),
				Duration = 25, MaxParticipants = 100,
				Mode = ActivityMode.Online,
				VideoUrl = "https://www.youtube.com/watch?v=brFHyOtTwH4",
				AssignedCoachID = coachId, AssignedCoachName = "Coach Alex",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},

			// ── Mental Health Awareness ─────────────────────────
			new Activity
			{
				Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
				ProgramID = program2Id,
				Title = "Mindfulness Workshop", Type = "Workshop",
				Description = "Hands-on mindfulness practice in person.",
				Date = new DateTime(2026, 5, 7, 14, 0, 0, DateTimeKind.Utc),
				Duration = 90, MaxParticipants = 30,
				Mode = ActivityMode.Offline,
				Location = "Chennai > SDB2 > 2nd floor > Conference Room B",
				AssignedCoachID = coachId, AssignedCoachName = "Coach Alex",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new Activity
			{
				Id = Guid.Parse("88888888-8888-8888-8888-888888888889"),
				ProgramID = program2Id,
				Title = "Guided Meditation", Type = "Meditation",
				Description = "10-minute guided meditation session — watch when you need a calm reset.",
				Date = new DateTime(2026, 5, 8, 11, 0, 0, DateTimeKind.Utc),
				Duration = 10, MaxParticipants = 200,
				Mode = ActivityMode.Online,
				VideoUrl = "https://www.youtube.com/watch?v=inpok4MKVLM",
				AssignedCoachID = coachId, AssignedCoachName = "Coach Alex",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new Activity
			{
				Id = Guid.Parse("8888888a-8888-8888-8888-888888888888"),
				ProgramID = program2Id,
				Title = "Stress Management Talk", Type = "Workshop",
				Description = "In-person workshop on managing workplace stress.",
				Date = new DateTime(2026, 5, 12, 16, 0, 0, DateTimeKind.Utc),
				Duration = 60, MaxParticipants = 40,
				Mode = ActivityMode.Offline,
				Location = "Chennai > SDB1 > 6th floor > Auditorium",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new Activity
			{
				Id = Guid.Parse("8888888b-8888-8888-8888-888888888888"),
				ProgramID = program2Id,
				Title = "Breathing Techniques", Type = "Meditation",
				Description = "Watch this short video to learn box-breathing for instant calm.",
				Date = new DateTime(2026, 5, 15, 10, 30, 0, DateTimeKind.Utc),
				Duration = 8, MaxParticipants = 200,
				Mode = ActivityMode.Online,
				VideoUrl = "https://www.youtube.com/watch?v=tEmt1Znux58",
				AssignedCoachID = coachId, AssignedCoachName = "Coach Alex",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			},
			new Activity
			{
				Id = Guid.Parse("8888888c-8888-8888-8888-888888888888"),
				ProgramID = program2Id,
				Title = "Sleep Hygiene Session", Type = "Workshop",
				Description = "Group discussion on building a healthier sleep routine.",
				Date = new DateTime(2026, 5, 20, 15, 0, 0, DateTimeKind.Utc),
				Duration = 45, MaxParticipants = 25,
				Mode = ActivityMode.Offline,
				Location = "Chennai > SDB2 > 3rd floor > Conference Room A",
				Status = SharedKernel.Enum.StatusType.Active,
				CreatedAt = seedDate
			}
		);
	}
}
