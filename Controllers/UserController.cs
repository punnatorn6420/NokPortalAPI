using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers.Internal;
using NokCore.Api.Responses.Web;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Resources;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/user")]
    public class UserController : BaseController
    {
        private readonly IUserService<User> userService;

        public UserController(
            IUserService<User> userService,
            ApiResponseFactory<ApiResponseLocalize> apiResponseFactory) : base(apiResponseFactory)
        {
            this.userService = userService;
        }

        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
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