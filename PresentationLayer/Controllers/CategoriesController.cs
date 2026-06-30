using BusinesLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace SolarVolt.PresentationLayer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _CategoryService;
        public CategoriesController(CategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryAddDTo CategoryDto)
        {

            var res = await _CategoryService.AddCategory(CategoryDto);

            if (res == "Category Exist")
                return BadRequest(new { Message = "this Category already Exist  " });

                return Ok(new {Message= "Category added Succissfuly" });
            
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var res= await _CategoryService.GetAllCategories();
        
            return Ok(new {data= res });
        }

        [HttpDelete("{CategoryID}")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var res=await _CategoryService.DeleteCategory(CategoryID);  
            
            if (res == "Not Found")
            {
                return NotFound(new { Message ="Category Not Fonund"});
            }
            if (res == "Category can't be deleted because it has a products")
            {
                return BadRequest(new { Message = "Category can't be deleted because it has a products" });
            }
            return Ok(new {data=res});
        }

    }
}
