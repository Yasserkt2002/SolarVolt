using BCrypt.Net;
using BusinesLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SolarVolt.BusinesLogicLayer;
using SolarVolt.DTOs;
using SolarVolt.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{


    public class AuthService
    {

        private readonly ApplicationDbContext _context;  //للتعامل مع قاعدة البيانات

        private readonly SmsService _smsService;
        private readonly OtpService _oTPCode;   

        private readonly IConfiguration _configuration;
        // حقن الـ
        // IConfiguration
        // لقراءة ملف الـ
        // appsettings.json
        //يعني
        // (((هذا متغير يسمح لك تقرأ من   
        // appsettings.json )))





        //هذا اسمه
        //Dependency Injection
        //يعني:
        //  ASP.NET
        //  يعطيك إعدادات النظام جاهزة بدل ما تنشئها بنفسك
        public AuthService(ApplicationDbContext context,IConfiguration configuration, SmsService smsService, OtpService oTPCode)
        {
            _context = context;
            _configuration = configuration;
            _smsService = smsService;
            _oTPCode = oTPCode;

        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        

        public async Task<string> VarifyOTP(VarifayOTP_DTO model)
        {
           var otp=await _context.OTPCodes.FirstOrDefaultAsync(o=>o.Phone== model.Phone&&( o.Code== model.Code|| model.Code == "654321")&& !o.IsUsed); ///////////////// model.Code == "654321"   ONLY FOR TEST
            if (otp == null)
            {
                return "Invalid OTP";
            }
            if (otp.ExpiresAt < DateTime.Now)
            {
                return "OTP Expired";
            }

            // إنشاء كائن المستخدم الجديد
            var newUser = new User
            {
                FullName = otp.FullName,
                // Email = model.Email?.ToLower(),
                Phone = otp.Phone,
                //Address=model.Address,
                PasswordHash = otp.HashPassword,
                Role = "Client", // القيمة الافتراضية لأي مستخدم بيسجل من الفرونت
                IsDeleted = false

            };

            _context.Users.Add(newUser);       //جهّز هذا المستخدم للإضافة فقط”
            otp.IsUsed = true;
            await _context.SaveChangesAsync(); //هنا يصير التنفيذ الحقيقي: “نفّذ كل العمليات اللي جهزتها”
            return "Account Created";
        }
        public async Task<string> RegisterAsync( UserRegisterDto model)
        {
            // مرّ على كل مستخدم في الجدول إذا وجدت واحد إيميله يساوي المدخل رجع ترو
            if (await _context.Users.AnyAsync(/*Note1*/u => u.Phone == model.Phone)) //_context.Users تمثيل مباشر للجدول داخل الكود،
            { 
                return "Phone Exists";
            }



            /*Note1
                   u    من وين اجت
                  !؟ 
                أنت ما قلت:
                foreach (var x in Users)
                لكن
                 LINQ 
                داخليًا يفعل شيء مشابه.
             */
            //LINQ = Language Integrated Query
            // طريقة تكتب فيها “استعلامات على البيانات” داخل
            // C#
            // بدل
            // SQL

            /*ex
             بدل ما تكتب SQL:
                SELECT * FROM Users WHERE Email = 'x'
                تكتب C#:
                _context.Users.Where(u => u.Email == "x")
              */


            // تشفير الباسورد قبل الحفظ
            // var hashedPassword = _authService.HashPassword(model.Password);


            string code = _oTPCode.GenerateOtp(); 
           await _oTPCode.SaveRegisterOTp(model.Phone, code,model.FullName, HashPassword(model.Password));
            await _smsService.SendSms(model.Phone, $"{model.FullName}\n" +
                $"مرحبا بك في  SolarVolt (:   كود التحقق : {code}  صالح ل5 دقائق");



/* //تم النقل الى دالة varifayOTP
            // إنشاء كائن المستخدم الجديد
            var newUser = new User
            {
                FullName = model.FullName,
               // Email = model.Email?.ToLower(),
                Phone = model.Phone,
                //Address=model.Address,
                PasswordHash = HashPassword(model.Password),
                Role = "Client", // القيمة الافتراضية لأي مستخدم بيسجل من الفرونت
                IsDeleted = false
               
            };

            _context.Users.Add(newUser);       //جهّز هذا المستخدم للإضافة فقط”

            await _context.SaveChangesAsync(); //هنا يصير التنفيذ الحقيقي: “نفّذ كل العمليات اللي جهزتها”
*/
            return "تم إنشاء الحساب بنجاح!";
        }



        public async Task<string> LoginAsync( UserLoginDto model)
        {
            // البحث عن المستخدم بالإيميل
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == model.Phone&&!u.IsDeleted); //https://t.me/c/3394009212/2/80 FirstOrDefalutAsyncواخواتها

            // إذا المستخدم مو موجود أو الباسورد غلط
            if (user == null) 
            {
                return null;//"فشل تسحيل الدحول!" ;
            }

            if (!VerifyPassword(model.Password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user.UserID, user.Phone, user.Role);
            // توليد الـ JWT Token وتمريره للفرونت
         //   var token = _authService.GenerateJwtToken(user.UserID.ToString(), user.Email, user.Role);

          //  return  "تم تسجيل الدخول بنجاح!"  ;
        }



        // 2. التحقق من صحة كلمة المرور (عند تسجيل الدخول)
        //يرجع ترو  او  فولس

        //مثلا مقارنة 
        //123456
        //مع
        //$2a$11$xyz
        //المخزنة ك كلمة سر بقاعدة البيانات
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }






        // 3. دالة شبه ثابتة
        //معناها الدالة
         //   إنشاء "بطاقة دخول"
        //(JWT)
        //للمستخدم بعد ما يسجّل دخول بنجاح
        public string GenerateJwtToken(int userId, string Phone, string role)
        {

            // جيب من 
            //appsettings.json
            // "Jwt":{
            //           "Key" : "..."
            //          ,"Issuer" : "..."
            //          ,"Audience" : "... "
            //          }
            var jwtSettings = _configuration.GetSection("Jwt");






            /* لتحويل المفتاح
                ليش؟
            لأن التوقيع الرقمي يحتاج
            bytes 
            مش
            string.
             */
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);




            // تجهيز الـ
            // Claims (بيانات الهوية داخل التوكن)
            var claims = new[]
            {                                                  //ex 
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),  
                new Claim(ClaimTypes.MobilePhone, Phone),             
                new Claim(ClaimTypes.Role, role)               
            }; 
            // اهم شي تغيير
            // climes 
            //باقي الكود ثابت




            //بناء التوكن
              var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(360), // التوكن صالح لمدة ......
                Issuer = jwtSettings["Issuer"],           // مين اصدر التوكن
                Audience = jwtSettings["Audience"],      // مين مسموح يسنخدمه:    ex flutter,React
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  //نوقّع التوكن بمفتاح سري حتى إذا أحد عدله:السيرفر يكشف التلاعب 
              };


            //create the token
            var tokenHandler = new JwtSecurityTokenHandler();       
            var token = tokenHandler.CreateToken(tokenDescriptor);  

            return tokenHandler.WriteToken(token); // convert token to string
        }

        // --- الـ DTOs اللازمة لنقل البيانات بأمان ---










        //3 func to reset password 
        public async Task<bool> SendSms_ForgetPassword(string phone)
        {
           

            string code=_oTPCode.GenerateOtp();
            await _oTPCode.SaveResetPassordOTp(phone, code);
            await _smsService.SendSms(phone ,
                   $"مرحبا بك في  SolarVolt (:   كود التحقق : {code}  صالح ل5 دقائق");
            return true;
        }

       

        public async Task<string> VarifyResetOTp(string phone,string code)
        {
           var res= await _context.OTPCodes.FirstOrDefaultAsync(o=>o.Phone==phone&&o.Code== code);
            if (res == null)
            {
                return "No phone or code found";
            }
            if (res.IsUsed)
            {
                return "Code already used";
            }
            if (res.ExpiresAt < DateTime.Now)
            {
                return "Code Expird";
            }
            res.IsVarified = true;
            await _context.SaveChangesAsync();
            return "Varifed";
        }

        public async Task<bool> SetNewPassoword(string phone, string NewPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone && !u.IsDeleted);
            if (user == null)
            {
                return false;
            }
            var otp = await _context.OTPCodes.FirstOrDefaultAsync(o=>o.IsVarified &&
                                                                    !o.IsUsed &&
                                                                     o.Phone==phone);
            if (otp==null)
            {
                return false;
            }
            user.PasswordHash = HashPassword(NewPassword);
            otp.IsUsed = true;  
            await _context.SaveChangesAsync();
            return true;

        }



    }
}