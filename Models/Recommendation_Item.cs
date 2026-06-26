namespace SolarVolt.Models
{
    public class Recommendation_Item
    {
        public int Recommendation_ItemID { get; set; }

        public int RecommendationID { get; set; }

        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public Recommendation recommendation { get; set; } = null!;

        public Product product { get; set; } = null!;  


        
    }
}
