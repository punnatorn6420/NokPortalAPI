using NokAir.Core.Interfaces.Common;

namespace NokPortalAPI.Models
{
    public class AppSearchCriteria : ISearchCriteria
    {
        public string Keyword { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string SortField { get; set; } = string.Empty;
        public bool Ascending { get; set; } = true;
    }
}
