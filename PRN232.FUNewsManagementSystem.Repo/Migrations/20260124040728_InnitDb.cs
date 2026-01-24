using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN232.FUNewsManagementSystem.Repo.Migrations
{
    /// <inheritdoc />
    public partial class InnitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentCategoryID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryID",
                        column: x => x.ParentCategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemAccounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountRole = table.Column<int>(type: "int", nullable: false),
                    AccountPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagID);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticles",
                columns: table => new
                {
                    NewsArticleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewsContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    NewsStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByID = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.NewsArticleID);
                    table.ForeignKey(
                        name: "FK_NewsArticles_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsArticles_SystemAccounts_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "SystemAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsArticles_SystemAccounts_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "SystemAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NewsTags",
                columns: table => new
                {
                    NewsArticleID = table.Column<int>(type: "int", nullable: false),
                    TagID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTags", x => new { x.NewsArticleID, x.TagID });
                    table.ForeignKey(
                        name: "FK_NewsTags_NewsArticles_NewsArticleID",
                        column: x => x.NewsArticleID,
                        principalTable: "NewsArticles",
                        principalColumn: "NewsArticleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "TagID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryID",
                table: "Categories",
                column: "ParentCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_CategoryID",
                table: "NewsArticles",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_CreatedByID",
                table: "NewsArticles",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_UpdatedByID",
                table: "NewsArticles",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTags_TagID",
                table: "NewsTags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccounts_AccountEmail",
                table: "SystemAccounts",
                column: "AccountEmail",
                unique: true,
                filter: "[AccountEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                column: "TagName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsTags");

            migrationBuilder.DropTable(
                name: "NewsArticles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "SystemAccounts");
        }
    }
}
