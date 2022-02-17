using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Credential objects for JSON formatting.
    /// </summary>
    public record ApplicationCredentialsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// OAuth Client JSON property name.
        /// </summary>
        public const string OAUTH_CLIENT = "oauthClient";

        /// <summary>
        /// Password JSON property name.
        /// </summary>
        public const string PASSWORD = "password";

        /// <summary>
        /// Scheme JSON property name.
        /// </summary>
        public const string SCHEME = "scheme";

        /// <summary>
        /// Signing JSON property name.
        /// </summary>
        public const string SIGNING = "signing";

        /// <summary>
        /// User Name JSON property name.
        /// </summary>
        public const string USER_NAME = "userName";

        /// <summary>
        /// User Name Template JSON property name.
        /// </summary>
        public const string USER_NAME_TEMPLATE = "userNameTemplate";

        // ----- PROPERTIES -----

        /// <summary>
        /// Credential for OAuth 2.0 client.
        /// </summary>
        [JsonPropertyName(OAUTH_CLIENT)]
        public ApplicationOAuthCredentialJson OAuthClient { get; init; } = default!;

        /// <summary>
        /// Shared password for app.
        /// </summary>
        [JsonPropertyName(PASSWORD)]
        public ApplicationPasswordJson Password { get; init; } = default!;

        /// <summary>
        /// Determines how credentials are managed for the signOnMode.
        /// Values returned are one of the constants specified in the ApplicationAuthenticationScheme record.
        /// </summary>
        [JsonPropertyName(SCHEME)]
        public string Scheme { get; init; } = default!;

        /// <summary>
        /// Signing credential for the signOnMode.
        /// </summary>
        [JsonPropertyName(SIGNING)]
        public ApplicationSigningCredentialJson Signing { get; init; } = default!;

        /// <summary>
        /// Shared username for app.
        /// </summary>
        [JsonPropertyName(USER_NAME)]
        public string UserName { get; init; } = default!;

        /// <summary>
        /// Template used to generate a user's username when the application is assigned via a group or directly to a user.
        /// </summary>
        [JsonPropertyName(USER_NAME_TEMPLATE)]
        public ApplicationUsernameTemplateJson UserNameTemplate { get; init; } = default;
    }
}
