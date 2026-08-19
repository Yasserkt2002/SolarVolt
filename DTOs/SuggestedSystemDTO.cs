namespace SolarVolt.DTOs
{
    public class SuggestedSystemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CapacityWatt { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
