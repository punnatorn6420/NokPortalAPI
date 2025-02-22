using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortalAPI.Shareds;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("health-check")]
    public class HealthCheckController : NokController<ControllerBase>
    {
        private readonly ReloadFileConfig reloadFileConfig;
        private readonly CorsPolicyReloader corsPolicyReloader;

        public HealthCheckController(ReloadFileConfig reloadFileConfig, CorsPolicyReloader corsPolicyReloader)
        {
            this.reloadFileConfig = reloadFileConfig;
            this.corsPolicyReloader = corsPolicyReloader;
        }

        [AllowAnonymous]
        [HttpGet("reload-config")]
        public ActionResult<ApiResponse<object, string>> ReloadConfig()
        {
            reloadFileConfig.ReloadConfiguration();
            corsPolicyReloader.ReloadCorsPolicy();
            return Ok(FormatSuccessResponse("Success"));
        }

        /*
        [AllowAnonymous]
        [HttpGet("current-config")]
        public async Task<ActionResult<ApiResponse<object, string>>> CurrentConfig()
        {
            var config = reloadFileConfig.GetConfigurationJson();
            return this.Ok(this.FormatSuccessResponse(config));
        }
        */
    }
}