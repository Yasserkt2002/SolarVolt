namespace SolarVolt.Models
{
    public class SuggestedSystem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CapacityWatt { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
