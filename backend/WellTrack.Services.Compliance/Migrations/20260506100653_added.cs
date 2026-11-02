using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Compliance.Migrations
{
    /// <inheritdoc />
    public partial class added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000002"),
                columns: new[] { "Result", "Status" },
                values: new object[] { "Closed", "Closed" });

            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000003"),
                columns: new[] { "Result", "Status" },
                values: new object[] { "InProgress", "InProgress" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000002"),
                columns: new[] { "Result", "Status" },
                values: new object[] { "Resolved", "Resolved" });

            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000003"),
                columns: new[] { "Result", "Status" },
                values: new object[] { "Assigned", "Assigned" });
        }
    }
}
