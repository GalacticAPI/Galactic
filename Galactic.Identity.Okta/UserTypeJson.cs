using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Type objects for JSON formatting.
    /// </summary>
    public record UserTypeJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        // ----- PROPERTIES -----

        /// <summary>
        /// The Id of the User's User Type.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;
    }
}
