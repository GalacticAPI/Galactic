using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule UpdateRequest objects for JSON formatting.
    /// </summary>
    public record GroupRuleUpdateRequestJson
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
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Status JSON property name.
        /// </summary>
        public const string STATUS = "status";

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
        /// Unique key for the Group rule.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// The human friendly name of the Group Rule.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// The Group Rule's status. (ACTIVE,INACTIVE)
        /// </summary>
        [JsonPropertyName(STATUS)]
        public string Status { get; init; } = default!;

        /// <summary>
        /// The type of object that the Group Rule is within Okta. (group_rule)
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;
    }
}
