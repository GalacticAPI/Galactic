using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User authentication Provider data for JSON formatting.
    /// </summary>
    public record UserProviderJson
    {
        /// <summary>
        /// The name of the authentication provider that validates the user's password
        /// credential.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;

        /// <summary>
        /// The type of the authentication provider that validates the user's password
        /// credential.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
    }
}
