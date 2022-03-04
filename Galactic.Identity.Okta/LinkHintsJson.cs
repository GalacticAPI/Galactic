using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// An optional object returned by HAL Link data indicating the HTTP verbs that
    /// are allowed by the link.
    /// A record representing HAL Link data for JSON formatting.
    /// A Link Object represents a hyperlink from the containing resource to a URI.
    /// Defined in: https://datatracker.ietf.org/doc/html/draft-kelly-json-hal-06
    /// </summary>
    public record LinkHintsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Allow JSON property name.
        /// </summary>
        public const string ALLOW = "allow";

        // ----- PROPERTIES -----

        /// <summary>
        /// (Optional) Indicates the types of HTTP verbs allowed by the associated link.
        /// </summary>
        [JsonPropertyName(ALLOW)]
        public string[] Allow { get; init; } = default!;
    }
}
