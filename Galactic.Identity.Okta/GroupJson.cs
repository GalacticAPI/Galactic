using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group objects for JSON formatting.
    /// </summary>
    public record GroupJson
    {
        /// <summary>
        /// Embedded resources related to the Group. JSON HAL format.
        /// </summary>
        [JsonPropertyName("_embedded")]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the Group.
        /// </summary>
        [JsonPropertyName("_links")]
        public LinkJson[] Links { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group was created.
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime Created { get; init; } = default!;

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        [JsonPropertyName("lastMembershipUpdated")]
        public DateTime LastMembershipUpdated { get; init; } = default!;

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; init; } = default!;

        /// <summary>
        /// Determines the Group's profile.
        /// </summary>
        [JsonPropertyName("objectClass")]
        public string[] ObjectClass { get; init; } = default!;

        /// <summary>
        /// The Group's profile properties.
        /// </summary>
        [JsonPropertyName("profile")]
        public GroupProfileJson Profile { get; init; } = default!;

        /// <summary>
        /// Determines how a Group's Profile and memberships are managed.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
    }
}
