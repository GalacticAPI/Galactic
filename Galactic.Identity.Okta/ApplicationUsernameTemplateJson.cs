using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application Username Template objects for JSON formatting.
    /// </summary>
    public record ApplicationUsernameTemplateJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Push Status JSON property name.
        /// </summary>
        public const string PUSH_STATUS = "pushStatus";

        /// <summary>
        /// Template JSON property name.
        /// </summary>
        public const string TEMPLATE = "template";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        /// <summary>
        /// User Suffix JSON property name.
        /// </summary>
        public const string USER_SUFFIX = "userSuffix";

        // Mapping Expression Types.
        public const string TYPE_BUILT_IN = "BUILT_IN";
        public const string TYPE_CUSTOM = "CUSTOM";
        public const string TYPE_NONE = "NONE";

        // Push Statuses.
        public const string PUSH_STATUS_DONT_PUSH = "DONT_PUSH";
        public const string PUSH_STATUS_PUSH = "PUSH";

        // ----- PROPERTIES -----

        /// <summary>
        /// Push username on update.
        /// The value of this property is one of the PUSH_STATUS_* constants in this record.
        /// </summary>
        [JsonPropertyName(PUSH_STATUS)]
        public string PushStatus { get; init; } = default!;

        /// <summary>
        /// Mapping expression for username.
        /// </summary>
        [JsonPropertyName(TEMPLATE)]
        public string Template { get; init; } = default!;

        /// <summary>
        /// Type of mapping expression.
        /// The value of this property is one of the TYPE_* constants in this record.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;

        /// <summary>
        /// Suffix for built-in mapping expressions.
        /// </summary>
        [JsonPropertyName(USER_SUFFIX)]
        public string UserSuffix { get; init; } = default!;
    }
}
