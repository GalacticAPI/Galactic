using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Password data for JSON formatting.
    /// </summary>
    public record UserPasswordJson
    {
        /// <summary>
        /// The value of the User's password.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; init; } = default!;

        /// <summary>
        /// Specifies the User's hashed password.
        /// </summary>
        [JsonPropertyName("hash")]
        public UserHashedPasswordJson Hash { get; init; } = default!;

        /// <summary>
        /// Specifies that a Password Import Inline Hook should be triggered to
        /// handle verification of the User's password the first time the User
        /// logs in.
        /// </summary>
        [JsonPropertyName("hook")]
        public UserPasswordHookJson Hook { get; init; } = default!;
    }
}
