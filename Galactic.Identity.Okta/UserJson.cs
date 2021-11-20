using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User objects for JSON formatting.
    /// </summary>
    public record UserJson
    {
        /// <summary>
        /// Embedded resources related to the User. JSON HAL format.
        /// </summary>
        [JsonPropertyName("_embedded")]
        public string Embedded { get; init; } = default!;

        /// <summary>
        /// Discoverable resources related to the User.
        /// </summary>
        [JsonPropertyName("_links")]
        public LinkJson[] Links { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's transition to ACTIVE status completed.
        /// </summary>
        [JsonPropertyName("activated")]
        public DateTime Activated { get; init; } = default!;

        /// <summary>
        /// Timestamp when User was created.
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime Created { get; init; } = default!;

        /// <summary>
        /// The User's primary authentication and recovery credentials.
        /// </summary>
        [JsonPropertyName("credentials")]
        public UserCredentialsJson Credentials { get; init; } = default!;

        /// <summary>
        /// Unique key for User.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp of the User's last login.
        /// </summary>
        [JsonPropertyName("lastLogin")]
        public DateTime LastLogin { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's profile was last updated.
        /// </summary>
        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's password last changed.
        /// </summary>
        [JsonPropertyName("passwordChanged")]
        public DateTime PasswordChanged { get; init; } = default!;

        /// <summary>
        /// The User's profile properties.
        /// </summary>
        [JsonPropertyName("profile")]
        public UserProfileJson Profile { get; init; } = default!;

        /// <summary>
        /// The current status of the user.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; init; } = default!;

        /// <summary>
        /// Timestamp when User's status last changed.
        /// </summary>
        [JsonPropertyName("statusChanged")]
        public DateTime StatusChanged { get; init; } = default!;

        /// <summary>
        /// Target status of a User's in-progress asynchronous status transition.
        /// </summary>
        [JsonPropertyName("transitioningToStatus")]
        public string TransitioningToStatus { get; init; } = default!;

        /// <summary>
        /// Determines the User's schema.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
    }
}
