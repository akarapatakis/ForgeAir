using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeAir.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStationLogoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoFilePath",
                table: "Stations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoFilePath",
                table: "Stations");
        }
    }
}
