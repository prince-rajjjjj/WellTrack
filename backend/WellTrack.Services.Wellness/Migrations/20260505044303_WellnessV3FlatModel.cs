using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Wellness.Migrations
{
    /// <inheritdoc />
    public partial class WellnessV3FlatModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Wellness");

            migrationBuilder.CreateTable(
                name: "ActivityAttendances",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    JoinTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityAttendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityResources",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HRID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_Programs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "Wellness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedCoachID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedCoachName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalSchema: "Wellness",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Wellness",
                table: "Programs",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "EndDate", "HRID", "StartDate", "Status", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A 12-week fitness challenge to improve overall health and wellness", new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Fitness Challenge 2026", null, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Monthly sessions focused on mental health and stress management", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Mental Health Awareness", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Wellness",
                table: "Activities",
                columns: new[] { "Id", "AssignedCoachID", "AssignedCoachName", "CreatedAt", "CreatedBy", "Date", "Description", "Duration", "Location", "MaxParticipants", "Mode", "ProgramID", "Status", "Title", "Type", "UpdatedAt", "UpdatedBy", "VideoUrl" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 5, 7, 0, 0, 0, DateTimeKind.Utc), "Energizing yoga at the office gym.", 60, "SDB1, 4th floor, ODC 5", 25, "Offline", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Active", "Morning Yoga", "Yoga", null, null, null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 10, 6, 30, 0, 0, DateTimeKind.Utc), "Watch this video on your own time to improve your running form.", 25, null, 100, "Online", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Active", "Running Form 101", "Running", null, null, "https://www.youtube.com/watch?v=brFHyOtTwH4" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 15, 14, 0, 0, 0, DateTimeKind.Utc), "Hands-on mindfulness practice in person.", 90, "SDB2, 2nd floor, Conference Room B", 30, "Offline", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "Mindfulness Workshop", "Workshop", null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProgramID",
                schema: "Wellness",
                table: "Activities",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttendances_ActivityID",
                schema: "Wellness",
                table: "ActivityAttendances",
                column: "ActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityResources_ActivityID",
                schema: "Wellness",
                table: "ActivityResources",
                column: "ActivityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities",
                schema: "Wellness");

            migrationBuilder.DropTable(
                name: "ActivityAttendances",
                schema: "Wellness");

            migrationBuilder.DropTable(
                name: "ActivityResources",
                schema: "Wellness");

            migrationBuilder.DropTable(
                name: "Programs",
                schema: "Wellness");
        }
    }
}
