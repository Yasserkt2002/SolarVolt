namespace SolarVolt.DTOs
{
    public class CreateOrderDTo
    {
        public List<CreateOrderItemDTo> OrderItems { get; set; } = new List<CreateOrderItemDTo>();   //{ get; set; } لانهم غير مكتوبين فرش السيرفر ساعتين كاملين // https://t.me/c/3394009212/2/135  الفرق بينField و Property
    }
}
