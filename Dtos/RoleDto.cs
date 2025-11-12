namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data transfer object for role information.
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// ID of the role.
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// Name of the role.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}