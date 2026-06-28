namespace SolarVolt.DTOs
{
   
        public class ProductAddDto
        {
            public string Name { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public int StockQuantity { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public int WattCapacity { get; set; }
            public string ImagePath { get; set; } = string.Empty;
            public int CategoryID { get; set; }
        
    }
}
