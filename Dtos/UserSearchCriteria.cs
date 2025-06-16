using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Dtos
{
    public class UserSearchCriteria : IUserSearchCriteria
    {
        public string Keyword { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string SortField { get; set; } = string.Empty;
        public bool Ascending { get; set; } = true;
    }
}
