using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteFeature_App.Migrations
{
    /// <inheritdoc />
    public partial class addNewPropInNoteModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveFrom",
                table: "Notes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveUntil",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveFrom",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ActiveUntil",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Notes");
        }
    }
}
