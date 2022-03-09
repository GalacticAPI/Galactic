using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Conditions Expression objects for JSON formatting.
    /// </summary>
    public record GroupRuleConditionsExpressionJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Value JSON property name.
        /// </summary>
        public const string VALUE = "value";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        // ----- PROPERTIES -----

        /// <summary>
        /// The value of the Okta Expression Language expression defining the set of users / groups to include in the rule.
        /// </summary>
        [JsonPropertyName(VALUE)]
        public string Value { get; init; } = default!;

        /// <summary>
        /// The type of Okta expression represented by this object.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;
    }
}
