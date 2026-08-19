using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinesLogicLayer;
using SolarVolt.BusinesLogicLayer;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost("Calculate")]
        public async Task<IActionResult> GetRecommendation(int SessionID)
        {
            try
            {

     
                var recommendation = await _recommendationService.CalculateRecommendation(SessionID, 1); /// ////// /////////// /////// //// //////// //////// ////// ////// /////// ////// //

                if (recommendation == null)
                {
                    return NotFound(new { message = "Session does not exist or Appliances List is empty." });
                }

                return Ok(recommendation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected server error occurred." });
            }
        }
    }
}