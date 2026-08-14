namespace SolarVolt.DTOs
{
    public class CreateSessionDTO
    {

      //  public int UserID { get; set; } ثغرة امنية:  هيك الفرونت بيقدر يخلق جلسات باسماء يوزر تانيين

        public string SourceType   { get; set; }
        public List <CreateSessionItemDTO> Items { get; set; } 
    }
}
