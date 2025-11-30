using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeAir.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStationPIColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "NameTag",
                keyValue: null,
                column: "NameTag",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NameTag",
                table: "Stations",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ushort>(
                name: "RdsPI",
                table: "Stations",
                type: "smallint unsigned",
                nullable: false,
                defaultValue: (ushort)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RdsPI",
                table: "Stations");

            migrationBuilder.AlterColumn<string>(
                name: "NameTag",
                table: "Stations",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
