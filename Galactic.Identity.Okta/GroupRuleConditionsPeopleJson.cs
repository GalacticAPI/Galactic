using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Conditions People objects for JSON formatting.
    /// </summary>
    public record GroupRuleConditionsPeopleJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Users JSON property name.
        /// </summary>
        public const string USERS = "users";

        /// <summary>
        /// Groups JSON property name.
        /// </summary>
        public const string GROUPS = "groups";

        // ----- PROPERTIES -----

        /// <summary>
        /// Groups to exclude from the group rule.
        /// </summary>
        [JsonPropertyName(GROUPS)]
        public GroupRuleConditionsPeopleExcludeJson Groups { get; init; } = default!;

        /// <summary>
        /// Users to exclude from the group rule.
        /// </summary>
        [JsonPropertyName(USERS)]
        public GroupRuleConditionsPeopleExcludeJson Users { get; init; } = default!;
    }
}
