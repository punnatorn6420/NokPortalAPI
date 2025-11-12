using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Shareds;

namespace NokPortalAPI.Controllers
{
    /// <summary>
    /// Controller for health check operations.
    /// </summary>
    [ApiController]
    [Route("v1/health-check")]
    public class HealthCheckController : InHouseControllerBase
    {
        private readonly ReloadFileConfig reloadFileConfig;
        private readonly CorsPolicyReloader corsPolicyReloader;

        /// <summary>
        /// Controller for health check operations.
        /// </summary>
        public HealthCheckController(
            ReloadFileConfig reloadFileConfig,
            CorsPolicyReloader corsPolicyReloader,
            IResponseFactory resFactory)
            : base(resFactory)
        {
            this.reloadFileConfig = reloadFileConfig;
            this.corsPolicyReloader = corsPolicyReloader;
        }

        /// <summary>
        /// Reloads the configuration and CORS policy.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("reload-config")]
        public ActionResult ReloadConfig()
        {
            reloadFileConfig.ReloadConfiguration();
            corsPolicyReloader.ReloadCorsPolicy();
            return OkSuccessResponse();
        }
    }
}