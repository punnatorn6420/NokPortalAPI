using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Entities;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    /// <summary>
    /// Controller for role management operations.
    /// </summary>
    [ApiController]
    [Route("v1/roles")]
    public class RoleController : InHouseControllerBase
    {
        private readonly IRoleService roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        public RoleController(IRoleService roleService, IResponseFactory resFactory)
            : base(resFactory)
        {
            this.roleService = roleService;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
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