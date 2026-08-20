using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarVolt.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SolarVolt.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1️⃣ إظهار كل المفضلة لمستخدم معين
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserFavorites(int userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => new
                {
                    f.ProductId,
                    ProductName = f.Product.Name,
                    Price = f.Product.Cost,
                    // ضيف أي خصائص إضافية للمنتج تحتاجها الواجهة (صورة، فئة.. إلخ)
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // 2️⃣ إضافة أو إزالة منتج من المفضلة (Toggle Button)
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite(int userId, int productId)
        {
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (existingFavorite != null)
            {
                // إذا كان موجود -> إزالة
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
                return Ok(new { isFavorite = false, message = "تمت الإزالة من المفضلة" });
            }

            // إذا لم يكن موجود -> إضافة
            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { isFavorite = true, message = "تمت الإضافة للمفضلة" });
        }
    }
}