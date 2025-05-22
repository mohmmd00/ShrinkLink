using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShrinkLink.UrlShortener.Service.Migrations
{
    /// <inheritdoc />
    public partial class mig11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Visitor",
                newName: "ProcessedUrlCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedUrlCode",
                table: "Visitor",
                newName: "Code");
        }
    }
}
