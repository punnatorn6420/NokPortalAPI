using NokAir.Shared.Interfaces.Common;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Application search criteria.
    /// </summary>
    public class AppSearchDto : ISearchCriteriaDto
    {
        /// <summary>
        /// Gets or sets the keyword for searching applications.
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the page number for pagination.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size for pagination.
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Gets or sets the field to sort by.
        /// </summary>
        public string SortField { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the sorting is in ascending order.
        /// </summary>
        public bool Ascending { get; set; } = true;
    }
}