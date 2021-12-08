using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Password data for JSON formatting.
    /// </summary>
    public record UserPasswordJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Value JSON property name.
        /// </summary>
        public const string VALUE = "value";

        /// <summary>
        /// Hash JSON property name.
        /// </summary>
        public const string HASH = "hash";

        /// <summary>
        /// Hook JSON property name.
        /// </summary>
        public const string HOOK = "hook";

        // ----- PROPERTIES -----

        /// <summary>
        /// The value of the User's password.
        /// </summary>
        [JsonPropertyName(VALUE)]
        public string Value { get; init; } = default!;

        /// <summary>
        /// Specifies the User's hashed password.
        /// </summary>
        [JsonPropertyName(HASH)]
        public UserHashedPasswordJson Hash { get; init; } = default!;

        /// <summary>
        /// Specifies that a Password Import Inline Hook should be triggered to
        /// handle verification of the User's password the first time the User
        /// logs in.
        /// </summary>
        [JsonPropertyName(HOOK)]
        public UserPasswordHookJson Hook { get; init; } = default!;
    }
}
