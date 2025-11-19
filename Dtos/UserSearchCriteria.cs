using NokAir.Core.Abstractions.Entities.Rbac;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// User search criteria DTO.
    /// </summary>
    public class UserSearchCriteria : IUserSearchCriteria
    {
        /// <summary>
        /// Keyword to search for.
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// Page number for pagination.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Field to sort by.
        /// </summary>
        public string SortField { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the sorting is in ascending order.
        /// </summary>
        public bool Ascending { get; set; } = true;
    }
}