using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Wellness.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceFinalizedAndSeedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AttendanceFinalized",
                schema: "Wellness",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "AttendanceFinalized",
                value: false);

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "AttendanceFinalized",
                value: false);

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "AssignedCoachID", "AssignedCoachName", "AttendanceFinalized", "Date" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", false, new DateTime(2026, 5, 7, 14, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                schema: "Wellness",
                table: "Activities",
                columns: new[] { "Id", "AssignedCoachID", "AssignedCoachName", "AttendanceFinalized", "CreatedAt", "CreatedBy", "Date", "Description", "Duration", "Location", "MaxParticipants", "Mode", "ProgramID", "Status", "Title", "Type", "UpdatedAt", "UpdatedBy", "VideoUrl" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888889"), new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 8, 11, 0, 0, 0, DateTimeKind.Utc), "10-minute guided meditation session — watch when you need a calm reset.", 10, null, 200, "Online", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "Guided Meditation", "Meditation", null, null, "https://www.youtube.com/watch?v=inpok4MKVLM" },
                    { new Guid("8888888a-8888-8888-8888-888888888888"), null, null, false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 12, 16, 0, 0, 0, DateTimeKind.Utc), "In-person workshop on managing workplace stress.", 60, "SDB1, 6th floor, Auditorium", 40, "Offline", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "Stress Management Talk", "Workshop", null, null, null },
                    { new Guid("8888888b-8888-8888-8888-888888888888"), new Guid("44444444-4444-4444-4444-444444444444"), "Coach Alex", false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Watch this short video to learn box-breathing for instant calm.", 8, null, 200, "Online", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "Breathing Techniques", "Meditation", null, null, "https://www.youtube.com/watch?v=tEmt1Znux58" },
                    { new Guid("8888888c-8888-8888-8888-888888888888"), null, null, false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 20, 15, 0, 0, 0, DateTimeKind.Utc), "Group discussion on building a healthier sleep routine.", 45, "SDB2, 3rd floor, Wellness Room", 25, "Offline", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "Sleep Hygiene Session", "Workshop", null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888889"));

            migrationBuilder.DeleteData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888a-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888b-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888c-8888-8888-8888-888888888888"));

            migrationBuilder.DropColumn(
                name: "AttendanceFinalized",
                schema: "Wellness",
                table: "Activities");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "AssignedCoachID", "AssignedCoachName", "Date" },
                values: new object[] { null, null, new DateTime(2026, 4, 15, 14, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
