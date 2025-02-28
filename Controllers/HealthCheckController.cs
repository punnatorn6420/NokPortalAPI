using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers.Internal;
using NokCore.Api.Responses.Web;
using NokPortalAPI.Shareds;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/health-check")]
    public class HealthCheckController : BaseController
    {
        private readonly ReloadFileConfig reloadFileConfig;
        private readonly CorsPolicyReloader corsPolicyReloader;

        public HealthCheckController(
            ReloadFileConfig reloadFileConfig,
            CorsPolicyReloader corsPolicyReloader,
            IApiResponseFactory apiResponseFactory) : base(apiResponseFactory)
        {
            this.reloadFileConfig = reloadFileConfig;
            this.corsPolicyReloader = corsPolicyReloader;
        }

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