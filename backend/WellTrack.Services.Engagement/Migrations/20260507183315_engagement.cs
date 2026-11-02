using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class engagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "Engagement",
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "Engagement",
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000002"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Engagement",
                table: "Engagements",
                columns: new[] { "Id", "ActivityScore", "AttendanceScore", "ConsistencyScore", "CreatedAt", "EmployeeID", "EngagementLevel", "FeedbackScore", "GoalScore", "LastCalculatedAt", "ProgramID", "TotalScore", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), 20.00m, 25.00m, 7.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "High", 10.00m, 20.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("99999999-9999-9999-9999-999999999999"), 82.00m, null },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), 15.00m, 18.00m, 0.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Medium", 10.00m, 12.50m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("88888888-8888-8888-8888-888888888888"), 55.50m, null }
                });

            migrationBuilder.InsertData(
                schema: "Engagement",
                table: "Feedbacks",
                columns: new[] { "Id", "Comments", "CreatedAt", "CreatedBy", "Date", "EmployeeID", "ProgramID", "Rating", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("f1000000-0000-0000-0000-000000000001"), "Really enjoyed the yoga session. Very calming!", new DateTime(2026, 3, 6, 9, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 6, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("99999999-9999-9999-9999-999999999999"), 5, null, null },
                    { new Guid("f1000000-0000-0000-0000-000000000002"), "Good content, but the pace was a bit fast.", new DateTime(2026, 3, 11, 12, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 11, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888888"), 4, null, null }
                });
        }
    }
}
