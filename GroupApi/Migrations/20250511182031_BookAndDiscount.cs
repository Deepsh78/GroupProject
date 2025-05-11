using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupApi.Migrations
{
    /// <inheritdoc />
    public partial class BookAndDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Order_Item",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookImage",
                table: "Book",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Order_Item");

            migrationBuilder.DropColumn(
                name: "BookImage",
                table: "Book");
        }
    }
}
