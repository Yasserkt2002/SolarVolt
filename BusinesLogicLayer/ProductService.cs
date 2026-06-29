using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

     
        public async Task<string> AddProduct(ProductAddDto NewProduct) // for admin
        {
            bool CategoryExiste = await _context.Categories
                .AnyAsync(u => u.CategoryId == NewProduct.CategoryID);
            if (!CategoryExiste)
            {
                return "الفئة غير موجودة";
            }
            //   _context.Products.Add(NewProduct);   this is wrong ... because https://t.me/c/3394009212/2/82
            Product product = new Product()
                     {
                        Name=NewProduct.Name,
                        brand=NewProduct.Brand,
                        StockQuantity=NewProduct.StockQuantity, 
                        Description=NewProduct.Description,
                        cost=NewProduct.Cost,   
                        WattCapacity    =NewProduct.WattCapacity,   
                        ImagePath ="No Image Yet",    //////////////////////////////////////////////////////////////////////////////////// 
                        CategoryID=NewProduct.CategoryID,   
                     };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return "تم اضافة المنتج بنجاح";
        }

        public async Task<string> DeleteProduct(int ID)
        {
            
           
               var res=await _context.Products.FirstOrDefaultAsync(u => u.ProductId == ID);
            if (res != null)
            {
                res.IsDeleted= true;
                await _context.SaveChangesAsync();
                return "Deleted";
            }
            return null;

             
            
        }


    }
}
