using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User objects for JSON formatting.
    /// </summary>
    public record UserJson
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
        /// Activated JSON property name.
        /// </summary>
        public const string ACTIVATED = "activated";

        /// <summary>
        /// Created JSON property name.
        /// </summary>
        public const string CREATED = "created";

        /// <summary>
        /// Credentials JSON property name.
        /// </summary>
        public const string CREDENTIALS = "credentials";

        /// <summary>
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// LastLogin JSON property name.
        /// </summary>
        public const string LAST_LOGIN = "lastLogin";

        /// <summary>
        /// LastUpdated JSON property name.
        /// </summary>
        public const string LAST_UPDATED = "lastUpdated";

        /// <summary>
        /// PasswordChanged JSON property name.
        /// </summary>
        public const string PASSWORD_CHANGED = "passwordChanged";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        /// <summary>
        /// Status JSON property name.
        /// </summary>
        public const string STATUS = "status";

        /// <summary>
        /// StatusChanged JSON property name.
        /// </summary>
        public const string STATUS_CHANGED = "statusChanged";

        /// <summary>
        /// TransitioningToStatus JSON property name.
        /// </summary>
        public const string TRANSITIONING_TO_STATUS = "transitioningToStatus";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        // ----- PROPERTIES -----

        /// <summary>
        /// Embedded resources related to the User. JSON HAL format.
        /// </summary>
        [JsonPropertyName(EMBEDDED)]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the User.
        /// </summary>
        [JsonPropertyName(LINKS)]
        public UserLinksJson Links { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's transition to ACTIVE status completed.
        /// </summary>
        [JsonPropertyName(ACTIVATED)]
        public DateTime? Activated { get; init; } = default!;

        /// <summary>
        /// Timestamp when User was created.
        /// </summary>
        [JsonPropertyName(CREATED)]
        public DateTime? Created { get; init; } = default!;

        /// <summary>
        /// The User's primary authentication and recovery credentials.
        /// </summary>
        [JsonPropertyName(CREDENTIALS)]
        public UserCredentialsJson Credentials { get; init; } = default!;

        /// <summary>
        /// Unique key for User.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp of the User's last login.
        /// </summary>
        [JsonPropertyName(LAST_LOGIN)]
        public DateTime? LastLogin { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's profile was last updated.
        /// </summary>
        [JsonPropertyName(LAST_UPDATED)]
        public DateTime? LastUpdated { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's password last changed.
        /// </summary>
        [JsonPropertyName(PASSWORD_CHANGED)]
        public DateTime? PasswordChanged { get; init; } = default!;

        /// <summary>
        /// The User's profile properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public UserProfileJson Profile { get; init; } = default!;

        /// <summary>
        /// The current status of the user.
        /// </summary>
        [JsonPropertyName(STATUS)]
        public string Status { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's status last changed.
        /// </summary>
        [JsonPropertyName(STATUS_CHANGED)]
        public DateTime? StatusChanged { get; init; } = default!;

        /// <summary>
        /// Target status of a User's in-progress asynchronous status transition.
        /// </summary>
        [JsonPropertyName(TRANSITIONING_TO_STATUS)]
        public string TransitioningToStatus { get; init; } = default!;

        /// <summary>
        /// Determines the User's schema.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public UserTypeJson Type { get; init; } = default!;
    }
}
