using Microsoft.EntityFrameworkCore;
using WellTrack.Services.Compliance.Domain;
using WellTrack.Services.Compliance.Domain.Entities;
using WellTrack.SharedKernel.Enum;


namespace WellTrack.Services.Compliance.Infrastructure.EF;

public class ComplianceDbContext : DbContext
{
	public ComplianceDbContext(DbContextOptions<ComplianceDbContext> options) : base(options) { }

	public DbSet<Policy> Policies { get; set; }
	public DbSet<ComplianceRecord> ComplianceRecords { get; set; }
	public DbSet<Audit> Audits { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		
		modelBuilder.Entity<Policy>(entity =>
		{
			entity.ToTable("Policies", "Compliance");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.PolicyName).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Severity).HasConversion<string>();
			entity.Property(e => e.Status).HasConversion<string>();
		});

		modelBuilder.Entity<ComplianceRecord>(entity =>
		{
			entity.ToTable("ComplianceRecords", "Compliance");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.IssueType).HasMaxLength(200);
			entity.Property(e => e.Notes).HasMaxLength(2000);
			entity.Property(e => e.Severity).HasConversion<string>();
			entity.Property(e => e.Status).HasConversion<string>();
			entity.Property(e => e.Type).HasMaxLength(100);
			entity.Property(e => e.Result).HasMaxLength(200);
		});

		modelBuilder.Entity<Audit>(entity =>
		{
			entity.ToTable("Audits", "Compliance");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Scope).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Findings).HasMaxLength(2000);
		});

		SeedData(modelBuilder);
	}

	private void SeedData(ModelBuilder modelBuilder)
	{
		var seedDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);
		var complianceOfficerId = Guid.Parse("55555555-5555-5555-5555-555555555555");

		var p1 = Guid.Parse("cc000001-0000-0000-0000-000000000001");
		var p2 = Guid.Parse("cc000001-0000-0000-0000-000000000002");
		var p3 = Guid.Parse("cc000001-0000-0000-0000-000000000003");
		var p4 = Guid.Parse("cc000001-0000-0000-0000-000000000004");

		modelBuilder.Entity<Policy>().HasData(
			new Policy { Id = p1, PolicyName = "Coach Certification Required", Description = "All coaches must hold a valid certification before conducting sessions.", Category = "Certification", Severity = PolicySeverity.High, Status = PolicyStatus.Active, CreatedDate = seedDate, CreatedAt = seedDate },
			new Policy { Id = p2, PolicyName = "Session Duration Limit", Description = "Sessions must not exceed 120 minutes without a mandatory break.", Category = "Safety", Severity = PolicySeverity.Medium, Status = PolicyStatus.Active, CreatedDate = seedDate, CreatedAt = seedDate },
			new Policy { Id = p3, PolicyName = "Attendance Recording Required", Description = "Attendance must be recorded for every session within 24 hours.", Category = "Documentation", Severity = PolicySeverity.Medium, Status = PolicyStatus.Active, CreatedDate = seedDate, CreatedAt = seedDate },
			new Policy { Id = p4, PolicyName = "Health & Safety Guidelines", Description = "All activities must comply with company health and safety standards.", Category = "Safety", Severity = PolicySeverity.Critical, Status = PolicyStatus.Active, CreatedDate = seedDate, CreatedAt = seedDate }
		);

		var programId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
		var r1 = Guid.Parse("cc000002-0000-0000-0000-000000000001");
		var r2 = Guid.Parse("cc000002-0000-0000-0000-000000000002");
		var r3 = Guid.Parse("cc000002-0000-0000-0000-000000000003");

		modelBuilder.Entity<ComplianceRecord>().HasData(
			new ComplianceRecord { Id = r1, ProgramID = programId, PolicyID = p2, IssueType = "Duration Exceeded", Severity = ComplianceSeverity.Medium, Notes = "Session ran 140 minutes without a break.", Status = ComplianceRecordStatus.Reported, Type = "Violation", Result = "Reported", CreatedDate = seedDate, Date = seedDate, CreatedAt = seedDate },
			new ComplianceRecord { Id = r2, ProgramID = programId, PolicyID = p3, IssueType = "Missing Attendance Record", Severity = ComplianceSeverity.Low, Notes = "Attendance not recorded for March 8 HIIT session.", Status = ComplianceRecordStatus.Closed, Type = "Observation", Result = "Closed", CreatedDate = seedDate.AddDays(2), Date = seedDate.AddDays(2), CreatedAt = seedDate.AddDays(2) },
			new ComplianceRecord { Id = r3, ProgramID = programId, PolicyID = p4, IssueType = "Safety Protocol Breach", Severity = ComplianceSeverity.High, Notes = "Participants not briefed on safety guidelines before activity.", Status = ComplianceRecordStatus.InProgress, AssignedTo = "HR", Type = "Violation", Result = "InProgress", CreatedDate = seedDate.AddDays(5), Date = seedDate.AddDays(5), CreatedAt = seedDate.AddDays(5) }
		);

		var a1 = Guid.Parse("cc000003-0000-0000-0000-000000000001");
		var a2 = Guid.Parse("cc000003-0000-0000-0000-000000000002");

		modelBuilder.Entity<Audit>().HasData(
			new Audit { Id = a1, ComplianceOfficerID = complianceOfficerId, Scope = "Fitness Challenge 2026 - Q1 Review", Findings = "Two open violations found. Coach certification verified. Attendance records incomplete for 2 sessions.", Date = seedDate.AddDays(7), AuditStatus = AuditStatus.Approved, Status = StatusType.Completed, CreatedAt = seedDate.AddDays(7) },
			new Audit { Id = a2, ComplianceOfficerID = complianceOfficerId, Scope = "Mental Health Awareness Program - Safety Audit", Findings = null, Date = seedDate.AddDays(10), AuditStatus = AuditStatus.Draft, Status = StatusType.Pending, CreatedAt = seedDate.AddDays(10) }
		);
	}
}
