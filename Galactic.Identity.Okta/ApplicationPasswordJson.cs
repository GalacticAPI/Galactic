using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Password objects for JSON formatting.
    /// </summary>
    public record ApplicationPasswordJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Value JSON property name.
        /// </summary>
        public const string VALUE = "value";

        // ----- PROPERTIES -----

        /// <summary>
        /// A password for a user.
        /// </summary>
        [JsonPropertyName(VALUE)]
        public string Value { get; init; } = default!;
    }
}
