using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Extensions
{
    /// <summary>
    /// Extension methods for User-related conversions.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Converts UserDto to User entity.
        /// </summary>
        public static User ToEntity(this UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                ObjectId = userDto.ObjectId,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                JobTitle = userDto.JobTitle,
                Department = userDto.Department,
                Active = userDto.Active,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Converts UserSearchCriteriaDto to UserSearchCriteria entity.
        /// </summary>
        public static UserSearchCriteria ToEntity(this UserSearchCriteriaDto userSearchCriteriaDto)
        {
            return new UserSearchCriteria
            {
                Keyword = userSearchCriteriaDto.Keyword,
                PageNumber = userSearchCriteriaDto.PageNumber,
                PageSize = userSearchCriteriaDto.PageSize,
                SortField = userSearchCriteriaDto.SortField,
                Ascending = userSearchCriteriaDto.Ascending,
                AppId = userSearchCriteriaDto.AppId
            };
        }

        /// <summary>
        /// Converts UserSearchCriteria entity to UserSearchCriteriaDto.
        /// </summary>
        public static UserSearchCriteriaDto ToDto(this UserSearchCriteria userSearchCriteria)
        {
            return new UserSearchCriteriaDto
            {
                Keyword = userSearchCriteria.Keyword,
                PageNumber = userSearchCriteria.PageNumber,
                PageSize = userSearchCriteria.PageSize,
                SortField = userSearchCriteria.SortField,
                Ascending = userSearchCriteria.Ascending,
                AppId = userSearchCriteria.AppId
            };
        }

        /// <summary>
        /// Converts User entity to UserDto.
        /// </summary>
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                ObjectId = user.ObjectId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                JobTitle = user.JobTitle,
                Department = user.Department,
                Active = user.Active,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
                Role = user.UserRoles.FirstOrDefault()?.RoleId.ToString() ?? string.Empty,
                Apps = user.UserAppRoleAssignments
                    .Where(a => a.App != null)
                    .Select(a => a.App!)
                    .DistinctBy(a => a.Id)
                    .Select(a => new UserAppAccessDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                    })
                    .ToList(),
            };
        }
    }
}