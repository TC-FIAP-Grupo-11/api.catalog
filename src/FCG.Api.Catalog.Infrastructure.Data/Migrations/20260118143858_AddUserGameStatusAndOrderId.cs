using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Api.Catalog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGameStatusAndOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "UserGames",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_OrderId",
                table: "UserGames",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames",
                columns: new[] { "UserId", "GameId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGames_OrderId",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserGames");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }
    }
}
