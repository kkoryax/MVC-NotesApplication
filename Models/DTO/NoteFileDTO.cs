using NoteFeature_App.Models.Note;
using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.DTO
{
    public class NoteFileDTO
    {
        public Guid NoteFileId { get; set; }

        public Guid NoteId { get; set; }

        public string? NoteFilePath { get; set; }

        public string? NoteFileType { get; set; }

        public DateTime UploadedDate { get; set; }
    }
}
