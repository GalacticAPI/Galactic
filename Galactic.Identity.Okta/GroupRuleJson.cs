using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule objects for JSON formatting.
    /// </summary>
    public record GroupRuleJson
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
        /// Created JSON property name.
        /// </summary>
        public const string CREATED = "created";

        /// <summary>
        /// Id JSON property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// LastUpdated JSON property name.
        /// </summary>
        public const string LAST_UPDATED = "lastUpdated";

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
        /// Timestamp when the Group rule was created.
        /// </summary>
        [JsonPropertyName(CREATED)]
        public DateTime? Created { get; init; } = default!;

        /// <summary>
        /// Unique key for the Group rule.
        /// </summary>
        [JsonPropertyName(ID)]
        public string Id { get; init; } = default!;

        /// <summary>
        /// Timestamp when the Group Rule was last updated.
        /// </summary>
        [JsonPropertyName(LAST_UPDATED)]
        public DateTime? LastUpdated { get; init; } = default!;

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
