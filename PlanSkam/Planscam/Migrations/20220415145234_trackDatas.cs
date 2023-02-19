using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planscam.Migrations
{
    public partial class trackDatas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Tracks");

            migrationBuilder.AddColumn<int>(
                name: "TrackDataId",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TrackDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_TrackDataId",
                table: "Tracks",
                column: "TrackDataId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_TrackDatas_TrackDataId",
                table: "Tracks",
                column: "TrackDataId",
                principalTable: "TrackDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_TrackDatas_TrackDataId",
                table: "Tracks");

            migrationBuilder.DropTable(
                name: "TrackDatas");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_TrackDataId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "TrackDataId",
                table: "Tracks");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "Tracks",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
