using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos
{
    public class CreateAppRequestDto
    {
        public string Name { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string Subheader { get; set; } = null!;
        public EnvironmentType EnvironmentType { get; set; }
        public string ClientUrl { get; set; } = null!;
        public string BackendUrl { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public int JwtExpiryHours { get; set; }
        public string Remark { get; set; } = null!;
    }
}