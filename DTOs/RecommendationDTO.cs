namespace SolarVolt.DTOs
{
    //https://t.me/c/3394009212/2/502
    public class RecommendationDTO
    {

        public int SessionID { get; set; }


        public double TotalEnergyWh { get; set; }



        public int RecommendationPanelCount { get; set; }
        public int RecommendationPanelWattage {get ; set;}



        public int NumberOfBatteries { get; set; }
        public int RecommendationBettaryAh { get; set; }
        public string BatteryType { get; set; }



        public double RecommendationInverterKw { get; set; }



        public decimal EstimatedCost { get; set; }


        public double MonthlyProductionKWh { get; set; }

    
    }
}
