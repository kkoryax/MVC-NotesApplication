using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Models.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "โปรดระบุอีเมลของคุณ")]
        [EmailAddress(ErrorMessage = "กรุณาใส่อีเมลที่ถูกต้อง")]
        public string Email { get; set; }

        [Required(ErrorMessage = "กรุณาระบุรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }

}
