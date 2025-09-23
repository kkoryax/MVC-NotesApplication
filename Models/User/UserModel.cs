using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.User
{
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "กรุณาใส่อีเมลที่ถูกต้อง")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
        public string? Role { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public bool? FlagActive { get; set; }
    }
}
