using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Extensions
{
    public static class UserExtensions
    {
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
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
        }

        public static UserSearchCriteria ToEntity(this UserSearchCriteriaDto userSearchCriteriaDto)
        {
            return new UserSearchCriteria
            {
                Keyword = userSearchCriteriaDto.Keyword,
                PageNumber = userSearchCriteriaDto.PageNumber,
                PageSize = userSearchCriteriaDto.PageSize,
                SortField = userSearchCriteriaDto.SortField,
                Ascending = userSearchCriteriaDto.Ascending
            };
        }

        public static UserSearchCriteriaDto ToDto(this UserSearchCriteria userSearchCriteria)
        {
            return new UserSearchCriteriaDto
            {
                Keyword = userSearchCriteria.Keyword,
                PageNumber = userSearchCriteria.PageNumber,
                PageSize = userSearchCriteria.PageSize,
                SortField = userSearchCriteria.SortField,
                Ascending = userSearchCriteria.Ascending
            };
        }

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
                ModifiedAt = user.ModifiedAt
            };
        }
    }
}
