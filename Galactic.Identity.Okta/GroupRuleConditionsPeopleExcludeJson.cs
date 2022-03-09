using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Conditions People Exclude objects for JSON formatting.
    /// </summary>
    public record GroupRuleConditionsPeopleExcludeJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Exclude JSON property name.
        /// </summary>
        public const string EXCLUDE = "exclude";

        // ----- PROPERTIES -----

        /// <summary>
        /// A list of user or group ids to exlude from a Group Rule People condition.
        /// </summary>
        [JsonPropertyName(EXCLUDE)]
        public string[] Exclude { get; init; } = default!;
    }
}
