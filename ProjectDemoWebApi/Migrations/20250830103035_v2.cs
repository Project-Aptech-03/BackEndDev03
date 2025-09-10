using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDemoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FAQ",
                table: "FAQ");

            migrationBuilder.DropIndex(
                name: "idx_faq_sort",
                table: "FAQ");

            migrationBuilder.DropColumn(
                name: "created_date",
                table: "FAQ");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "FAQ");

            migrationBuilder.RenameTable(
                name: "FAQ",
                newName: "Faqs");

            migrationBuilder.RenameColumn(
                name: "question",
                table: "Faqs",
                newName: "Question");

            migrationBuilder.RenameColumn(
                name: "answer",
                table: "Faqs",
                newName: "Answer");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Faqs",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                table: "Faqs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "Faqs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Faqs",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Faqs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Faqs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Faqs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faqs",
                table: "Faqs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Faqs",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Faqs");

            migrationBuilder.RenameTable(
                name: "Faqs",
                newName: "FAQ");

            migrationBuilder.RenameColumn(
                name: "Question",
                table: "FAQ",
                newName: "question");

            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "FAQ",
                newName: "answer");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "FAQ",
                newName: "is_active");

            migrationBuilder.AlterColumn<string>(
                name: "question",
                table: "FAQ",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "FAQ",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "FAQ",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_date",
                table: "FAQ",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "FAQ",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FAQ",
                table: "FAQ",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "idx_faq_sort",
                table: "FAQ",
                column: "sort_order");
        }
    }
}
