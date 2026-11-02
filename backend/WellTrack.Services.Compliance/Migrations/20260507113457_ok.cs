using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Compliance.Migrations
{
    /// <inheritdoc />
    public partial class ok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgramNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000001"),
                columns: new[] { "ActivityNameSnapshot", "PolicyNameSnapshot", "ProgramNameSnapshot" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000002"),
                columns: new[] { "ActivityNameSnapshot", "PolicyNameSnapshot", "ProgramNameSnapshot" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                schema: "Compliance",
                table: "ComplianceRecords",
                keyColumn: "Id",
                keyValue: new Guid("cc000002-0000-0000-0000-000000000003"),
                columns: new[] { "ActivityNameSnapshot", "PolicyNameSnapshot", "ProgramNameSnapshot" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords");

            migrationBuilder.DropColumn(
                name: "PolicyNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords");

            migrationBuilder.DropColumn(
                name: "ProgramNameSnapshot",
                schema: "Compliance",
                table: "ComplianceRecords");
        }
    }
}
