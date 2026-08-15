namespace SolarVolt.DTOs
{
    public class GetSessionInfoDTO
    {
        public int EnergyInputSessionID { get; set; }
        public int UserID { get; set; }
        public string SourceType { get; set; }=string.Empty;
        public int TotalWatt { get; set; }

        public  DateTime TimeCreated  { get; set; }

        public List<SessionItemDTO> Item { get; set; } = new List<SessionItemDTO>();
    }
}
