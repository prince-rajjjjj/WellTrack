using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Engagement");

            migrationBuilder.CreateTable(
                name: "Engagements",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engagements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Engagement",
                table: "Engagements",
                columns: new[] { "Id", "ActivityID", "CreatedAt", "Date", "EmployeeID", "ProgramID", "Score", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), new Guid("a1000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("99999999-9999-9999-9999-999999999999"), 85.00m, null },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), new Guid("a1000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("99999999-9999-9999-9999-999999999999"), 92.50m, null },
                    { new Guid("e1000000-0000-0000-0000-000000000003"), new Guid("a1000000-0000-0000-0000-000000000003"), new DateTime(2026, 3, 10, 8, 15, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 10, 8, 15, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888888"), 78.00m, null }
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Engagements",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Feedbacks",
                schema: "Engagement");
        }
    }
}
