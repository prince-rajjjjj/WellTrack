using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Wellness.Migrations
{
    /// <inheritdoc />
    public partial class UseStructuredLocationFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "Location",
                value: "Chennai > SDB1 > 4th floor > ODC 5");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "Location",
                value: "Chennai > SDB2 > 2nd floor > Conference Room B");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888a-8888-8888-8888-888888888888"),
                column: "Location",
                value: "Chennai > SDB1 > 6th floor > Auditorium");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888c-8888-8888-8888-888888888888"),
                column: "Location",
                value: "Chennai > SDB2 > 3rd floor > Conference Room A");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "Location",
                value: "SDB1, 4th floor, ODC 5");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "Location",
                value: "SDB2, 2nd floor, Conference Room B");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888a-8888-8888-8888-888888888888"),
                column: "Location",
                value: "SDB1, 6th floor, Auditorium");

            migrationBuilder.UpdateData(
                schema: "Wellness",
                table: "Activities",
                keyColumn: "Id",
                keyValue: new Guid("8888888c-8888-8888-8888-888888888888"),
                column: "Location",
                value: "SDB2, 3rd floor, Wellness Room");
        }
    }
}
