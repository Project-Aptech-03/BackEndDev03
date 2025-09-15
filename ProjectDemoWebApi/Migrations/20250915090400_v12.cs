using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDemoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "publisher_address",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AuthorFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    follower_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    author_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorFollows_AspNetUsers_author_id",
                        column: x => x.author_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthorFollows_AspNetUsers_follower_id",
                        column: x => x.follower_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "ntext", nullable: false),
                    summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    featured_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    view_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    like_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    comment_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    published_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    author_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_AspNetUsers_author_id",
                        column: x => x.author_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blogs_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "ntext", nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    like_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    blog_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    parent_comment_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogComments_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogComments_BlogComments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "BlogComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogComments_Blogs_blog_id",
                        column: x => x.blog_id,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    blog_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogLikes_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogLikes_Blogs_blog_id",
                        column: x => x.blog_id,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    comment_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentLikes_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentLikes_BlogComments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "BlogComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_author_follows_author",
                table: "AuthorFollows",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "idx_author_follows_follower",
                table: "AuthorFollows",
                column: "follower_id");

            migrationBuilder.CreateIndex(
                name: "idx_author_follows_unique",
                table: "AuthorFollows",
                columns: new[] { "follower_id", "author_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_blog_comments_blog",
                table: "BlogComments",
                column: "blog_id");

            migrationBuilder.CreateIndex(
                name: "idx_blog_comments_parent",
                table: "BlogComments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "idx_blog_comments_user",
                table: "BlogComments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_blog_likes_unique",
                table: "BlogLikes",
                columns: new[] { "blog_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogLikes_user_id",
                table: "BlogLikes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_blogs_author",
                table: "Blogs",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "idx_blogs_category",
                table: "Blogs",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "idx_blogs_featured",
                table: "Blogs",
                column: "is_featured");

            migrationBuilder.CreateIndex(
                name: "idx_blogs_published",
                table: "Blogs",
                columns: new[] { "is_published", "published_date" });

            migrationBuilder.CreateIndex(
                name: "idx_blogs_slug",
                table: "Blogs",
                column: "slug",
                unique: true,
                filter: "[slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_comment_likes_unique",
                table: "CommentLikes",
                columns: new[] { "comment_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_user_id",
                table: "CommentLikes",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorFollows");

            migrationBuilder.DropTable(
                name: "BlogLikes");

            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "BlogComments");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "publisher_address",
                table: "Publishers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
