using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BusinesLogicLayer;
using System.Text;
using SolarVolt.BusinesLogicLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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


builder.Services.AddHttpClient<SmsService>();

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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

/////////////////////////////////////////////////////////////////////////////////////////////////////
// Õﬁ‰ «·‹ DbContext Êﬁ—«¡… ‰’ «·« ’«· „‰ „·› appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
///////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  ›⁄Ì· «·‹ Authentication («· Õﬁﬁ „‰ «·ÂÊÌ…) ﬁ»· «·‹ Authorization («·’·«ÕÌ« )
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();