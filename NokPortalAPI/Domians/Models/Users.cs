using System.ComponentModel.DataAnnotations;

namespace NokPortal.Domians.Models
{
    public class Users
    {
        public int UserID { get; set; }

        [StringLength(255)]
        required public string Email { get; set; }

        [StringLength(255)]
        required public string FirstName { get; set; }

        [StringLength(255)]
        required public string LastName { get; set; }

        [StringLength(100)]
        required public string JobTitle { get; set; }

        [StringLength(255)]
        required public string Department { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
