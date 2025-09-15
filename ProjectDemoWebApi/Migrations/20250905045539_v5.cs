using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDemoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminReplies_AspNetUsers_AdminId1",
                table: "AdminReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_AspNetUsers_UserId1",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerQueries_AspNetUsers_CustomerId1",
                table: "CustomerQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerQueries_CustomerId1",
                table: "CustomerQueries");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_UserId1",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_AdminReplies_AdminId1",
                table: "AdminReplies");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "CustomerQueries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "AdminId1",
                table: "AdminReplies");

            migrationBuilder.AddColumn<string>(
                name: "cancellation_reason",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancelled_date",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancellation_reason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "cancelled_date",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId1",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId1",
                table: "CustomerQueries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "CustomerAddresses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminId1",
                table: "AdminReplies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId1",
                table: "Orders",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerQueries_CustomerId1",
                table: "CustomerQueries",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_UserId1",
                table: "CustomerAddresses",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AdminReplies_AdminId1",
                table: "AdminReplies",
                column: "AdminId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminReplies_AspNetUsers_AdminId1",
                table: "AdminReplies",
                column: "AdminId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_AspNetUsers_UserId1",
                table: "CustomerAddresses",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerQueries_AspNetUsers_CustomerId1",
                table: "CustomerQueries",
                column: "CustomerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId1",
                table: "Orders",
                column: "CustomerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
