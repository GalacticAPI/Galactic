using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Application Group objects for JSON formatting.
    /// </summary>
    public record ApplicationApplicationGroupJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Embedded JSON property name.
        /// </summary>
        public const string EMBEDDED = "_embedded";

        /// <summary>
        /// Links JSON property name.
        /// </summary>
        public const string LINKS = "_links";

        /// <summary>
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// LastUpdated JSON property name.
        /// </summary>
        public const string LAST_UPDATED = "lastUpdated";

        /// <summary>
        /// Priority JSON property name.
        /// </summary>
        public const string PRIORITY = "priority";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        // ----- PROPERTIES -----

        /// <summary>
        /// Embedded resources related to the app group. JSON HAL format.
        /// </summary>
        [JsonPropertyName(EMBEDDED)]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the app group.
        /// </summary>
        [JsonPropertyName(LINKS)]
        public string Links { get; init; } = default!;

        /// <summary>
        /// Unique key of the group.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp when app group was last updated.
        /// </summary>
        [JsonPropertyName(LAST_UPDATED)]
        public DateTime? LastUpdated { get; init; }

        /// <summary>
        /// Priority of group assignment.
        /// </summary>
        [JsonPropertyName(PRIORITY)]
        public int Priority { get; init; } = default!;

        /// <summary>
        /// Valid JSON schema for specifying properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public string Profile { get; init; } = default!;
    }
}
