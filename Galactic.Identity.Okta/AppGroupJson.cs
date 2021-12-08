using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta App Group objects for JSON formatting.
    /// </summary>
    public record AppGroupJson : GroupJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Source JSON property name.
        /// </summary>
        public const string SOURCE = "source";

        // ----- PROPERTIES -----

        /// <summary>
        /// The ID of the source application of the group.
        /// </summary>
        [JsonPropertyName(SOURCE)]
        public string[] Source { get; init; } = default!;
    }
}
