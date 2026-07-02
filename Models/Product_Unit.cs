namespace SolarVolt.Models
{

    public enum UnitStatus
    {
        Available,
        Sold,
        Damage,
        Deleted
    }

    public class Product_Unit
    {
        public int Product_UnitId { get; set; } 
        public int ProductID { get; set; }
        public UnitStatus Status { get; set; } 


        public Product product { get; set; } = null!;   
    }
}
