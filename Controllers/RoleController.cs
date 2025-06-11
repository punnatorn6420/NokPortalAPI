using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Entities;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/roles")]
    public class RoleController : InHouseControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService, IResponseFactory resFactory)
            : base(resFactory)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = "RootOnly")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            try
            {
                var roles = await roleService.GetAllRolesAsync();
                return OkResponseWithResult(roles);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}
