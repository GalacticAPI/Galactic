using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application OAuth Credential objects for JSON formatting.
    /// </summary>
    public record ApplicationOAuthCredentialJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Auto Key Rotation JSON property name.
        /// </summary>
        public const string AUTO_KEY_ROTATION = "autoKeyRotation";

        /// <summary>
        /// Client ID JSON property name.
        /// </summary>
        public const string CLIENT_ID = "client_id";

        /// <summary>
        /// Client Secret JSON property name.
        /// </summary>
        public const string CLIENT_SECRET = "client_secret";

        /// <summary>
        /// Token Endpoint Auth Method JSON property name.
        /// </summary>
        public const string TOKEN_ENDPOINT_AUTH_METHOD = "token_endpoint_auth_method";

        // ----- PROPERTIES -----

        /// <summary>
        /// Requested key rotation mode.
        /// </summary>
        [JsonPropertyName(AUTO_KEY_ROTATION)]
        public bool AutoKeyRotation { get; init; } = default!;

        /// <summary>
        /// Unique identifier for the OAuth 2.0 client application.
        /// </summary>
        [JsonPropertyName(CLIENT_ID)]
        public string ClientId { get; init; } = default!;

        /// <summary>
        /// OAuth 2.0 client secret string.
        /// </summary>
        [JsonPropertyName(CLIENT_SECRET)]
        public string ClientSecret { get; init; } = default!;

        /// <summary>
        /// Request authentication method for the token endpoint.
        /// </summary>
        [JsonPropertyName(TOKEN_ENDPOINT_AUTH_METHOD)]
        public string TokenEndpointAuthMethod { get; init; } = default!;
    }
}
