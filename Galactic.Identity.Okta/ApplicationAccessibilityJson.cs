using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Accessibility objects for JSON formatting.
    /// </summary>
    public record ApplicationAccessibilityJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Error Redirect URL JSON property name.
        /// </summary>
        public const string ERROR_REDIRECT_URL = "errorRedirectUrl";

        /// <summary>
        /// Login Redirect URL JSON property name.
        /// </summary>
        public const string LOGIN_REDIRECT_URL = "loginRedirectUrl";

        /// <summary>
        /// Self Service JSON property name.
        /// </summary>
        public const string SELF_SERVICE = "selfService";

        // ----- PROPERTIES -----

        /// <summary>
        /// Custom error page for this application.
        /// </summary>
        [JsonPropertyName(ERROR_REDIRECT_URL)]
        public string ErrorRedirectUrl { get; init; } = default!;

        /// <summary>
        /// Custom login page for this application.
        /// </summary>
        [JsonPropertyName(LOGIN_REDIRECT_URL)]
        public string LoginRedirectUrl { get; init; } = default!;

        /// <summary>
        /// Enable self-service application assignment.
        /// </summary>
        [JsonPropertyName(SELF_SERVICE)]
        public bool SelfService { get; init; } = default!;
    }
}
