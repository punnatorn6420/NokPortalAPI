namespace NokPortalAPI.Models
{
    public class ServiceSettings
    {
        /// <summary>
        /// OAuth2 settings.
        /// </summary>
        public OAuth2Setting OAuth2 { get; set; } = new OAuth2Setting();

        public class OAuth2Setting
        {
            public string ClientId { get; set; } = string.Empty;
            public string ResponseType { get; set; } = "code";
            public string TenantId { get; set; } = string.Empty;
            public string RedirectUriSignUp { get; set; } = string.Empty;
            public string RedirectUriSignIn { get; set; } = string.Empty;
            public string Scope { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
        }

    }
}