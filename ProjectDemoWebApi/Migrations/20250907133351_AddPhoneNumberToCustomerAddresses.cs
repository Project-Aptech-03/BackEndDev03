using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDemoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToCustomerAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "city",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "district",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "CustomerAddresses");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "CustomerAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "CustomerAddresses",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "CustomerAddresses");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "CustomerAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "district",
                table: "CustomerAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "CustomerAddresses",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
