using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User AppLink objects for JSON formatting.
    /// </summary>
    public record UserAppLinkJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// App ID JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Label JSON property name.
        /// </summary>
        public const string LABEL = "label";

        /// <summary>
        /// Link URL JSON property name.
        /// </summary>
        public const string LINK_URL = "linkUrl";

        /// <summary>
        /// Logo URL property name.
        /// </summary>
        public const string LOGO_URL = "logoUrl";

        /// <summary>
        /// App Name JSON property name.
        /// </summary>
        public const string APP_NAME = "appName";

        /// <summary>
        /// Auto Instance ID JSON property name.
        /// </summary>
        public const string APP_INSTANCE_ID = "appInstanceId";

        /// <summary>
        /// App Assignment ID JSON property name.
        /// </summary>
        public const string APP_ASSIGNMENT_ID = "appAssignmentId";

        /// <summary>
        /// Credentials Setup JSON property name.
        /// </summary>
        public const string CREDENTIALS_SETUP = "credentialsSetup";

        /// <summary>
        /// Hidden JSON property name.
        /// </summary>
        public const string HIDDEN = "hidden";

        /// <summary>
        /// Sort Order JSON property name.
        /// </summary>
        public const string SORT_ORDER = "sortOrder";

        // ----- PROPERTIES -----

        /// <summary>
        /// Application ID.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Application label.
        /// </summary>
        [JsonPropertyName(LABEL)]
        public string Label { get; init; } = default!;

        /// <summary>
        /// Application sign-in link.
        /// </summary>
        [JsonPropertyName(LINK_URL)]
        public string LinkUrl { get; init; } = default!;

        /// <summary>
        /// Application logo url.
        /// </summary>
        [JsonPropertyName(LOGO_URL)]
        public string LogoUrl { get; init; } = default!;

        /// <summary>
        /// Application name.
        /// </summary>
        [JsonPropertyName(APP_NAME)]
        public string AppName { get; init; } = default!;

        /// <summary>
        /// Application instance ID.
        /// </summary>
        [JsonPropertyName(APP_INSTANCE_ID)]
        public string AppInstanceId { get; init; } = default!;

        /// <summary>
        /// Application assignment ID.
        /// </summary>
        [JsonPropertyName(APP_ASSIGNMENT_ID)]
        public string AppAssignmentId { get; init; } = default!;

        /// <summary>
        /// Credentials setup.
        /// </summary>
        [JsonPropertyName(CREDENTIALS_SETUP)]
        public bool CredentialsSetup { get; init; } = default!;

        /// <summary>
        /// If application is hidden.
        /// </summary>
        [JsonPropertyName(HIDDEN)]
        public bool Hidden { get; init; } = default!;

        /// <summary>
        /// Application sort order.
        /// </summary>
        [JsonPropertyName(SORT_ORDER)]
        public int SortOrder { get; init; } = default!;
    }
}