namespace SolarVolt.Models
{
    public class Appliance
    {
        public int ApplianceID { get; set; }
        public string Name { get; set; }
        public int DefalutWattage { get; set; }

        public string ImagePath { get; set; }=string.Empty;



        public List<Energy_Input_Item> Energy_Input_Items_List=new List<Energy_Input_Item>();   
    }
}
