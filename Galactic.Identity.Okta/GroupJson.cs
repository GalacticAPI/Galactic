using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group objects for JSON formatting.
    /// </summary>
    public record GroupJson
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
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// LastMembershipUpdated JSON property name.
        /// </summary>
        public const string LAST_MEMBERSHIP_UPDATED = "lastMembershipUpdated";

        /// <summary>
        /// LastUpdated JSON property name.
        /// </summary>
        public const string LAST_UPDATED = "lastUpdated";

        /// <summary>
        /// ObjectClass JSON property name.
        /// </summary>
        public const string OBJECT_CLASS = "objectClass";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        // ----- PROPERTIES -----

        /// <summary>
        /// Embedded resources related to the Group. JSON HAL format.
        /// </summary>
        [JsonPropertyName(EMBEDDED)]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the Group.
        /// </summary>
        [JsonPropertyName(LINKS)]
        public LinkJson[] Links { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group was created.
        /// </summary>
        [JsonPropertyName(CREATED)]
        public DateTime Created { get; init; } = default!;

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        [JsonPropertyName(LAST_MEMBERSHIP_UPDATED)]
        public DateTime LastMembershipUpdated { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
        [JsonPropertyName(LAST_UPDATED)]
        public DateTime LastUpdated { get; init; } = default!;

        /// <summary>
        /// Determines the Group's profile.
        /// </summary>
        [JsonPropertyName(OBJECT_CLASS)]
        public string[] ObjectClass { get; init; } = default!;

        /// <summary>
        /// The Group's profile properties.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public GroupProfileJson Profile { get; init; } = default!;

        /// <summary>
        /// Determines how a Group's Profile and memberships are managed.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;
    }
}
