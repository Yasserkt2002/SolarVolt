using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static BusinessLogicLayer.AuthService;

namespace SolarVolt.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _authService;
        //👆
        ////https://t.me/c/3394009212/2/78                                                   ////"أنا لا أنشئ الـ services… أنا أطلبها، والنظام يحقنها لي جاهزة داخل الكونستركتر"
        //👇                                                                                 ////
        public UsersController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);

            if (result == "Email_Exists")
                return BadRequest(new { message = "الإيميل مسجل مسبقاً (: !" });

            return Ok(new { message = "تم إنشاء الحساب بنجاح!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            var token = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized(new { message = "الإيميل أو كلمة المرور غير صحيحة!" });

            return Ok(new { token = token, message = "تم تسجيل الدخول بنجاح!" });
        }
    }
}