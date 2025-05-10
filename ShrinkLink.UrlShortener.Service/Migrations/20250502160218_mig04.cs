using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShrinkLink.UrlShortener.Service.Migrations
{
    /// <inheritdoc />
    public partial class mig04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Links");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalUrl",
                table: "Links",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OriginalUrl",
                table: "Links",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AddColumn<long>(
                name: "ClickCount",
                table: "Links",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
