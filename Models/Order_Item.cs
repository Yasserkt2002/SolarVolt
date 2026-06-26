namespace SolarVolt.Models
{
    public class Order_Item
    {
        public int Id { get; set; } 

        public int OrderID { get; set; }
        public int ProductID { get; set;    }

        public int Quantity   { get; set; }

        public decimal Price { get; set; }

        public Order order { get; set; } = null!;   
        public Product product { get; set; } = null!;
    }
}
