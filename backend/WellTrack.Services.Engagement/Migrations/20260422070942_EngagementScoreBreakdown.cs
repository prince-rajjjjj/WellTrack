using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class EngagementScoreBreakdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DropColumn(
                name: "ActivityID",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.RenameColumn(
                name: "Score",
                schema: "Engagement",
                table: "Engagements",
                newName: "TotalScore");

            migrationBuilder.RenameColumn(
                name: "Date",
                schema: "Engagement",
                table: "Engagements",
                newName: "LastCalculatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "ActivityScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AttendanceScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConsistencyScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EngagementLevel",
                schema: "Engagement",
                table: "Engagements",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Low");

            migrationBuilder.AddColumn<decimal>(
                name: "FeedbackScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GoalScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivityScore", "AttendanceScore", "ConsistencyScore", "CreatedAt", "EngagementLevel", "FeedbackScore", "GoalScore", "LastCalculatedAt", "TotalScore" },
                values: new object[] { 20.00m, 25.00m, 7.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), "High", 10.00m, 20.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), 82.00m });

            migrationBuilder.UpdateData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivityScore", "AttendanceScore", "ConsistencyScore", "CreatedAt", "EmployeeID", "EngagementLevel", "FeedbackScore", "GoalScore", "LastCalculatedAt", "ProgramID", "TotalScore" },
                values: new object[] { 15.00m, 18.00m, 0.00m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Medium", 10.00m, 12.50m, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("88888888-8888-8888-8888-888888888888"), 55.50m });

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_EmployeeID_ProgramID",
                schema: "Engagement",
                table: "Engagements",
                columns: new[] { "EmployeeID", "ProgramID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Engagements_EmployeeID_ProgramID",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "ActivityScore",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "AttendanceScore",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "ConsistencyScore",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "EngagementLevel",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "FeedbackScore",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.DropColumn(
                name: "GoalScore",
                schema: "Engagement",
                table: "Engagements");

            migrationBuilder.RenameColumn(
                name: "TotalScore",
                schema: "Engagement",
                table: "Engagements",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "LastCalculatedAt",
                schema: "Engagement",
                table: "Engagements",
                newName: "Date");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityID",
                schema: "Engagement",
                table: "Engagements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivityID", "CreatedAt", "Date", "Score" },
                values: new object[] { new Guid("a1000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc), 85.00m });

            migrationBuilder.UpdateData(
                schema: "Engagement",
                table: "Engagements",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivityID", "CreatedAt", "Date", "EmployeeID", "ProgramID", "Score" },
                values: new object[] { new Guid("a1000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("99999999-9999-9999-9999-999999999999"), 92.50m });

            migrationBuilder.InsertData(
                schema: "Engagement",
                table: "Engagements",
                columns: new[] { "Id", "ActivityID", "CreatedAt", "Date", "EmployeeID", "ProgramID", "Score", "UpdatedAt" },
                values: new object[] { new Guid("e1000000-0000-0000-0000-000000000003"), new Guid("a1000000-0000-0000-0000-000000000003"), new DateTime(2026, 3, 10, 8, 15, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 10, 8, 15, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888888"), 78.00m, null });
        }
    }
}
