using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Credential data for JSON formatting.
    /// </summary>
    public record UserCredentialsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Password JSON property name.
        /// </summary>
        public const string PASSWORD = "password";

        /// <summary>
        /// RecoveryQuestion JSON property name.
        /// </summary>
        public const string RECOVERY_QUESTION = "recovery_question";

        /// <summary>
        /// Provider JSON property name.
        /// </summary>
        public const string PROVIDER = "provider";

        // ----- PROPERTIES -----

        /// <summary>
        /// Specifies a password for the User.
        /// </summary>
        [JsonPropertyName(PASSWORD)]
        public UserPasswordJson Password { get; init; } = default!;

        /// <summary>
        /// Specifies a secret question and answer that is validated (case insensitive)
        /// when a user forgets their password or unlocks their account.
        /// </summary>
        [JsonPropertyName(RECOVERY_QUESTION)]
        public UserRecoveryQuestionJson RecoveryQuestion { get; init; } = default!;

        /// <summary>
        /// Specifies the authentication provider that validates the User's password
        /// credential. The User's current provider is managed by the Delegated
        /// Authentication settings for the organization.
        /// </summary>
        [JsonPropertyName(PROVIDER)]
        public UserProviderJson Provider { get; init; } = default!;
    }
}
