using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;
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
            var result = await _authService.RegisterAsync(model);      /*   متزامن     : ( Synchronous )
    
                                                                       👉   غير متزامن : ( Asynchronous )   */

            if (result == "Phone Exists")
                return BadRequest(new { message = "الهاتف مسجل مسبقاً (: !" });

            return Ok(new { message = "تم ارسال كود التحقق!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login([FromBody] UserLoginDto model)
        {
            var token = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized(new { message = "الهاتف أو كلمة المرور غير صحيحة!" });

            return Ok(new { token = token, message = "تم تسجيل الدخول بنجاح!" });
        }

        [HttpPost("Varify-OTP-Register")]
        public async Task<IActionResult> VarifyOTP(VarifayOTP_DTO model)
        {
            var res =await _authService.VarifyOTP(model);
            if (res == "Invalid OTP")
                return BadRequest(new { message = "Invalid OTP" });
            if (res == "OTP Expired")
                return BadRequest(new { message = "OTP Expired" });
            return Ok(new { Results=res});

        }

        [HttpPost("Send-OTP-Reset")]
        public async Task<IActionResult> SendResetSms(string phone)
        {
           await _authService.SendSms_ForgetPassword(phone);   
            return Ok(new { messgage=$"Code is sinding to{phone}"});    
        }

        [HttpGet("Varify-OTP-Reset")]
        public async Task<IActionResult> VarifyOtpReset(string phone,string code)
        {
           var res=await _authService.VarifyResetOTp(phone,code);
            if (res == null)
            {
                return BadRequest( new { message="No phone or code found" });
            }
            if (res == "Code already used")
            {
                return BadRequest(new { message = "Code already used" });
            }
            if (res == "Code Expird")
            {
                return BadRequest(new { message = "Code Expird" });
            }
            return Ok(new { messgage = "Varifed Succissfuly" });
        }

        [HttpPost("ResetPassowrd")]
        public async Task<IActionResult> ResetPassword(string phone, string NewPassword)
        {
            await _authService.SetNewPassoword(phone, NewPassword);
            return Ok(new {message="Password has changed"});
        }

    }
}