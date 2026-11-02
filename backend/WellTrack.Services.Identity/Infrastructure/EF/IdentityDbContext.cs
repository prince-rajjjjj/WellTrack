using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Identity.Domain;
using WellTrack.Services.Identity.Domain.Entities;

namespace WellTrack.Services.Identity.Infrastructure.EF;

public class IdentityDbContext : DbContext
{
	public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
	{
	}

	public DbSet<User> Users { get; set; }
	public DbSet<AuditLog> AuditLogs { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<User>(entity =>
		{
			entity.ToTable("Users", "Identity");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
			entity.HasIndex(e => e.Email).IsUnique();
			entity.Property(e => e.PasswordHash).IsRequired();
			entity.Property(e => e.Phone).HasMaxLength(20);
		});

		modelBuilder.Entity<AuditLog>(entity =>
		{
			entity.ToTable("AuditLogs", "Identity");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Resource).IsRequired().HasMaxLength(200);
		});

		SeedData(modelBuilder);
	}

	private void SeedData(ModelBuilder modelBuilder)
{
    var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var hrId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    var employeeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    var coachId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    // Static hash for "WellTrack@123" - generated once to remain constant
    const string staticHash = "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6";
    var seedDate = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc);

    var users = new List<User>
    {
        new User { Id = adminId, Name = "Admin User", Email = "admin@welltrack.com", PasswordHash = staticHash, Role = SharedKernel.Enum.UserRole.Admin, Phone = "+1234567890", Status = SharedKernel.Enum.StatusType.Active, CreatedAt = seedDate },
        new User { Id = hrId, Name = "HR Manager", Email = "hr@welltrack.com", PasswordHash = staticHash, Role = SharedKernel.Enum.UserRole.HR, Phone = "+1234567891", Status = SharedKernel.Enum.StatusType.Active, CreatedAt = seedDate },
        new User { Id = employeeId, Name = "John Employee", Email = "employee@welltrack.com", PasswordHash = staticHash, Role = SharedKernel.Enum.UserRole.Employee, Phone = "+1234567892", Status = SharedKernel.Enum.StatusType.Active, CreatedAt = seedDate },
        new User { Id = coachId, Name = "Coach Alex", Email = "coach@welltrack.com", PasswordHash = staticHash, Role = SharedKernel.Enum.UserRole.Coach, Phone = "+1234567893", Status = SharedKernel.Enum.StatusType.Active, CreatedAt = seedDate }
    };

		// ... (Arrays for names/depts remain the same)
		var firstNames = new[] { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
								 "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen",
								 "Christopher", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra",
								 "Donald", "Ashley", "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle",
								 "Kenneth", "Carol", "Kevin", "Amanda", "Brian", "Dorothy", "George", "Melissa", "Timothy", "Deborah" };

		var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
								"Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
								"Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
								"Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
								"Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts" };

		var departments = new[] { "Engineering", "Marketing", "Sales", "HR", "Finance", "Operations", "IT", "Customer Service", "Product", "Design" };
		var statuses = new[] { SharedKernel.Enum.StatusType.Active, SharedKernel.Enum.StatusType.Active, SharedKernel.Enum.StatusType.Active,
							   SharedKernel.Enum.StatusType.Inactive, SharedKernel.Enum.StatusType.Pending };



		var random = new Random(42); // Fixed seed for consistency

    for (int i = 0; i < 50; i++)
    {
        var firstName = firstNames[i % firstNames.Length];
        var lastName = lastNames[i % lastNames.Length];
        
        // Generate a deterministic GUID so it never changes between runs
        var deterministicId = Guid.Parse($"55555555-5555-5555-5555-{i:D12}");

        users.Add(new User
        {
            Id = deterministicId,
            Name = $"{firstName} {lastName}",
            Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@welltrack.com",
            PasswordHash = staticHash,
            Role = SharedKernel.Enum.UserRole.Employee,
            Phone = $"+1555{i:D6}",
            Status = SharedKernel.Enum.StatusType.Active,
            CreatedAt = seedDate.AddDays(-i)
        });
    }

    modelBuilder.Entity<User>().HasData(users);
}
}
