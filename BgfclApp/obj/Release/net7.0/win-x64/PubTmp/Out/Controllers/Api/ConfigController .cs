using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetERPIntegrationStatus()
        {
            bool isIntegrated = _configuration.GetValue<bool>("ERPIntegration:IsERPIntegrated");
            return Ok(new { isERPIntegrated = isIntegrated });
        }

    }

}
