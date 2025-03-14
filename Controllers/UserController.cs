using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers.Internal;
using NokCore.Api.Responses.Web;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : BaseController
    {
        private readonly IUserService<User> userService;

        public UserController(
            IUserService<User> userService,
            IApiResponseFactory apiResponseFactory) : base(apiResponseFactory)
        {
            this.userService = userService;
        }

        [HttpGet("search")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> SearchUsersAsync([FromQuery] UserSearchCriteria searchCriteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                IEnumerable<IUser> user = await userService.GetUsersByCriteriaAsync(searchCriteria);
                return OkResponseWithResult(user);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}