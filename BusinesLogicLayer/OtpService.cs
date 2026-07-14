using DataAccessLayer;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;

        public OtpService(ApplicationDbContext context)
        {
            _context=context;
        }

        public string GenerateOtp()
        {
            return Random.Shared.Next(100000,999999).ToString();  
        }

        public async Task SaveOTp(string phone , string code,string FullName,string HashPassword)
        {
            OTPCode code1 = new OTPCode()
            {
                Phone = phone,
                Code = code,
                ExpiresAt = DateTime.Now.AddMinutes(5),
                IsUsed = false,
              FullName= FullName,   
              HashPassword=HashPassword 
                

            };
            await _context.OTPCodes.AddAsync(code1);
            await _context.SaveChangesAsync();  
        }

    }
}
