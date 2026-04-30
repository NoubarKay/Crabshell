using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crabshell.Data.Migrations.Application
{
    /// <inheritdoc />
    public partial class Addednumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Author",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Author");
        }
    }
}
