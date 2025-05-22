using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShrinkLink.UrlShortener.Service.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectSuccessful",
                table: "Visitor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RedirectSuccessful",
                table: "Visitor",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
