using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Visibility objects for JSON formatting.
    /// </summary>
    public record ApplicationVisibilityJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// App Links JSON property name.
        /// </summary>
        public const string APP_LINKS = "appLinks";

        /// <summary>
        /// Auto Launch JSON property name.
        /// </summary>
        public const string AUTO_LAUNCH = "autoLaunch";

        /// <summary>
        /// Auto Submit Toolbar JSON property name.
        /// </summary>
        public const string AUTO_SUBMIT_TOOLBAR = "autoSubmitToolbar";

        /// <summary>
        /// Hide JSON property name.
        /// </summary>
        public const string HIDE = "hide";

        // ----- PROPERTIES -----

        /// <summary>
        /// Displays specific appLinks for the app.
        /// Each application defines one or more appLinks that can be published. You can disable AppLinks by setting
        /// the link value to false.
        /// </summary>
        [JsonPropertyName(APP_LINKS)]
        public object AppLinks { get; init; } = default!;

        /// <summary>
        /// Automatically signs into the app when user signs into Okta.
        /// </summary>
        [JsonPropertyName(AUTO_LAUNCH)]
        public bool AutoLaunch { get; init; } = default!;

        /// <summary>
        /// Automatically sign in when user lands on the sign-in page.
        /// </summary>
        [JsonPropertyName(AUTO_SUBMIT_TOOLBAR)]
        public bool AutoSubmitToolbar { get; init; } = default!;

        /// <summary>
        /// Hides the app for specific end-user apps.
        /// </summary>
        [JsonPropertyName(HIDE)]
        public ApplicationHideJson Hide { get; init; } = default!;
    }
}
