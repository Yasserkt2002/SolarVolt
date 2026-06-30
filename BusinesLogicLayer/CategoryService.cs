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
            var res=await _context.Categories.FirstOrDefaultAsync(c=>c.Name== CategoryDTo.Name&&!c.IsDeleted);
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
            var res = await _context.Categories.Where(c => !c.IsDeleted).Select(c=>new GetAllCategoriesDTo
            { CategoryID = c.CategoryId,Name=c.Name }).ToListAsync();
            
           
         
                return res;
            
           
        }

    }
}
