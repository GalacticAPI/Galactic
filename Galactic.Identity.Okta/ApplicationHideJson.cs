using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Hide objects for JSON formatting.
    /// </summary>
    public record ApplicationHideJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// iOS JSON property name.
        /// </summary>
        public const string IOS = "iOS";

        /// <summary>
        /// Web JSON property name.
        /// </summary>
        public const string WEB = "web";

        // ----- PROPERTIES -----

        /// <summary>
        /// Okta Mobile for iOS or Android (pre-dates Android).
        /// </summary>
        [JsonPropertyName(IOS)]
        public bool IOs { get; init; } = default!;

        /// <summary>
        /// Okta Web Browser Home Page.
        /// </summary>
        [JsonPropertyName(WEB)]
        public bool Web { get; init; } = default!;
    }
}
