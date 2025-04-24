using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Dtos
{
    public class UserDto
    {
        public int Id { get; set; } = 0;

        [MaxLength(255)]
        public string ObjectId { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string JobTitle { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Department { get; set; } = string.Empty;

        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();

        public bool Active { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ModifiedAt { get; set; } = DateTime.Now;

    }
}
