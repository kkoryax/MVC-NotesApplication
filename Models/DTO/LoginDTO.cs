using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "โปรดระบุอีเมลของคุณ")]
        [EmailAddress(ErrorMessage = "กรุณาใส่อีเมลที่ถูกต้อง")]
        public string Email { get; set; }

        [Required(ErrorMessage = "กรุณาระบุรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
