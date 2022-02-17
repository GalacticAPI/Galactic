using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Signing Credential objects for JSON formatting.
    /// </summary>
    public record ApplicationSigningCredentialJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// KID JSON property name.
        /// </summary>
        public const string KID = "kid";

        // ----- PROPERTIES -----

        /// <summary>
        /// Reference for key credential for the app.
        /// </summary>
        [JsonPropertyName(KID)]
        public string KId { get; init; } = default!;
    }
}
