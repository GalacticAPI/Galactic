using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application objects for JSON formatting.
    /// </summary>
    public record ApplicationJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Embedded JSON property name.
        /// </summary>
        public const string EMBEDDED = "_embedded";

        /// <summary>
        /// Links JSON property name.
        /// </summary>
        public const string LINKS = "_links";

        /// <summary>
        /// Accessibility JSON property name.
        /// </summary>
        public const string ACCESSIBILITY = "accessibility";

        /// <summary>
        /// Created JSON property name.
        /// </summary>
        public const string CREATED = "created";

        /// <summary>
        /// Credentials JSON property name.
        /// </summary>
        public const string CREDENTIALS = "credentials";

        /// <summary>
        /// Features JSON property name.
        /// </summary>
        public const string FEATURES = "features";

        /// <summary>
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Label JSON property name.
        /// </summary>
        public const string LABEL = "label";

        /// <summary>
        /// LastUpdated JSON property name.
        /// </summary>
        public const string LAST_UPDATED = "lastUpdated";

        /// <summary>
        /// Name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        /// <summary>
        /// Request Object Signing Algorithm property name.
        /// </summary>
        public const string REQUEST_OBJECT_SIGNING_ALG = "request_object_signing_alg";

        /// <summary>
        /// Settings property name.
        /// </summary>
        public const string SETTINGS = "settings";

        /// <summary>
        /// Sign on mode property name.
        /// </summary>
        public const string SIGN_ON_MODE = "signOnMode";

        /// <summary>
        /// Status JSON property name.
        /// </summary>
        public const string STATUS = "status";

        /// <summary>
        /// Visibility JSON property name.
        /// </summary>
        public const string VISIBILITY = "visibility";

        // ----- PROPERTIES -----

        /// <summary>
        /// Embedded resources related to the app. JSON HAL format.
        /// </summary>
        [JsonPropertyName(EMBEDDED)]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the app.
        /// </summary>
        [JsonPropertyName(LINKS)]
        public UserLinksJson Links { get; init; } = default!;

        /// <summary>
        /// Access settings for app.
        /// </summary>
        [JsonPropertyName(ACCESSIBILITY)]
        public ApplicationAccessibilityJson Accessibility { get; init; } = default!;

        /// <summary>
        /// Timestamp when App was created.
        /// </summary>
        [JsonPropertyName(CREATED)]
        public DateTime? Created { get; init; } = default!;

        /// <summary>
        /// Credentials for the specified signOnMode.
        /// </summary>
        [JsonPropertyName(CREDENTIALS)]
        public ApplicationCredentialsJson Credentials { get; init; } = default!;

        /// <summary>
        /// Enabled app features.
        /// The value of this property is one of the constants in the ApplicationFeature record.
        /// </summary>
        [JsonPropertyName(FEATURES)]
        public string Features { get; init; } = default!;

        /// <summary>
        /// Unique key for app.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// User defined display name for app.
        /// </summary>
        [JsonPropertyName(LABEL)]
        public string Label { get; init; } = default!;

        /// <summary>
        /// Timestamp when app was last updated.
        /// </summary>
        [JsonPropertyName(LAST_UPDATED)]
        public DateTime? LastUpdated { get; init; } = default!;

        /// <summary>
        /// Unique key for app definition.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// Valid JSON schema for specifying properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public string Profile { get; init; } = default!;

        /// <summary>
        /// They type of JSON Web Key Set (JWKS) algorithm that must be used for signing request objects.
        /// </summary>
        [JsonPropertyName(REQUEST_OBJECT_SIGNING_ALG)]
        public string RequestObjectSigningAlg { get; init; } = default!;

        /// <summary>
        /// Settings for app.
        /// Note: This can't be managed via the API currently.
        /// </summary>
        [JsonPropertyName(SETTINGS)]
        public object Settings { get; init; } = default!;

        /// <summary>
        /// The current status of the user.
        /// </summary>
        [JsonPropertyName(STATUS)]
        public string Status { get; init; } = default!;

        /// <summary>
        /// Visibility settings for the app.
        /// </summary>
        [JsonPropertyName(VISIBILITY)]
        public ApplicationVisibilityJson Visibility { get; init; } = default!;
    }
}
