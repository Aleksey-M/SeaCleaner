using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeaCleaner.Server.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gamers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gamers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameDate = table.Column<DateTimeOffset>(nullable: false),
                    GameDuration = table.Column<long>(nullable: false),
                    SavedDolphins = table.Column<int>(nullable: false),
                    IsVictory = table.Column<bool>(nullable: false),
                    GamerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameResults_Gamers_GamerId",
                        column: x => x.GamerId,
                        principalTable: "Gamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_GamerId",
                table: "GameResults",
                column: "GamerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameResults");

            migrationBuilder.DropTable(
                name: "Gamers");
        }
    }
}
