using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteFeature_App.Migrations
{
    /// <inheritdoc />
    public partial class addNoteFileNameInNoteFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteFileName",
                table: "NoteFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteFileName",
                table: "NoteFiles");
        }
    }
}
