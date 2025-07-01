using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class addCityIdandGovIdToVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GovernmentId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CityId",
                table: "Vendors",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_GovernmentId",
                table: "Vendors",
                column: "GovernmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Cities_CityId",
                table: "Vendors",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Governments_GovernmentId",
                table: "Vendors",
                column: "GovernmentId",
                principalTable: "Governments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Cities_CityId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Governments_GovernmentId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CityId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_GovernmentId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "GovernmentId",
                table: "Vendors");
        }
    }
}
