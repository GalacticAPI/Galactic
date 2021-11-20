using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Profile data for JSON formatting.
    /// </summary>
    public record GroupProfileJson
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; init; } = default!;
    }
}
