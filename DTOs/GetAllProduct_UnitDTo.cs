namespace SolarVolt.DTOs
{
    public class GetAllProduct_UnitDTo
    {
        public int Product_UnitID { get; set; }
        public string unitStatus { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
