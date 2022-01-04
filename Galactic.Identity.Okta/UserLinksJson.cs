using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Links objects for JSON formatting.
    /// </summary>
    public record UserLinksJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Activate JSON property name.
        /// </summary>
        public const string ACTIVATE = "activate";

        /// <summary>
        /// ChangePassword JSON property name.
        /// </summary>
        public const string CHANGE_PASSWORD = "changePassword";

        /// <summary>
        /// ChangeRecoveryQuestion JSON property name.
        /// </summary>
        public const string CHANGE_RECOVERY_QUESTION = "changeRecoveryQuestion";

        /// <summary>
        /// Deactivate JSON property name.
        /// </summary>
        public const string DEACTIVATE = "deactivate";

        /// <summary>
        /// ExpirePassword JSON property name.
        /// </summary>
        public const string EXPIRE_PASSWORD = "expirePassword";

        /// <summary>
        /// ForgotPassword JSON property name.
        /// </summary>
        public const string FORGOT_PASSWORD = "forgotPassword";

        /// <summary>
        /// ResetFactors JSON property name.
        /// </summary>
        public const string RESET_FACTORS = "resetFactors";

        /// <summary>
        /// ResetPassword JSON property name.
        /// </summary>
        public const string RESET_PASSWORD = "resetPassword";

        /// <summary>
        /// Self JSON property name.
        /// </summary>
        public const string SELF = "self";

        /// <summary>
        /// Suspend JSON property name.
        /// </summary>
        public const string SUSPEND = "suspend";

        /// <summary>
        /// Unlock JSON property name.
        /// </summary>
        public const string UNLOCK = "unlock";

        /// <summary>
        /// Unsuspend JSON property name.
        /// </summary>
        public const string UNSUSPEND = "unsuspend";

        // ----- PROPERTIES -----

        /// <summary>
        /// Lifecycle action to activate the user.
        /// </summary>
        [JsonPropertyName(ACTIVATE)]
        public LinkJson Activate { get; init; } = default!;

        /// <summary>
        /// Changes a user's password by validating the user's current password.
        /// </summary>
        [JsonPropertyName(CHANGE_PASSWORD)]
        public LinkJson ChangePassword { get; init; } = default!;

        /// <summary>
        /// Changes a user's recovery credential by validating the user's current password.
        /// </summary>
        [JsonPropertyName(CHANGE_RECOVERY_QUESTION)]
        public LinkJson ChangeRecoveryQuestion { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to deactivate the user.
        /// </summary>
        [JsonPropertyName(DEACTIVATE)]
        public LinkJson Deactivate { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to expire the user's password.
        /// </summary>
        [JsonPropertyName(EXPIRE_PASSWORD)]
        public LinkJson ExpirePassword { get; init; } = default!;

        /// <summary>
        /// Reset a user's password by validating the user's recovery credential.
        /// </summary>
        [JsonPropertyName(FORGOT_PASSWORD)]
        public LinkJson ForgotPassword { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to reset all MFA factors.
        /// </summary>
        [JsonPropertyName(RESET_FACTORS)]
        public LinkJson ResetFactors { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to trigger a password reset.
        /// </summary>
        [JsonPropertyName(RESET_PASSWORD)]
        public LinkJson ResetPassword { get; init; } = default!;

        /// <summary>
        /// A self-referential link to this user.
        /// </summary>
        [JsonPropertyName(SELF)]
        public LinkJson Self { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to suspend the user.
        /// </summary>
        [JsonPropertyName(SUSPEND)]
        public LinkJson Suspend { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to unlock a locked-out user.
        /// </summary>
        [JsonPropertyName(UNLOCK)]
        public LinkJson Unlock { get; init; } = default!;

        /// <summary>
        /// Lifecycle action to unsuspend the user.
        /// </summary>
        [JsonPropertyName(UNSUSPEND)]
        public LinkJson Unsuspend { get; init; } = default!;
    }
}
