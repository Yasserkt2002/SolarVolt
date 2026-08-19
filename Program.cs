using BusinesLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarVolt.BusinesLogicLayer;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



//v1
//builder.Services.AddSwaggerGen();



//v2
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "«œŒ· «· Êﬂ‰ Â‰« „»«‘—… »œÊ‰ ﬂ·„… Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});








//////////////////////////////////////////////////////////////////////////////////////
//  ”ÃÌ· «·‹ Service «·Œ«’… »«·‹ Authentication
builder.Services.AddScoped<BusinessLogicLayer.AuthService>();
////https://t.me/c/3394009212/2/78
///

builder.Services.AddScoped<BusinesLogicLayer.ProductService>();
builder.Services.AddScoped<BusinesLogicLayer.CategoryService>();
builder.Services.AddScoped<BusinesLogicLayer.Product_UnitsService>();
builder.Services.AddScoped<BusinesLogicLayer.OrderService>();
builder.Services.AddScoped<BusinesLogicLayer.ApplianceService>();
builder.Services.AddScoped<BusinesLogicLayer.OtpService>();
builder.Services.AddScoped<BusinesLogicLayer.SessionService>();
builder.Services.AddScoped<BusinesLogicLayer.RecommendationService>();


builder.Services.AddHttpClient<SmsService>();



//v1
//// ≈⁄œ«œ Ê ÂÌ∆… Œœ„«  «·‹ JWT Authentication »«·”Ì” „
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});



//v2
// ≈⁄œ«œ Ê ÂÌ∆… Œœ„«  «·‹ JWT Authentication »«·”Ì” „
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        //    ﬂ«‰ ‰«ﬁ’ﬂ
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});






/////////////////////////////////////////////////////////////////////////////////////////////////////
// Õﬁ‰ «·‹ DbContext Êﬁ—«¡… ‰’ «·« ’«· „‰ „·› appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
///////////////////////////////////////////////////////////////////////////////////////////////////



builder.Services.AddHttpClient<GeminiService>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
});



var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

//  ›⁄Ì· «·‹ Authentication («· Õﬁﬁ „‰ «·ÂÊÌ…) ﬁ»· «·‹ Authorization («·’·«ÕÌ« )
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();