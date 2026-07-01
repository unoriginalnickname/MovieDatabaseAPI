using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieData.Migrations
{
    /// <inheritdoc />
    public partial class test123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "Movies",
                newName: "ChangedAtDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAtDateTime",
                table: "Movies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedAtDateTime",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "ChangedAtDateTime",
                table: "Movies",
                newName: "AddedAt");
        }
    }
}
