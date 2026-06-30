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
                        Brand=NewProduct.Brand,
                        StockQuantity=NewProduct.StockQuantity, 
                        Description=NewProduct.Description,
                        Cost=NewProduct.Cost,   
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





        public async Task<string> UpdateProduct(ProductAddDto Product,int ID)
        {
            var res=await _context.Products.FirstOrDefaultAsync(   p => p.ProductId == ID);

            if (res == null)
            {
                return null;
            }

            res.Name = Product.Name;
            res.Brand = Product.Brand;
            res.StockQuantity = Product.StockQuantity;  
            res.Description = Product.Description;
            res.Cost = Product.Cost;
           res.WattCapacity = Product.WattCapacity; 
           res.ImagePath = Product.ImagePath;   
            res.CategoryID = Product.CategoryID;

          /*  Product prod = new Product()   //this is wrong
            {
                Brand = Product.Brand,
                StockQuantity =Product.StockQuantity,
                Description=Product.Description,                                        //  https://t.me/c/3394009212/2/87
                CategoryID = Product.CategoryID,                                        //  ليش كود ال
                Name= Product.Name,                                                        //update 
                Cost  =Product.Cost,                                                    //  مو متل كود ال 
                ImagePath=Product.ImagePath,                                            //add 
                WattCapacity=Product.WattCapacity,                                       // من ناحية انشاء اوبجكت 
            };          */                                                                   //Product
            await _context.SaveChangesAsync();
            return "Updated";
        }



        public async Task<List<Product>> GetAllProducts()
        {
            var res=await _context.Products.Where(p=>!p.IsDeleted).ToListAsync(); //_context.Products.ToListAsync();  هيك رح ترجع الكل سواء محذوف او لا     IsDeleted

            return res;
        }






    }
}
