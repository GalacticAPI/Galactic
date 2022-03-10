using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Profile Request data for JSON formatting.
    /// </summary>
    public record GroupProfileRequestJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        // ----- PROPERTIES -----

        /// <summary>
        /// The profile object embedded in this request.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public GroupProfileJson Profile { get; init; } = default!;
    }
}
