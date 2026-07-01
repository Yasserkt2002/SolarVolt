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
        [HttpPost]
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


        [HttpPut("{ProductID}")] 
        public async Task<IActionResult> UpdateProduct([FromBody] ProductAddDto NewProduct, int ProductID)
        {
            var res = await _productService.UpdateProduct(NewProduct, ProductID);
            if (res == null)
            {
                return BadRequest(new { id = ProductID, m = " Product  not Updated " });
            }
            return Ok(new { res = $"product with ID={ProductID} Updated Succissfuly" });
        }


        [HttpGet]  //https://t.me/c/3394009212/2/121    //  [HttpGet("{CategoryID}")] ليش مو       //https://t.me/c/3394009212/2/122
        public async Task<IActionResult> GetAllProduct(int? CategoryID = null)    //(int? CategoryID = null)  <=== query param
        {
            var res=await  _productService.GetAllProducts(CategoryID);
            if (res.Any())
            {
                return Ok(new { Message="this is a list of products" ,Data=res});
            } 
            return BadRequest(new { m = "  ): no products returned" });
        }

        [HttpGet("{ProductID}")]
        public async Task<IActionResult> GetProductByID(int ProductID)
        {
            var res=await _productService.GetProductByID(ProductID);  
            if(res!=null)
                return Ok(new { data=res });
            return BadRequest(new { m = "error" });
        }


    }
}
