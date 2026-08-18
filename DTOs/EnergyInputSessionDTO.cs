namespace SolarVolt.DTOs
{
    public class EnergyInputSessionDto
    {
        public int EnergyInputSessionId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public int TotalWatt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<EnergyInputItemDto> Items { get; set; } = new();
    }

    public class EnergyInputItemDto
    {
        public int EnergyInputItemId { get; set; }
        public int? ApplianceId { get; set; }
        public string? ApplianceName { get; set; }
        public int Quantity { get; set; }
        public int? WattOverride { get; set; }
        public double OperatingHours { get; set; }
    }
}
