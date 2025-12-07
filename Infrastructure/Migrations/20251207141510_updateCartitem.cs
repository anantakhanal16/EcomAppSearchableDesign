using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCartitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ProductID",
                table: "CartItem",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Products_ProductID",
                table: "CartItem",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Products_ProductID",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_ProductID",
                table: "CartItem");
        }
    }
}
