using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Participation.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityTitleToParticipation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityTitle",
                schema: "Participation",
                table: "Participations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Participation",
                table: "Participations",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "ActivityTitle",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Participation",
                table: "Participations",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "ActivityTitle",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Participation",
                table: "Participations",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "ActivityTitle",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityTitle",
                schema: "Participation",
                table: "Participations");
        }
    }
}
