using BusinesLogicLayer;
using Microsoft.AspNetCore.Authorization;
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
     

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }



        [NonAction]
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : 0;
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
        [Authorize(Roles = "Admin,Client")]

        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTo OrderDto)
        {
            var res = await _orderService.CreateOrder(OrderDto,GetUserId());   ////////////////////////////////////////////////////
            if (!res.IsValid)
            {

                switch (res.enProductOrQuanitiy)
                {
                    case ProductOrQuanitiy.ProductNotExists:
                        {
                            return NotFound(new { message = "this product not found", IDs = res.InvalidProductIDs });
                        }
                    case ProductOrQuanitiy.QuantityLessThenOrder:
                        {

                            return BadRequest(new { message = " our Quanitiy of this product  Less Then Order ", IDs = res.InvalidProductIDs });

                        }
                    default:return StatusCode(500, new { Message = "ERRORR while creating the order" });
                }
            }
            return StatusCode(201, new { message = "Order Created Succissfuly" ,
            orderID=res.OrderID,
            deliverPIN=res.DeliverPin
            });

        }

      
        [HttpGet("{OrderID}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> GetOrderByID(int OrderID)
        {
            var res = await _orderService.GetOrderByID(OrderID);
            if (res == null)
            {
                return NotFound(new { message = $"Order with ID={OrderID} not found" });
            }
            return Ok(new { message = "Order Found", OrderDetails = res });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getAllUserOrders()
        {
            var res =await _orderService.getAllUserOrders();
            if (res == null)
                return NotFound(new { message = "No Orders Found" });
            return Ok(new { Orders=res });

        }


        [HttpPut("Complete/{OrderID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompletedOrder(int OrderID)  // Admin // admin// //
        {
            bool IsCompleted =await _orderService.CompletedOrder(OrderID);
            if (IsCompleted)
            {
                return Ok(new {message="Order status Completed" });
            }
            return BadRequest(new { message = "Order not found or has been canceled" });
        }



        //v1
        //[HttpPut("Cancel/{OrderID}")]
        //public async Task<IActionResult> CanceledOrder(int OrderID)  // Admin // admin// //
        //{
        //    bool IsCanceled = await _orderService.CanceledOrder(OrderID);
        //    if (IsCanceled)
        //    {
        //        return Ok(new { message = "Order status Canceled" });
        //    }
        //    return BadRequest(new { message = "Order not found or already has been Completed" });
        //}



        [HttpPut("Cancel/{orderId}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            // استخراج بيانات المستخدم الحالي من الـ JWT Claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            // استدعاء الخدمة مع التمرير الصريح للبيانات الأمنية
            bool isCanceled = await _orderService.CancelOrderAsync(orderId, GetUserId(), userRole);           ////////////////////////////////////////////////////////////////////////////////////

            if (isCanceled)
            {
                return Ok(new { message = "Order status canceled successfully and stock restored." });
            }

            return BadRequest(new { message = "Order not found, not owned by user, or cannot be canceled (Not Pending)." });
        }

    }
}
