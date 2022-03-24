using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Settings objects for JSON formatting.
    /// </summary>
    public record ApplicationSettingsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// App JSON property name.
        /// </summary>
        public const string APP = "app";

        /// <summary>
        /// Notifications JSON property name.
        /// </summary>
        public const string NOTIFICATIONS = "notifications";

        /// <summary>
        /// SignOn JSON property name.
        /// </summary>
        public const string SIGN_ON = "signOn";

        // ----- PROPERTIES -----

        /// <summary>
        /// Displays app settings.
        /// </summary>
        [JsonPropertyName(APP)]
        public object App { get; init; } = default!;

        /// <summary>
        /// Displays notification settings.
        /// </summary>
        [JsonPropertyName(NOTIFICATIONS)]
        public object Notifications { get; init; } = default!;

        /// <summary>
        /// Displays sign-on settings.
        /// </summary>
        [JsonPropertyName(SIGN_ON)]
        public ApplicationSignOnSettingsJson SignOn { get; init; } = default!;
    }
}
