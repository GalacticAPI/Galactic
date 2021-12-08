using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Profile data for JSON formatting.
    /// </summary>
    public record GroupProfileJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Description JSON property name.
        /// </summary>
        public const string DESCRIPTION = "description";

        // ----- PROPERTIES -----

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [JsonPropertyName(DESCRIPTION)]
        public string Description { get; init; } = default!;
    }
}
