using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeAir.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFXBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "FX",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "FX",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FX_Banks",
                columns: table => new
                {
                    FxId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FX_Banks", x => new { x.BankId, x.FxId });
                    table.ForeignKey(
                        name: "FK_FX_Banks_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FX_Banks_FX_FxId",
                        column: x => x.FxId,
                        principalTable: "FX",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FX_BankId",
                table: "FX",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_FX_FilePath",
                table: "FX",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IX_FX_Banks_FxId",
                table: "FX_Banks",
                column: "FxId");

            migrationBuilder.AddForeignKey(
                name: "FK_FX_Banks_BankId",
                table: "FX",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FX_Banks_BankId",
                table: "FX");

            migrationBuilder.DropTable(
                name: "FX_Banks");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_FX_BankId",
                table: "FX");

            migrationBuilder.DropIndex(
                name: "IX_FX_FilePath",
                table: "FX");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "FX");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "FX",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
