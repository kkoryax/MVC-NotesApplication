using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.Note
{
    public class NoteFile
    {
        [Key]
        public Guid NoteFileId { get; set; }

        [Required]
        public Guid NoteId { get; set; }

        [Required]
        public string? NoteFileName { get; set; }

        [Required]
        public string? NoteFilePath { get; set; }

        [Required]
        public string? NoteFileType { get; set; }

        [Required]
        public long NoteFileSize { get; set; }

        public DateTime UploadedDate { get; set; }

        //Navigation property
        public NoteModel? Note { get; set; }
    }
}
