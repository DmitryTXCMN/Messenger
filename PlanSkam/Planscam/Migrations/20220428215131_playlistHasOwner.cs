using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planscam.Migrations
{
    public partial class playlistHasOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnedById",
                table: "Playlists",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnedPlaylistsId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OwnedPlaylists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedPlaylists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_OwnedById",
                table: "Playlists",
                column: "OwnedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OwnedPlaylistsId",
                table: "AspNetUsers",
                column: "OwnedPlaylistsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_OwnedPlaylists_OwnedPlaylistsId",
                table: "AspNetUsers",
                column: "OwnedPlaylistsId",
                principalTable: "OwnedPlaylists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_OwnedPlaylists_OwnedById",
                table: "Playlists",
                column: "OwnedById",
                principalTable: "OwnedPlaylists",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_OwnedPlaylists_OwnedPlaylistsId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_OwnedPlaylists_OwnedById",
                table: "Playlists");

            migrationBuilder.DropTable(
                name: "OwnedPlaylists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_OwnedById",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OwnedPlaylistsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OwnedById",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "OwnedPlaylistsId",
                table: "AspNetUsers");
        }
    }
}
