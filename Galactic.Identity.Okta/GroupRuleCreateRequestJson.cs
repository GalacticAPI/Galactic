using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Create Request objects for JSON formatting.
    /// </summary>
    public record GroupRuleCreateRequestJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Actions JSON property name.
        /// </summary>
        public const string ACTIONS = "actions";

        /// <summary>
        /// Conditions JSON property name.
        /// </summary>
        public const string CONDITIONS = "conditions";

        /// <summary>
        /// Name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        // ----- PROPERTIES -----

        /// <summary>
        /// Actions the rule undertakes.
        /// </summary>
        [JsonPropertyName(ACTIONS)]
        public GroupRuleActionsJson Actions { get; init; } = default!;

        /// <summary>
        /// Conditions that apply to the rule.
        /// </summary>
        [JsonPropertyName(CONDITIONS)]
        public GroupRuleConditionsJson Conditions { get; init; } = default!;

        /// <summary>
        /// The human friendly name of the Group Rule.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// The type of object that the Group Rule is within Okta. (group_rule)
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;
    }
}
