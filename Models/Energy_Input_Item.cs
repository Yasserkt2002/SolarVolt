namespace SolarVolt.Models
{
    public class Energy_Input_Item
    {
        public int Energy_Input_ItemID { get; set; }    
        public int SessionID { get; set; }
        public int ApplianceID { get; set; }
        public int Quantity { get; set; }   
        public int WattOverride { get; set; }



        public Energy_Input_Session energy_Input_Session { get; set; } = null!;

        public Appliance appliance =null!;  

    }
}



