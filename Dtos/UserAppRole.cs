
namespace NokPortalAPI.Dtos
{
    public class UserAppRoleDto
    {
        public int UserId { get; set; }
        public int AppId { get; set; }
        public IList<int> AssignedRoleIds { get; set; } = new List<int>();
    }
}