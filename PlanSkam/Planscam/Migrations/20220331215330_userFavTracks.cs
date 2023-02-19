using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planscam.Migrations
{
    public partial class userFavTracks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavouriteTracksId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavouriteTracksId",
                table: "AspNetUsers",
                column: "FavouriteTracksId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FavouriteTracks_FavouriteTracksId",
                table: "AspNetUsers",
                column: "FavouriteTracksId",
                principalTable: "FavouriteTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FavouriteTracks_FavouriteTracksId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavouriteTracksId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavouriteTracksId",
                table: "AspNetUsers");
        }
    }
}
