using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crabshell.Data.Migrations.Application
{
    /// <inheritdoc />
    public partial class Addedrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Article",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Article");
        }
    }
}
