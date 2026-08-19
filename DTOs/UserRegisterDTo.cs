using System.ComponentModel.DataAnnotations;

namespace SolarVolt.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string FullName { get; set; } = string.Empty;
        //public string? Email { get; set; }

        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        [MinLength(8,ErrorMessage =" غير مسموح اقل من 8 احرف")]
        public string Password { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string Phone { get; set; }

        //public string? Address { get; set; }
    }
}
