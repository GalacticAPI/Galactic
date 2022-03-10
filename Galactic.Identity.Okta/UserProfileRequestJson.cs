using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing a User's Profile Request JSON data. Used commonly in User related create and update requests.
    /// </summary>
    public record UserProfileRequestJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        // ----- PROPERTIES -----

        /// <summary>
        /// The User's profile properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public UserProfileJson Profile { get; init; } = default!;
    }
}
