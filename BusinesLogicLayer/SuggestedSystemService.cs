using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;

namespace BusinesLogicLayer
{
    public class SuggestedSystemService
    {
        private readonly ApplicationDbContext _context;

        public SuggestedSystemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SuggestedSystemDTO>> GetActiveSuggestedSystemsAsync()
        {
            return await _context.SuggestedSystems
                .Where(s => s.IsActive)
                .Select(s => new SuggestedSystemDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    CapacityWatt = s.CapacityWatt,
                    Price = s.Price,
                    ImageUrl = s.ImageUrl
                })
                .ToListAsync();
        }
    }
}
