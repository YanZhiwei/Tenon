using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blog.Repository.Migrations
{
    /// <inheritdoc />
    public partial class init_dbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "friendLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Markdown = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyBy = table.Column<long>(type: "INTEGER", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_posts_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    PostId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tags_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_Id",
                table: "categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_friendLinks_Id",
                table: "friendLinks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_CategoryId",
                table: "posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_Id",
                table: "posts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tags_Id",
                table: "tags",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tags_PostId",
                table: "tags",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friendLinks");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
