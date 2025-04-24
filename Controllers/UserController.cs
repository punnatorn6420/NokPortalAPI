using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : InHouseControllerBase
    {
        private readonly IUserService<UserDto> userService;

        public UserController(
            IUserService<UserDto> userService,
            IResponseFactory resFactory) : base(resFactory)
        {
            this.userService = userService;
        }

        [HttpGet("search")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> SearchUsersAsync([FromQuery] UserSearchCriteriaDto searchCriteriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                IEnumerable<UserDto> user = await userService.GetUsersByCriteriaAsync(searchCriteriaDto);
                return OkResponseWithResult(user);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}