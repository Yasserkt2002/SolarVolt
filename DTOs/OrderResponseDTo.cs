using SolarVolt.Models;

namespace SolarVolt.DTOs
{
    public class OrderResponseDTo
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalCost { get; set; }
      //  public string Status { get; set; }

        public List<OrderItemResponseDTo> Order_Items_List { get; set; } = new List<OrderItemResponseDTo>();

    }
}
