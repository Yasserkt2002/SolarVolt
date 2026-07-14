namespace SolarVolt.Models
{
    public class OTPCode
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }    
        public DateTime ExpiresAt { get; set; } 
        public bool IsUsed { get; set; }

        //temp info for user
        public string FullName { get; set; }
        public string HashPassword { get; set; }
    }
}
