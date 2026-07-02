using BusinesLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Product_UnitsController : ControllerBase
    {
        private readonly Product_UnitsService _product_UnitsService;
        public Product_UnitsController(Product_UnitsService product_UnitsService)
        {
            _product_UnitsService = product_UnitsService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct_Units(AddProduct_UnitDto ProUnitDTo)
        {
            var ProductExists =await _product_UnitsService.AddProduct_Unit(ProUnitDTo);

            return ProductExists ? Ok(new { Message = "Product_Unit Added Succissfuly" }) 
                : BadRequest(new { Message = "No Product Like that" });
        }

        [HttpDelete("{Product_UnitID}")]
        public async Task<IActionResult> DeleteProduct_Units(int Product_UnitID)
        {
            var IsDeleted = await _product_UnitsService.DeleteProduct_Unit(Product_UnitID);
            if (!IsDeleted)
            {
                return BadRequest(new {Message= "Product_Unit not deleted" });
            }
       
                return Ok(new { Message = "deleted Succissfuly" });
            
        }


        [HttpPut("{Product_UnitID}")]
        public async Task<IActionResult> ChangeProduct_UnitStatus(int Product_UnitID,[FromBody] UpdatedProduct_UnitStatusDTo status)
        {
            bool IsUpdated =await _product_UnitsService.ChangeProduct_UnitStatus(Product_UnitID, status);
            if (!IsUpdated)
            {
                return NotFound(new {Message="Product or Product_unit not Found" });
            }
            return Ok(new { Message = "Status Change Succissfuly" });
        }

    }
}
