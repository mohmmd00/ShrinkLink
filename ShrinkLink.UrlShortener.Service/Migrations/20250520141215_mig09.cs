using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShrinkLink.UrlShortener.Service.Migrations
{
    /// <inheritdoc />
    public partial class mig09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "ShortenUrl");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ShortenUrl",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShortenUrl");

            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "ShortenUrl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
