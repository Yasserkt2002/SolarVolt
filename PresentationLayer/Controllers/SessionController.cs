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

        public SessionController(SessionService sessionService)
        {

            _sessionService = sessionService;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
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

            int SessionId = await _sessionService.CreateSession(SessionDTO, 1         /*                               UserID                  */ );            ////////////////////////////////////////////////

            if (SessionId > 0)
            {
                return Ok(new {SessionId=SessionId, Message = "Session Created (: " });
            }
            else
            {
                return BadRequest(new { SessionId= SessionId,Message = "Session not created ): " });
            }

        }


        [HttpGet("{SessionID}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetSessionByID(int SessionID)
        {
            // معلق مؤقتا 
            /*
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(UserIdClaim, out int UserID))
            {
                return Unauthorized(new { Message = "User غير مصرح به" });
            }
            */

            var res =await _sessionService.GetSessionInfo(SessionID, 1 /*                    UserID                      */);    //         // 
            if (res == null)
                return NotFound(new { message="Session not found"});
            return Ok(new {message="session found", Data=res }); 
        }


        //AI
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            return Ok(sessions);
        }
    }

  
}
