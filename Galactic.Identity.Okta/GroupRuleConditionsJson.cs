using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Conditions objects for JSON formatting.
    /// </summary>
    public record GroupRuleConditionsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// People JSON property name.
        /// </summary>
        public const string PEOPLE = "people";

        /// <summary>
        /// Expression JSON property name.
        /// </summary>
        public const string EXPRESSION = "expression";

        // ----- PROPERTIES -----

        /// <summary>
        /// The Okta Expression Language expression defining the set of users / groups to include in the rule.
        /// </summary>
        [JsonPropertyName(EXPRESSION)]
        public GroupRuleConditionsExpressionJson Expression { get; init; } = default!;

        /// <summary>
        /// Users and groups to exclude from the group rule.
        /// </summary>
        [JsonPropertyName(PEOPLE)]
        public GroupRuleConditionsPeopleJson People { get; init; } = default!;
    }
}
