using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.User
{
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "โปรดระบุอีเมลของคุณ")]
        [EmailAddress(ErrorMessage = "กรุณาใส่อีเมลที่ถูกต้อง")]
        public string Email { get; set; }

        [Required(ErrorMessage = "กรุณาระบุรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool? FlagActive { get; set; }
    }
}
