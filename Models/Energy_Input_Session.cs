namespace SolarVolt.Models
{
    public class Energy_Input_Session
    {
        public int Energy_Input_SessionID { get; set; }
        public int UserID { get; set; }
        public string SourceType { get; set; } = string.Empty;

        public int TotalWatt { get; set; }
        public DateTime CreatedAt { get; set; }



        public User user { get; set; } = null!;
        public List<Energy_Input_Item> energy_Input_Items_List { get; set; }=new List<Energy_Input_Item>();
        public Recommendation? recommendation { get; set; }

    
    }
}
