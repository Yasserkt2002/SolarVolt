using BusinesLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarVolt.DTOs;

namespace SolarVolt.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplianceController : ControllerBase
    {
        private readonly ApplianceService _applianceService;
        public ApplianceController(ApplianceService applianceService)
        {
            _applianceService = applianceService;
        }
        [HttpPost]
        public async Task<IActionResult> AddNewAppliance([FromBody]AddNewApplianceDTo addNewApplianceDTo)
        {
            bool IsApplianceAddedSuccissfuly=await _applianceService.AddNewAppliance(addNewApplianceDTo);    
            if (IsApplianceAddedSuccissfuly)
            {
                return Ok(new { message="Appliance add Succissfuly"});
            }
            return BadRequest(new { message = "Appliance Not added (watage most be >0 or ObjectDto is null)" });
        }
    }
}
