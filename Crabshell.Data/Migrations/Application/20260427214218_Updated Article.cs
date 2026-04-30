using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crabshell.Data.Migrations.Application
{
    /// <inheritdoc />
    public partial class UpdatedArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Excerpt",
                table: "Article",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArticleType",
                table: "Article",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Article",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanonicalUrl",
                table: "Article",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Article",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Article",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Article",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgDescription",
                table: "Article",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgTitle",
                table: "Article",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PullQuote",
                table: "Article",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Robots",
                table: "Article",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterCard",
                table: "Article",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleType",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "CanonicalUrl",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OgDescription",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OgTitle",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "PullQuote",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Robots",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "TwitterCard",
                table: "Article");

            migrationBuilder.AlterColumn<string>(
                name: "Excerpt",
                table: "Article",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
