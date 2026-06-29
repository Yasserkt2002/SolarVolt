using BusinesLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductAddDto NewProduct)
        {
            var res = await _productService.AddProduct(NewProduct);
            if (res == "الفئة غير موجودة")
                return BadRequest(new { m= "الفئة غير موجودة" });
            return Ok(new {m="نم اضافة المنتج بنجاح" }); 


        }


        [HttpDelete("{ProductID}")] //https://t.me/c/3394009212/2/83     لان النية حذف
        public async Task<IActionResult> DeleteProduct(int ProductID)
        {
            var res=await _productService.DeleteProduct(ProductID);
            if (res == null)
            {
                return BadRequest(new { id = ProductID, m = " Product not existe " });
            }
            return Ok(new { res = $"product with ID={ProductID} deleted Succissfuly"});    
        }
    }
}
