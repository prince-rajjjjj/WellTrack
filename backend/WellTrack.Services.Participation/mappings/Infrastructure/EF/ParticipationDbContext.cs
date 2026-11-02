using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Participation.Domain;
using WellTrack.Services.Participation.Domain.Entities;

namespace WellTrack.Services.Participation.Infrastructure.EF;

public class ParticipationDbContext : DbContext
{
	public ParticipationDbContext(DbContextOptions<ParticipationDbContext> options) : base(options)
	{
	}

	public DbSet<ParticipationEntity> Participations { get; set; }
	public DbSet<Goal> Goals { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ParticipationEntity>(entity =>
		{
			entity.ToTable("Participations", "Participation");
			entity.HasKey(e => e.Id);
		});

		modelBuilder.Entity<Goal>(entity =>
		{
			entity.ToTable("Goals", "Participation");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
			entity.Property(e => e.TargetValue).HasPrecision(18, 2);
			entity.Property(e => e.CurrentValue).HasPrecision(18, 2);
		});

		// Seed participation data
		SeedData(modelBuilder);
	}

	private void SeedData(ModelBuilder modelBuilder)
	{
		var now = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc);

		var participations = new List<ParticipationEntity>
		{
			new ParticipationEntity
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
				ProgramID = Guid.Parse("2a174612-af40-4334-aef0-d82e6630accf"),
				EmployeeID = Guid.Parse("33333333-3333-3333-3333-333333333333"),
				ActivityID = Guid.Parse("66666666-6666-6666-6666-666666666666"),
				Date = DateTime.Parse("2026-03-13T08:00:00"),
				Status = SharedKernel.Enum.StatusType.Completed,
				Notes = "Great session!",
				CreatedAt = now,
				UpdatedAt = now
			},
			new ParticipationEntity
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
				ProgramID = Guid.Parse("2a174612-af40-4334-aef0-d82e6630accf"),
				EmployeeID = Guid.Parse("33333333-3333-3333-3333-333333333333"),
				ActivityID = Guid.Parse("77777777-7777-7777-7777-777777777777"),
				Date = DateTime.Parse("2026-03-12T14:00:00"),
				Status = SharedKernel.Enum.StatusType.Completed,
				Notes = "Very relaxing",
				CreatedAt = now,
				UpdatedAt = now
			},
			new ParticipationEntity
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
				ProgramID = Guid.Parse("2a174612-af40-4334-aef0-d82e6630accf"),
				EmployeeID = Guid.Parse("d3bf5d76-8a54-47da-943a-de3e06682ae5"),
				ActivityID = Guid.Parse("66666666-6666-6666-6666-666666666666"),
				Date = DateTime.Parse("2026-03-10T09:00:00"),
				Status = SharedKernel.Enum.StatusType.Active,
				Notes = "In progress",
				CreatedAt = now,
				UpdatedAt = now
			}
		};

		modelBuilder.Entity<ParticipationEntity>().HasData(participations);
	}
}
