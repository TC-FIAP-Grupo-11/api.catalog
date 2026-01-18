using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Api.Catalog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePromotionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_GameId",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_GameId",
                table: "Promotions",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_GameId",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_GameId",
                table: "Promotions",
                column: "GameId",
                unique: true);
        }
    }
}
