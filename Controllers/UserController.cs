using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Dtos;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    /// <summary>
    /// Controller for user management operations.
    /// </summary>
    [ApiController]
    [Route("v1/users")]
    public class UserController : InHouseControllerBase
    {
        private readonly IUserService<UserDto> userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        public UserController(
            IUserService<UserDto> userService,
            IResponseFactory resFactory)
            : base(resFactory)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Searches for users based on the provided criteria.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> SearchUsersAsync(
            [FromQuery] string? keyword = null,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 25,
            [FromQuery] bool? ascending = true,
            [FromQuery] string? sortField = null)
        {
            var criteria = new UserSearchCriteriaDto
            {
                Keyword = keyword ?? string.Empty,
                SortField = sortField ?? string.Empty,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 25,
                Ascending = ascending ?? true,
            };
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                var (items, total) = await userService.GetUsersByCriteriaAsync(criteria);
                return OkResponseWithResult(new
                {
                    totalRecords = total,
                    items
                });
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dto"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("{userId}/role")]
        [Authorize(Policy = "RootOnly")]
        public async Task<IActionResult> UpdateUserRoleAsync(int userId, [FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                await userService.UpdateUserRoleAsync(userId, dto.RoleId);
                return OkSuccessResponse();
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}