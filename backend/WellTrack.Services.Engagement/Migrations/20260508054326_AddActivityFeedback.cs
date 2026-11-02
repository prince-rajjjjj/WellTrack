using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActivityID",
                schema: "Engagement",
                table: "Feedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityTitle",
                schema: "Engagement",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityID",
                schema: "Engagement",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ActivityTitle",
                schema: "Engagement",
                table: "Feedbacks");
        }
    }
}
