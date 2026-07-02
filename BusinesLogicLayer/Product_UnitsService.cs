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

        public async Task<bool> ChangeProduct_UnitStatus(int Product_UnitID, UpdatedProduct_UnitStatusDTo NewStatus)
        {
            var product_unit = await _context.Product_Units.FirstOrDefaultAsync(pu=>pu.Product_UnitId== Product_UnitID); ///////////////////  &&pu.Status!=UnitStatus.Deleted    /////////////////////////////////
            if (product_unit ==null) return false;
            

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == product_unit.ProductID && !p.IsDeleted);
            if (product == null) return false;
            
                if (NewStatus.StatusDto == UnitStatus.Available && product_unit.Status != UnitStatus.Available)
                {
                    product.StockQuantity++;
                }
                else if (NewStatus.StatusDto != UnitStatus.Available && product_unit.Status == UnitStatus.Available)
                {
                    product.StockQuantity--;
                }
         
            product_unit.Status = NewStatus.StatusDto;
            await _context.SaveChangesAsync();  
            return true;

        }

        public async Task<GetCategory_UnitByIDDTo> GetProduct_UnitByID( int product_UnitID)             /////   //alot of logic
        {
            var product_unit = await _context.Product_Units.FirstOrDefaultAsync(pu => pu.Product_UnitId == product_UnitID && pu.Status!=UnitStatus.Deleted);
            if (product_unit == null) return null;
            var product = await _context.Products.FirstOrDefaultAsync(p=>p.ProductId== product_unit.ProductID&&!p.IsDeleted);
            if (product == null) return null;

            return new GetCategory_UnitByIDDTo()
            {
                Product_UnitID = product_UnitID,
                ProductID = product_unit.ProductID,
                ProductName = product.Name,
                unitStatus = product_unit.Status.ToString(),
            };
        }

    }
}
