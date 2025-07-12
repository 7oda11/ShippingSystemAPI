using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class orderCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderCancellations_OrderId",
                table: "OrderCancellations");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCancellations_OrderId",
                table: "OrderCancellations",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderCancellations_OrderId",
                table: "OrderCancellations");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCancellations_OrderId",
                table: "OrderCancellations",
                column: "OrderId");
        }
    }
}
