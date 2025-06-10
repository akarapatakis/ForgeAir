using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeAir.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropVideoColumnsFromForgeAir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "containsSubtitles",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "externalSubtitles",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "externalSubtitlesPath",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "stretchAspectRatio",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "zoomAspectRatio",
                table: "Tracks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "containsSubtitles",
                table: "Tracks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "externalSubtitles",
                table: "Tracks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "externalSubtitlesPath",
                table: "Tracks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "stretchAspectRatio",
                table: "Tracks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "zoomAspectRatio",
                table: "Tracks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
