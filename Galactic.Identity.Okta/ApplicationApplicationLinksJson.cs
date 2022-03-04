using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Links objects for JSON formatting.
    /// </summary>
    public record ApplicationApplicationLinksJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// App Links JSON property name.
        /// </summary>
        public const string APP_LINKS = "appLinks";

        /// <summary>
        /// Deactivate JSON property name.
        /// </summary>
        public const string DEACTIVATE = "deactivate";

        /// <summary>
        /// Groups JSON property name.
        /// </summary>
        public const string GROUPS = "groups";

        /// <summary>
        /// Help JSON property name.
        /// </summary>
        public const string HELP = "help";

        /// <summary>
        /// Logo JSON property name.
        /// </summary>
        public const string LOGO = "logo";

        /// <summary>
        /// Metadata JSON property name.
        /// </summary>
        public const string METADATA = "metadata";

        /// <summary>
        /// Upload Logo JSON property name.
        /// </summary>
        public const string UPLOAD_LOGO = "uploadLogo";

        /// <summary>
        /// Users JSON property name.
        /// </summary>
        public const string USERS = "users";


        // ----- PROPERTIES -----

        /// <summary>
        /// A link to the application within Okta.
        /// </summary>
        [JsonPropertyName(APP_LINKS)]
        public LinkJson[] AppLinks { get; init; } = default!;

        /// <summary>
        /// A link to deactivate the application within Okta.
        /// </summary>
        [JsonPropertyName(DEACTIVATE)]
        public LinkJson Deactivate { get; init; } = default!;

        /// <summary>
        /// A link to retrieve the groups associated with the application within Okta.
        /// </summary>
        [JsonPropertyName(GROUPS)]
        public LinkJson Groups { get; init; } = default!;

        /// <summary>
        /// A link to Okta's help documentation concerning the application's configuration / setup.
        /// </summary>
        [JsonPropertyName(HELP)]
        public LinkJson Help { get; init; } = default!;

        /// <summary>
        /// A link to retrieve the logo associated with the application within Okta.
        /// </summary>
        [JsonPropertyName(LOGO)]
        public LinkJson[] Logo { get; init; } = default!;

        /// <summary>
        /// A link to the application's metadata within Okta.
        /// </summary>
        [JsonPropertyName(METADATA)]
        public LinkJson Metadata { get; init; } = default!;

        /// <summary>
        /// A link to upload a logo for the application within Okta.
        /// </summary>
        [JsonPropertyName(UPLOAD_LOGO)]
        public LinkJson UploadLogo { get; init; } = default!;

        /// <summary>
        /// A link to retrieve the users associated with the application within Okta.
        /// </summary>
        [JsonPropertyName(USERS)]
        public LinkJson Users { get; init; } = default!;

    }
}
