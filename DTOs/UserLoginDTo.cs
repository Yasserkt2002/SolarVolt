using System.ComponentModel.DataAnnotations;

namespace SolarVolt.DTOs
{
    public class UserLoginDto
    {

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        public string Password { get; set; }
    }
}
