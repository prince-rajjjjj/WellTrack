using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Notification.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Notification");

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Notification",
                table: "Notifications",
                columns: new[] { "Id", "Category", "CreatedAt", "Message", "ProgramID", "Status", "UpdatedAt", "UserID" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), "System", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "System Maintenance scheduled for Sunday at 2:00 AM.", null, 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), "Compliance", new DateTime(2026, 3, 10, 1, 0, 0, 0, DateTimeKind.Utc), "New compliance report is ready for Fitness Challenge 2026.", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 1, new DateTime(2026, 3, 10, 1, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("e1000000-0000-0000-0000-000000000003"), "Event", new DateTime(2026, 3, 10, 2, 0, 0, 0, DateTimeKind.Utc), "Your participation in 'Morning Yoga' has been confirmed.", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 4, new DateTime(2026, 3, 10, 2, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("e1000000-0000-0000-0000-000000000004"), "Assignment", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "You have been assigned as the lead coach for Mental Health Awareness.", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 1, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "Notification");
        }
    }
}
