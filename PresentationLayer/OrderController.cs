using BusinesLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;
using System.Security.Claims;
using static BusinesLogicLayer.ValidationResult;

namespace SolarVolt.PresentationLayer
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderService _orderService;
        int GetUserID() //JWT → ASP.NET يفكه → User Claims → GetUserId() → int userId
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }





        /* Data for test 
          
         
         
         //OrderItems  Not orderItems ///////////

    {
      "OrderItems": [
        {
          "ProductID": 3,
          "Quantity": 1
        },
        {
          "ProductID": 2,
          "Quantity": 1
        }
                    ]
    }

         
          */

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]CreateOrderDTo OrderDto)
        {
            var res=await _orderService.CreateOrder(OrderDto,/*GetUserID()*/1);
            if (!res.IsValid)
            {

                switch (res.enProductOrQuanitiy)
                {
                    case ProductOrQuanitiy.ProductNotExists:
                        {
                            return NotFound(new { message="this product not found",IDs= res.InvalidProductIDs });
                        }
                    case ProductOrQuanitiy.QuantityLessThenOrder:
                        {

                            return BadRequest(new { message = " our Quanitiy of this product  Less Then Order ", IDs = res.InvalidProductIDs });

                        }
                }
            }
            return StatusCode(201,new {message="Order Created Succissfuly" });

        }
    }
}
