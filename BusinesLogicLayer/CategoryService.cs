using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddCategory(CategoryAddDTo CategoryDTo)
        {
            var res = await _context.Categories.FirstOrDefaultAsync(c => c.Name == CategoryDTo.Name && !c.IsDeleted);
            if (res != null)
            {
                return "Category Exist";
            }

            Category category = new Category()
            {
                Name = CategoryDTo.Name,

            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return "Success";
        }


        public async Task<List<GetAllCategoriesDTo>> GetAllCategories()
        {
            var res = await _context.Categories.Where(c => !c.IsDeleted).Select(c => new GetAllCategoriesDTo
            { CategoryID = c.CategoryId, Name = c.Name }).ToListAsync();

            return res;
        }

        public async Task<GetCategoryByIdDTo> GetCategoryByID(int CategoryId)
        {                                                                       /*https://t.me/c/3394009212/2/125 More performance 
                                                                                    بدل من جلب الكل تجلب العامود الذي تحتاجه فقط    */
            var res = await _context.Categories.
                FirstOrDefaultAsync(c => c.CategoryId == CategoryId && !c.IsDeleted);
            if (res == null)
                return null;
            return new GetCategoryByIdDTo()
            {
                CategoryID = res.CategoryId,
                Name = res.Name,
            };
        }
        public async Task<string> DeleteCategory(int CategoryID)     //Re//////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            var res = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == CategoryID&&!c.IsDeleted);
            if (res == null)
                return "Not Found";
            var hasproducts = await _context.Products.AnyAsync(p=> p.CategoryID==CategoryID && !p.IsDeleted);           //نسأل جدول المنتجات: هل في أي منتج مو محذوف تابع لهاد القسم؟
           if (hasproducts)
            {
                    return "Category can't be deleted because it has a products";
            }
            
                res.IsDeleted = true;
                await _context.SaveChangesAsync();
                return "Deleted";
            
        }


        public async Task<string> UpdateCategory(UpdateCategoryDTo categoryDTo,int CategoryID)
        {
            var res = await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryId== CategoryID && !c.IsDeleted);
            if (res == null)
                return " Category Not Found";
            res.Name=categoryDTo.Name;


            await _context.SaveChangesAsync();
            return "Category Updated";

        }
    }
}
