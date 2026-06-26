namespace SolarVolt.Models
{
    public class Product_Unit
    {
        public int Product_UnitId { get; set; } 
        public int ProductID { get; set; }
        public string Status { get; set; } = string.Empty; 


        public Product product { get; set; } = null!;   
    }
}
