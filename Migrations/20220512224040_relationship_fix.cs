using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    public partial class relationship_fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Views",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RecommendedBy",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PinnedArticles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommentLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ArticleLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Views",
                table: "Views",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecommendedBy",
                table: "RecommendedBy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PinnedArticles",
                table: "PinnedArticles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleLikes",
                table: "ArticleLikes",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Views",
                table: "Views");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecommendedBy",
                table: "RecommendedBy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PinnedArticles",
                table: "PinnedArticles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleLikes",
                table: "ArticleLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RecommendedBy");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PinnedArticles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommentLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ArticleLikes");
        }
    }
}
