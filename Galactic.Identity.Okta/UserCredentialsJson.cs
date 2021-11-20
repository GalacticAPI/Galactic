using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Credential data for JSON formatting.
    /// </summary>
    public record UserCredentialsJson
    {
        /// <summary>
        /// Specifies a password for the User.
        /// </summary>
        [JsonPropertyName("password")]
        public UserPasswordJson Password { get; init; } = default!;

        /// <summary>
        /// Specifies a secret question and answer that is validated (case insensitive)
        /// when a user forgets their password or unlocks their account.
        /// </summary>
        [JsonPropertyName("recovery_question")]
        public UserRecoveryQuestionJson RecoveryQuestion { get; init; } = default!;

        /// <summary>
        /// Specifies the authentication provider that validates the User's password
        /// credential. The User's current provider is managed by the Delegated
        /// Authentication settings for the organization.
        /// </summary>
        [JsonPropertyName("provider")]
        public UserProviderJson Provider { get; init; } = default!;
    }
}
