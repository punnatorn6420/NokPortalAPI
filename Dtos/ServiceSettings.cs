namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Service settings configuration.
    /// </summary>
    public class ServiceSettings
    {
        /// <summary>
        /// OAuth2 settings.
        /// </summary>
        public OAuth2Setting OAuth2 { get; set; } = new OAuth2Setting();

        /// <summary>
        /// OAuth2 Setting details.
        /// </summary>
        public class OAuth2Setting
        {
            /// <summary>
            /// Client ID for the OAuth2 setting.
            /// </summary>
            public string ClientId { get; set; } = string.Empty;

            /// <summary>
            /// OAuth2 response type.
            /// </summary>
            public string ResponseType { get; set; } = "code";

            /// <summary>
            /// Tenant ID for the OAuth2 setting.
            /// </summary>
            public string TenantId { get; set; } = string.Empty;

            /// <summary>
            /// Redirect URI for sign-up.
            /// </summary>
            public string RedirectUriSignUp { get; set; } = string.Empty;

            /// <summary>
            /// Redirect URI for sign-in.
            /// </summary>
            public string RedirectUriSignIn { get; set; } = string.Empty;

            /// <summary>
            /// Environment type for the app user assignment.
            /// </summary>
            public string Scope { get; set; } = string.Empty;

            /// <summary>
            /// OAuth2 authority state.
            /// </summary>
            public string State { get; set; } = string.Empty;
        }
    }
}