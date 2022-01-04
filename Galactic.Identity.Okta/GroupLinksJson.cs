using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Links objects for JSON formatting.
    /// </summary>
    public record GroupLinksJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Apps JSON property name.
        /// </summary>
        public const string APPS = "apps";

        /// <summary>
        /// Logo JSON property name.
        /// </summary>
        public const string LOGO = "logo";

        /// <summary>
        /// Self JSON property name.
        /// </summary>
        public const string SELF = "self";

        /// <summary>
        /// Source JSON property name.
        /// </summary>
        public const string SOURCE = "source";

        /// <summary>
        /// Users JSON property name.
        /// </summary>
        public const string USERS = "users";

        // ----- PROPERTIES -----

        /// <summary>
        /// Lists all applications that are assigned to the Group.
        /// </summary>
        [JsonPropertyName(APPS)]
        public LinkJson Apps { get; init; } = default!;

        /// <summary>
        /// Provides links to logo images for the Group if available.
        /// </summary>
        [JsonPropertyName(LOGO)]
        public LinkJson[] Logo { get; init; } = default!;

        /// <summary>
        /// The primary URL for the Group.
        /// </summary>
        [JsonPropertyName(SELF)]
        public LinkJson Self { get; init; } = default!;

        /// <summary>
        /// The URL for the source application of the group. This link attribute is only present in groups of APP_GROUP type.
        /// </summary>
        [JsonPropertyName(SOURCE)]
        public LinkJson Source { get; init; } = default!;

        /// <summary>
        /// Provides Group member operations for the Group.
        /// </summary>
        [JsonPropertyName(USERS)]
        public LinkJson Users { get; init; } = default!;
    }
}
