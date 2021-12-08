using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing a User's Profile and Credentials JSON data. Used commonly in User related update requests.
    /// </summary>
    public record UserProfileAndCredentialsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Credentials JSON property name.
        /// </summary>
        public const string CREDENTIALS = "credentials";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        // ----- PROPERTIES -----

        /// <summary>
        /// The User's primary authentication and recovery credentials.
        /// </summary>
        [JsonPropertyName(CREDENTIALS)]
        public UserCredentialsJson Credentials { get; init; } = default!;

        /// <summary>
        /// The User's profile properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public UserProfileJson Profile { get; init; } = default!;
    }
}
