using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class addCustomerphone12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerPhone",
                table: "Orders",
                newName: "CustomerPhone2");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone1",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPhone1",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CustomerPhone2",
                table: "Orders",
                newName: "CustomerPhone");
        }
    }
}
