using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class Product_UnitsService
    {
        private readonly ApplicationDbContext _context; 
        public Product_UnitsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddProduct_Unit(AddProduct_UnitDto ProUnit)
        {
            var Prod =await _context.Products.FirstOrDefaultAsync(c=>c.ProductId==ProUnit.ProductID&&!c.IsDeleted);

           

            if (Prod!=null)
            {
                await _context.Product_Units.AddAsync(new Product_Unit {
                                                        ProductID=ProUnit.ProductID,
                                                        Status=ProUnit.Status,                  
                                                                       }
                                                     );
                Prod.StockQuantity++;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProduct_Unit(int ProUnitID)
        {

            var product_Unit = await _context.Product_Units.FirstOrDefaultAsync(c=>c.Product_UnitId== ProUnitID);
            if (product_Unit == null)
            {
                return false;
            }
            var product = await _context.Products.FirstOrDefaultAsync(p=>p.ProductId== product_Unit.ProductID&&!p.IsDeleted);
            if (product != null)
            {
                product.StockQuantity--;
            }
            product_Unit.Status =UnitStatus.Deleted ;     
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
