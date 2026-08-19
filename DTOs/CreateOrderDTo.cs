namespace SolarVolt.DTOs
{
    public class CreateOrderDTo
    {
       
        public List<CreateOrderItemDTo> OrderItems { get; set; } = new List<CreateOrderItemDTo>();   //{ get; set; } لانهم غير مكتوبين فرش السيرفر ساعتين كاملين // https://t.me/c/3394009212/2/135  الفرق بينField و Property


        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Discount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;


    }
}
