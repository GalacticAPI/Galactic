using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Password Hook data for JSON formatting.
    /// </summary>
    public record UserPasswordHookJson
    {
        /// <summary>
        /// The type of Password Inline Hook. Currently, must be set to default.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
    }
}
