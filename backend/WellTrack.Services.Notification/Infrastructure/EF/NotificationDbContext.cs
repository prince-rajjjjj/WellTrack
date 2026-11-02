using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Notification.Domain.Entities;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Notification.Infrastructure.EF;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationEntity> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("Notifications", "Notification");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate    = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var adminId     = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var hrId        = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var employeeId  = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var coachId     = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var fitnessProgramId     = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var mentalHealthProgramId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        modelBuilder.Entity<NotificationEntity>().HasData(
            new NotificationEntity
            {
                Id        = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
                UserID    = adminId,
                ProgramID = null,
                Message   = "System Maintenance scheduled for Sunday at 2:00 AM.",
                Category  = "System",
                Status    = StatusType.Active,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new NotificationEntity
            {
                Id        = Guid.Parse("e1000000-0000-0000-0000-000000000002"),
                UserID    = hrId,
                ProgramID = fitnessProgramId,
                Message   = "New compliance report is ready for Fitness Challenge 2026.",
                Category  = "Compliance",
                Status    = StatusType.Active,
                CreatedAt = seedDate.AddHours(1),
                UpdatedAt = seedDate.AddHours(1)
            },
            new NotificationEntity
            {
                Id        = Guid.Parse("e1000000-0000-0000-0000-000000000003"),
                UserID    = employeeId,
                ProgramID = fitnessProgramId,
                Message   = "Your participation in 'Morning Yoga' has been confirmed.",
                Category  = "Event",
                Status    = StatusType.Completed,
                CreatedAt = seedDate.AddHours(2),
                UpdatedAt = seedDate.AddHours(2)
            },
            new NotificationEntity
            {
                Id        = Guid.Parse("e1000000-0000-0000-0000-000000000004"),
                UserID    = coachId,
                ProgramID = mentalHealthProgramId,
                Message   = "You have been assigned as the lead coach for Mental Health Awareness.",
                Category  = "Assignment",
                Status    = StatusType.Active,
                CreatedAt = seedDate.AddDays(1),
                UpdatedAt = seedDate.AddDays(1)
            }
        );
    }
}
