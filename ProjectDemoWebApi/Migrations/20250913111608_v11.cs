using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDemoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dimensions",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "dimension_height",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "dimension_length",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "dimension_width",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dimension_height",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "dimension_length",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "dimension_width",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "dimensions",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
