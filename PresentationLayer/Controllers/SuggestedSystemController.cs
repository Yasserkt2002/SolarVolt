using BusinesLogicLayer;
using Microsoft.AspNetCore.Mvc;

namespace SolarVolt.PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuggestedSystemsController : ControllerBase
    {
        private readonly SuggestedSystemService _suggestedSystemService;

        public SuggestedSystemsController(SuggestedSystemService suggestedSystemService)
        {
            _suggestedSystemService = suggestedSystemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestedSystems()
        {
            var systems = await _suggestedSystemService.GetActiveSuggestedSystemsAsync();

            if (systems == null || !systems.Any())
                return NotFound(new { message = "لا يوجد منظومات مقترحة حالياً." });

            return Ok(new { message = "تم جلب المنظومات المقترحة بنجاح", data = systems });
        }
    }
}