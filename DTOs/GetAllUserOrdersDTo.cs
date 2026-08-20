namespace SolarVolt.DTOs
{
    public class GetAllUserOrdersDTo
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
        public decimal TotalCost { get; set; }

        public string CustomerName { get; set; }
    }
}
