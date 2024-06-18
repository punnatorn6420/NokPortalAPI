using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("app")]

    public class AppController : NokController<ControllerBase>
    {
        private readonly IAppService _appService;

        public AppController(IAppService appService)
        {
            _appService = appService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateApp(RequestCreateApp reqApp)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await _appService.CreateAppAsync(reqApp);
                return this.Ok(this.FormatSuccessResponse(null));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<App> app = await _appService.GetAllAppAsync();
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}
