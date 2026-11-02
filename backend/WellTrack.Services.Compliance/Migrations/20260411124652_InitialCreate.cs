using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Compliance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Compliance");

            migrationBuilder.CreateTable(
                name: "Audits",
                schema: "Compliance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplianceOfficerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceRecords",
                schema: "Compliance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                schema: "Compliance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Compliance",
                table: "Audits",
                columns: new[] { "Id", "AuditStatus", "ComplianceOfficerID", "CreatedAt", "CreatedBy", "Date", "Findings", "RejectionReason", "Scope", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cc000003-0000-0000-0000-000000000001"), 3, new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Two open violations found. Coach certification verified. Attendance records incomplete for 2 sessions.", null, "Fitness Challenge 2026 - Q1 Review", 4, null, null },
                    { new Guid("cc000003-0000-0000-0000-000000000002"), 1, new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Mental Health Awareness Program - Safety Audit", 3, null, null }
                });

            migrationBuilder.InsertData(
                schema: "Compliance",
                table: "ComplianceRecords",
                columns: new[] { "Id", "ActivityID", "AssignedTo", "AssignedToId", "CreatedAt", "CreatedBy", "CreatedDate", "Date", "IssueType", "Notes", "PolicyID", "ProgramID", "ResolutionNotes", "Result", "Severity", "Status", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cc000002-0000-0000-0000-000000000001"), null, null, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Duration Exceeded", "Session ran 140 minutes without a break.", new Guid("cc000001-0000-0000-0000-000000000002"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "Reported", "Medium", "Reported", "Violation", null, null },
                    { new Guid("cc000002-0000-0000-0000-000000000002"), null, null, null, new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Missing Attendance Record", "Attendance not recorded for March 8 HIIT session.", new Guid("cc000001-0000-0000-0000-000000000003"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "Resolved", "Low", "Resolved", "Observation", null, null },
                    { new Guid("cc000002-0000-0000-0000-000000000003"), null, "HR", null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Safety Protocol Breach", "Participants not briefed on safety guidelines before activity.", new Guid("cc000001-0000-0000-0000-000000000004"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "Assigned", "High", "Assigned", "Violation", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Compliance",
                table: "Policies",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "CreatedDate", "Description", "PolicyName", "Severity", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cc000001-0000-0000-0000-000000000001"), "Certification", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "All coaches must hold a valid certification before conducting sessions.", "Coach Certification Required", "High", "Active", null, null },
                    { new Guid("cc000001-0000-0000-0000-000000000002"), "Safety", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sessions must not exceed 120 minutes without a mandatory break.", "Session Duration Limit", "Medium", "Active", null, null },
                    { new Guid("cc000001-0000-0000-0000-000000000003"), "Documentation", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Attendance must be recorded for every session within 24 hours.", "Attendance Recording Required", "Medium", "Active", null, null },
                    { new Guid("cc000001-0000-0000-0000-000000000004"), "Safety", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "All activities must comply with company health and safety standards.", "Health & Safety Guidelines", "Critical", "Active", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits",
                schema: "Compliance");

            migrationBuilder.DropTable(
                name: "ComplianceRecords",
                schema: "Compliance");

            migrationBuilder.DropTable(
                name: "Policies",
                schema: "Compliance");
        }
    }
}
