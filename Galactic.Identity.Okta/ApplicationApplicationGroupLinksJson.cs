using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Application Group Links object for JSON formatting.
    /// </summary>
    public record ApplicationApplicationGroupLinksJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// App JSON property name.
        /// </summary>
        public const string APP = "app";

        /// <summary>
        /// Group JSON property name.
        /// </summary>
        public const string GROUP = "group";

        /// <summary>
        /// Self JSON property name.
        /// </summary>
        public const string SELF = "self";

        // ----- PROPERTIES -----

        /// <summary>
        /// A link to the application within Okta.
        /// </summary>
        [JsonPropertyName(APP)]
        public LinkJson App { get; init; } = default!;

        /// <summary>
        /// A link to this group directly within Okta.
        /// </summary>
        [JsonPropertyName(GROUP)]
        public LinkJson Group { get; init; } = default!;

        /// <summary>
        /// A link to this group via the application within Okta.
        /// </summary>
        [JsonPropertyName(SELF)]
        public LinkJson Self { get; init; } = default!;
    }
}
