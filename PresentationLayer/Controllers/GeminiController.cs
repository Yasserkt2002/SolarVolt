using Microsoft.AspNetCore.Mvc;
using SolarVolt.BusinesLogicLayer;

    namespace SolarVolt.PresentationLayer.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public GeminiController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("extract-device")]
        public async Task<IActionResult> ExtractDeviceInfo(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("الرجاء رفع صورة صالحة.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            try
            {
                var resultJson = await _geminiService.ExtractDeviceInfoFromImageAsync(imageBytes);
                return Ok(resultJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء معالجة الصورة", Error = ex.Message });
            }
        }
    }
}


/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
    }
}
*/