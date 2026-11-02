using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Participation.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recommendations",
                schema: "Wellness");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Wellness");

            migrationBuilder.CreateTable(
                name: "Recommendations",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CoachID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoachName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetActivityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TargetProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
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
    }
}
