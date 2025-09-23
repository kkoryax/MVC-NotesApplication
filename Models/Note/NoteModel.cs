using NoteFeature_App.Models.User;
using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.Note
{
    public class NoteModel
    {
        [Key]
        public Guid NoteId { get; set; }

        [Required(ErrorMessage = "โปรดระบุชื่อเรื่อง")]
        [StringLength(100, ErrorMessage = "ชื่อเรื่องต้องไม่เกิน 100 ตัวอักษร")]
        [Display(Name = "ชื่อเรื่อง")]
        public string? NoteTitle { get; set; }

        [Required(ErrorMessage = "โปรดใส่เนื้อหา")]
        [StringLength(2000, ErrorMessage = "เนื้อหาต้องมีความยาวไม่เกิน 200 ตัวอักษร")]
        [Display(Name = "เนื้อหา")]
        public string? NoteContent { get; set; }
        public bool? IsPinned { get; set; } = false;

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool FlagActive { get; set; } = true;


        ///Add relation to UserModel
        [Required]
        public Guid UserId { get; set; }
        public UserModel? User { get; set; }

    }

}
