using BusinesLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.BusinesLogicLayer;
using SolarVolt.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionController( SessionService sessionService)
        {
          
            _sessionService=sessionService; 
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(CreateSessionDTO SessionDTO)
        {


            // معلق مؤقتا 
            /*
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(UserIdClaim, out int UserID))
            {
                return Unauthorized(new { Message = "User غير مصرح به" });
            }
            */

            var res = await _sessionService.CreateSession(SessionDTO,   1         /*                               UserID                  */ );            ////////////////////////////////////////////////

            if (res == true)
            {
                return Ok(new { Message = "Session Created (: " });
            }
            else
            {
                return BadRequest(new { Message = "Session not created ): " });
            }

        }
    }
}
