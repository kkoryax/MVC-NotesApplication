using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteFeature_App.Migrations
{
    /// <inheritdoc />
    public partial class addNoteFileSizeInNoteFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NoteFileSize",
                table: "NoteFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteFileSize",
                table: "NoteFiles");
        }
    }
}
