using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Models;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : InHouseControllerBase
    {
        private readonly IUserService<User> userService;

        public UserController(
            IUserService<User> userService,
            IResponseFactory resFactory) : base(resFactory)
        {
            this.userService = userService;
        }

        [HttpGet("search")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> SearchUsersAsync([FromQuery] IUserSearchCriteria searchCriteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                IEnumerable<User> user = await userService.GetUsersByCriteriaAsync(searchCriteria);
                return OkResponseWithResult(user);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}