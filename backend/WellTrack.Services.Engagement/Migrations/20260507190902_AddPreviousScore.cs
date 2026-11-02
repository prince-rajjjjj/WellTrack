using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellTrack.Services.Engagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviousScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PreviousScore",
                schema: "Engagement",
                table: "Engagements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousScore",
                schema: "Engagement",
                table: "Engagements");
        }
    }
}
