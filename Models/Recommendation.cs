namespace SolarVolt.Models
{
    public class Recommendation
    {
        public int RecommendationID { get; set; }

        public int UserID  { get; set; }
        public int SessionID { get; set; }
        public int TotalWattage { get; set; }
        public decimal EstimatedCost { get; set; }
        public DateTime CreatedAt { get; set; }


        public User user { get; set; } = null!;
        public Energy_Input_Session energy_Input_Session { get; set; } = null!;


        public  List<Recommendation_Item> Recommendation_Items_List = new List<Recommendation_Item>();    
    }
}
