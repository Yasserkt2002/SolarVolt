using System.ComponentModel.DataAnnotations;

namespace SolarVolt.DTOs
{
    public class AddNewApplianceDTo
    {
        public string Name { get; set; }

        [Range(1,10000)]
        public int DefaultWattage { get; set; }

        public string ImagePath { get; set; } = string.Empty;
    }
}
