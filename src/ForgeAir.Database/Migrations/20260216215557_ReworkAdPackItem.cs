using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeAir.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReworkAdPackItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPackItem_AdPacks_AdPackId",
                table: "AdPackItem");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPackItem_Tracks_AdTrackId",
                table: "AdPackItem");

            migrationBuilder.DropIndex(
                name: "IX_AdPackItem_AdTrackId",
                table: "AdPackItem");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AdPackItem");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "AdPackItem",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "AdTrackId",
                table: "AdPackItem",
                newName: "PlayOrder");

            migrationBuilder.AlterColumn<int>(
                name: "AdPackId",
                table: "AdPackItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdPackItem_TrackId",
                table: "AdPackItem",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPackItem_AdPacks_AdPackId",
                table: "AdPackItem",
                column: "AdPackId",
                principalTable: "AdPacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdPackItem_Tracks_TrackId",
                table: "AdPackItem",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdPackItem_AdPacks_AdPackId",
                table: "AdPackItem");

            migrationBuilder.DropForeignKey(
                name: "FK_AdPackItem_Tracks_TrackId",
                table: "AdPackItem");

            migrationBuilder.DropIndex(
                name: "IX_AdPackItem_TrackId",
                table: "AdPackItem");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "AdPackItem",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "PlayOrder",
                table: "AdPackItem",
                newName: "AdTrackId");

            migrationBuilder.AlterColumn<int>(
                name: "AdPackId",
                table: "AdPackItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AdPackItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AdPackItem_AdTrackId",
                table: "AdPackItem",
                column: "AdTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPackItem_AdPacks_AdPackId",
                table: "AdPackItem",
                column: "AdPackId",
                principalTable: "AdPacks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdPackItem_Tracks_AdTrackId",
                table: "AdPackItem",
                column: "AdTrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
