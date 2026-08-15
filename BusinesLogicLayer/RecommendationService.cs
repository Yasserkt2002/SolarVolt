using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;

namespace SolarVolt.BusinesLogicLayer
{

   
    public class RecommendationService
    {
        private readonly ApplicationDbContext _context;

       public RecommendationService(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task<RecommendationDTO> CalculateRecommendation(int SessionID,int UserID)
        {
            var Session = await _context.Energy_Input_Sessions.
                Include(e => e.energy_Input_Items_List).
                FirstOrDefaultAsync(e=>e.Energy_Input_SessionID==SessionID&&e.UserID==UserID);

            if (Session == null||!Session.energy_Input_Items_List.Any()) //انت هيك عندك حتى في جلسة بس مافي عناصر رح ترجعلك نل
                return null;

            double TotalConsumingWh = 0;
            foreach (var item in Session.energy_Input_Items_List)
            {
                TotalConsumingWh += item.Quantity * (item.OperatingHours) * (item.WattOverride??0);
            }


            return new RecommendationDTO {  };  //temp
        }

    }
}
