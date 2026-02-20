using NokAir.Shared.Interfaces.Common;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// DTO for user search criteria.
    /// </summary>
    public class UserSearchCriteriaDto : ISearchCriteriaDto
    {
        /// <summary>
        /// Keyword for searching users.
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

        /// <summary>
        /// Optional application ID used for filtering users by app access.
        /// </summary>
        public int? AppId { get; set; }
    }
}
