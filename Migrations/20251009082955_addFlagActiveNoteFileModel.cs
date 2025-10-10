using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteFeature_App.Migrations
{
    /// <inheritdoc />
    public partial class addFlagActiveNoteFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FlagActive",
                table: "NoteFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagActive",
                table: "NoteFiles");
        }
    }
}
