using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Assign User to Groups objects for JSON formatting.
    /// </summary>
    public record GroupRuleAssignUserToGroupsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Group IDs JSON property name.
        /// </summary>
        public const string GROUP_IDS = "groupIds";

        // ----- PROPERTIES -----

        /// <summary>
        /// The IDs of the groups to assign a user to as part of a group rule.
        /// </summary>
        [JsonPropertyName(GROUP_IDS)]
        public string[] GroupIds { get; init; } = default!;
    }
}
