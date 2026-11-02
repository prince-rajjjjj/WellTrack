using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WellTrack.Services.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Intialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "Name", "PasswordHash", "Phone", "Role", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@welltrack.com", "Admin User", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1234567890", 1, 1, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "hr@welltrack.com", "HR Manager", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1234567891", 2, 1, null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "employee@welltrack.com", "John Employee", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1234567892", 3, 1, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "coach@welltrack.com", "Coach Alex", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1234567893", 4, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000000"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "james.smith0@welltrack.com", "James Smith", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000000", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000001"), new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "mary.johnson1@welltrack.com", "Mary Johnson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000001", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000002"), new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "john.williams2@welltrack.com", "John Williams", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000002", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000003"), new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "patricia.brown3@welltrack.com", "Patricia Brown", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000003", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000004"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "robert.jones4@welltrack.com", "Robert Jones", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000004", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000005"), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "jennifer.garcia5@welltrack.com", "Jennifer Garcia", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000005", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000006"), new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, "michael.miller6@welltrack.com", "Michael Miller", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000006", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000007"), new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "linda.davis7@welltrack.com", "Linda Davis", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000007", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000008"), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "william.rodriguez8@welltrack.com", "William Rodriguez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000008", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000009"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "elizabeth.martinez9@welltrack.com", "Elizabeth Martinez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000009", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000010"), new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, "david.hernandez10@welltrack.com", "David Hernandez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000010", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000011"), new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, "barbara.lopez11@welltrack.com", "Barbara Lopez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000011", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000012"), new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, "richard.gonzalez12@welltrack.com", "Richard Gonzalez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000012", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000013"), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, "susan.wilson13@welltrack.com", "Susan Wilson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000013", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000014"), new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, "joseph.anderson14@welltrack.com", "Joseph Anderson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000014", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000015"), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "jessica.thomas15@welltrack.com", "Jessica Thomas", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000015", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000016"), new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "thomas.taylor16@welltrack.com", "Thomas Taylor", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000016", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000017"), new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "sarah.moore17@welltrack.com", "Sarah Moore", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000017", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000018"), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "charles.jackson18@welltrack.com", "Charles Jackson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000018", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000019"), new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), null, "karen.martin19@welltrack.com", "Karen Martin", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000019", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000020"), new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, "christopher.lee20@welltrack.com", "Christopher Lee", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000020", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000021"), new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "nancy.perez21@welltrack.com", "Nancy Perez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000021", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000022"), new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, "daniel.thompson22@welltrack.com", "Daniel Thompson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000022", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000023"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "lisa.white23@welltrack.com", "Lisa White", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000023", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000024"), new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, "matthew.harris24@welltrack.com", "Matthew Harris", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000024", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000025"), new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), null, "betty.sanchez25@welltrack.com", "Betty Sanchez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000025", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000026"), new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, "anthony.clark26@welltrack.com", "Anthony Clark", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000026", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000027"), new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "margaret.ramirez27@welltrack.com", "Margaret Ramirez", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000027", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000028"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "mark.lewis28@welltrack.com", "Mark Lewis", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000028", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000029"), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "sandra.robinson29@welltrack.com", "Sandra Robinson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000029", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000030"), new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "donald.walker30@welltrack.com", "Donald Walker", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000030", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000031"), new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "ashley.young31@welltrack.com", "Ashley Young", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000031", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000032"), new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "steven.allen32@welltrack.com", "Steven Allen", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000032", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000033"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "kimberly.king33@welltrack.com", "Kimberly King", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000033", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000034"), new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, "paul.wright34@welltrack.com", "Paul Wright", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000034", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000035"), new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "emily.scott35@welltrack.com", "Emily Scott", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000035", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000036"), new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "andrew.torres36@welltrack.com", "Andrew Torres", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000036", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000037"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "donna.nguyen37@welltrack.com", "Donna Nguyen", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000037", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000038"), new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, "joshua.hill38@welltrack.com", "Joshua Hill", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000038", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000039"), new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "michelle.flores39@welltrack.com", "Michelle Flores", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000039", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000040"), new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, "kenneth.green40@welltrack.com", "Kenneth Green", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000040", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000041"), new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, "carol.adams41@welltrack.com", "Carol Adams", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000041", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000042"), new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevin.nelson42@welltrack.com", "Kevin Nelson", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000042", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000043"), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, "amanda.baker43@welltrack.com", "Amanda Baker", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000043", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000044"), new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, "brian.hall44@welltrack.com", "Brian Hall", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000044", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000045"), new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, "dorothy.rivera45@welltrack.com", "Dorothy Rivera", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000045", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000046"), new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "george.campbell46@welltrack.com", "George Campbell", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000046", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000047"), new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "melissa.mitchell47@welltrack.com", "Melissa Mitchell", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000047", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000048"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "timothy.carter48@welltrack.com", "Timothy Carter", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000048", 3, 1, null, null },
                    { new Guid("55555555-5555-5555-5555-000000000049"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "deborah.roberts49@welltrack.com", "Deborah Roberts", "$2b$11$MtBrbgLpRb67uI9uolZE4.7xB7ufnS2Hec96PNuHqT8W0G4dEvQQ6", "+1555000049", 3, 1, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Identity",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");
        }
    }
}
