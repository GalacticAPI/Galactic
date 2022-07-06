using System;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace Galactic.Identity.Okta
{
    public record AppUserJson
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
        /// Created JSON property name.
        /// </summary>
        public const string CREATED = "created";

        /// <summary>
        /// Credentials JSON property name.
        /// </summary>
        public const string CREDENTIALS = "credentials";

        /// <summary>
        /// External ID JSON property name.
        /// </summary>
        public const string EXTERNAL_ID = "externalId";

        /// <summary>
        /// ID JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// LastSync JSON property name.
        /// </summary>
        public const string LAST_SYNC = "lastSync";

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
        /// Scope JSON property name.
        /// </summary>
        public const string SCOPE = "scope";

        /// <summary>
        /// Status JSON property name.
        /// </summary>
        public const string STATUS = "status";

        /// <summary>
        /// StatusChanged JSON property name.
        /// </summary>
        public const string STATUS_CHANGED = "statusChanged";

        /// <summary>
        /// SyncState JSON property name.
        /// </summary>
        public const string SYNC_STATE = "syncState";

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
        /// Id of user in target app (must be imported or provisioned).
        /// </summary>
        [JsonPropertyName(EXTERNAL_ID)]
        public string ExternalId { get; init; } = default!;

        /// <summary>
        /// Unique key for User.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp when last sync operation was executed.
        /// </summary>
        [JsonPropertyName(LAST_SYNC)]
        public DateTime? LastSync { get; init; } = default!;

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
        public JsonObject Profile { get; init; } = default!;

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
        /// Synchronization state for app user.
        /// </summary>
        [JsonPropertyName(SYNC_STATE)]
        public string SyncState { get; init; } = default!;
    }
}

