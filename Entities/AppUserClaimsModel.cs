public class AppUserClaimsModel
{
    public string ObjectId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool Active { get; set; } = false;
    public string[] Roles { get; set; } = Array.Empty<string>();
}
