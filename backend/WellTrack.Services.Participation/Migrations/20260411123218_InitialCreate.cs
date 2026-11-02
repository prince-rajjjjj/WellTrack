using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Participation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Participation");

            migrationBuilder.EnsureSchema(
                name: "Wellness");

            migrationBuilder.CreateTable(
                name: "Goals",
                schema: "Participation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participations",
                schema: "Participation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoachID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoachName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TargetActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetActivityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Participation",
                table: "Participations",
                columns: new[] { "Id", "ActivityID", "CreatedAt", "CreatedBy", "Date", "EmployeeID", "Notes", "ProgramID", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 13, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), "Great session!", new Guid("2a174612-af40-4334-aef0-d82e6630accf"), 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 12, 14, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), "Very relaxing", new Guid("2a174612-af40-4334-aef0-d82e6630accf"), 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d3bf5d76-8a54-47da-943a-de3e06682ae5"), "In progress", new Guid("2a174612-af40-4334-aef0-d82e6630accf"), 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                schema: "Wellness",
                table: "Recommendations",
                columns: new[] { "Id", "Category", "CoachID", "CoachName", "CreatedAt", "CreatedBy", "CreatedDate", "Description", "ReviewedAt", "ReviewedBy", "Status", "TargetActivityID", "TargetActivityName", "TargetProgramID", "TargetProgramName", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000001"), "Fitness", new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Start each morning with a 10-minute full-body stretch to improve flexibility and reduce injury risk.", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "HR Manager", "Approved", null, null, null, null, "Daily 10-Minute Stretching Routine", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1000000-0000-0000-0000-000000000002"), "Mental Health", new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Practice 4-7-8 breathing technique twice daily: inhale 4 counts, hold 7, exhale 8. Reduces cortisol and improves focus.", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "HR Manager", "Approved", null, null, null, null, "Mindful Breathing Practice", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1000000-0000-0000-0000-000000000003"), "Nutrition", new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Maintain optimal hydration by drinking at least 8 glasses of water per day. Set hourly reminders to stay on track.", null, null, "Pending", null, null, null, null, "Hydration Goal: 8 Glasses Daily", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1000000-0000-0000-0000-000000000004"), "Sleep", new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Avoid screens 1 hour before bed. Replace with reading, light stretching, or journaling to improve sleep quality.", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "HR Manager", "Rejected", null, null, null, null, "Screen-Free Wind-Down Routine", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1000000-0000-0000-0000-000000000005"), "Fitness", new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Complete at least 150 minutes of moderate cardio per week. Track progress using the wellness app.", null, null, "Pending", null, null, null, null, "Weekly Cardio Challenge", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals",
                schema: "Participation");

            migrationBuilder.DropTable(
                name: "Participations",
                schema: "Participation");

            migrationBuilder.DropTable(
                name: "Recommendations",
                schema: "Wellness");
        }
    }
}
