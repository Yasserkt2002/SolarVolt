namespace SolarVolt.Models
{
  public class Order
{
    public int OrderId { get; set; } 
    public int UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal SubTotal { get; set; }       // المجموع الفرعي للكتالوج
    public decimal DeliveryFee { get; set; }    // رسوم التوصيل ($20 مثلاً)
    public decimal Discount { get; set; }       // الخصم المطبق ($50 مثلاً)
    public decimal TotalCost { get; set; }      // الإجمالي النهائي
    
    public string Status { get; set; }          // حالات مثل: Pending, Processing, OnTheWay, Delivered
    public string DeliveryPin { get; set; }     // كود الاستلام (مثال: "5821")
    public string PaymentMethod { get; set; }   // طريقة الدفع (CashOnDelivery, CreditCard, Wallet)
    
    public string? DeliveryAgentName { get; set; } // اسم المندوب (مثال: "أحمد ياسين")

    public User user { get; set; } = null!; 
    public List<Order_Item> Order_Items_List { get; set; } = new List<Order_Item>();
}
}


