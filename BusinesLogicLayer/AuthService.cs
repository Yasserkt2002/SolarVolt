using BCrypt.Net;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SolarVolt.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class AuthService
    {

        private readonly ApplicationDbContext _context;  //للتعامل مع قاعدة البيانات
        private readonly AuthService _authService;       // BCrypt و JWT للتعامل مع 



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
        public AuthService(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        
        public async Task<string> RegisterAsync( UserRegisterDto model)
        {
            // التأكد إذا الإيميل موجود مسبقاً
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            { 
                return "Email_Exists";
            }

            // تشفير الباسورد قبل الحفظ
           // var hashedPassword = _authService.HashPassword(model.Password);



            // إنشاء كائن المستخدم الجديد
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email.ToLower(),
                Phone = model.Phone,
                Address=model.Address,
                PasswordHash = HashPassword(model.Password),
                Role = "Client", // القيمة الافتراضية لأي مستخدم بيسجل من الفرونت
                IsDeleted = false
               
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return "تم إنشاء الحساب بنجاح!";
        }



        public async Task<string> LoginAsync( UserLoginDto model)
        {
            // البحث عن المستخدم بالإيميل
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            // إذا المستخدم مو موجود أو الباسورد غلط
            if (user == null) 
            {
                return null;//"فشل تسحيل الدحول!" ;
            }

            if (!VerifyPassword(model.Password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user.UserID.ToString(), user.Email, user.Role);
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
        public string GenerateJwtToken(string userId, string email, string role)
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
                new Claim(ClaimTypes.NameIdentifier, userId),   //{  "userId": "5", 
                new Claim(ClaimTypes.Email, email),             //   "email": "test@test.com",
                new Claim(ClaimTypes.Role, role)                //     "role": "Admin"          }
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
        public class UserRegisterDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }

            public string Address { get; set; }
        }

        public class UserLoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

    }
}