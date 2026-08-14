namespace SolarVolt.DTOs
{
    public class CreateSessionItemDTO
    {

        public int ApplianceID { get; set; }    
        public int? Watt { get; set; }   //int?   //https://t.me/c/3394009212/2/500

        public int Quantity { get; set; }

        public string OpeatingTime { get; set; } = string.Empty;    // ex :  h6   or  m30 
    }
}



