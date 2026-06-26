namespace SolarVolt.Models
{
    public class Order
    {
        public int OrderId { get; set; } 
        public int UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalCost { get; set; }

        public string Status { get; set; }

        public User user { get; set; } = null!; 

        public List<Order_Item>Order_Items_List { get; set; }    =new List<Order_Item>();


    }
}


