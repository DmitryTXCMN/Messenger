using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planscam.Migrations
{
    public partial class favTracks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteTracks_Playlists_Id",
                        column: x => x.Id,
                        principalTable: "Playlists",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteTracks");
        }
    }
}
