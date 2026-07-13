using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class ApplianceService
    {
        private readonly ApplicationDbContext _context;
        public ApplianceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewAppliance(AddNewApplianceDTo addNewApplianceDTo)
        {
            if (addNewApplianceDTo != null )
            {
                

                await _context.Appliances.AddAsync(
                    new Appliance()
                    {
                        Name= addNewApplianceDTo.Name,  
                        DefaultWattage=addNewApplianceDTo.DefaultWattage,
                        ImagePath=addNewApplianceDTo.ImagePath,

                    });
                await _context.SaveChangesAsync();
                return true; 
            }
            
                return false;
        }

    }
}
