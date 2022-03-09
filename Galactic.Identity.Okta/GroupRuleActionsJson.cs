using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Group Rule Actions objects for JSON formatting.
    /// </summary>
    public record GroupRuleActionsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Assign User To Groups JSON property name.
        /// </summary>
        public const string ASSIGN_USER_TO_GROUPS = "assignUserToGroups";

        // ----- PROPERTIES -----

        /// <summary>
        /// Groups to assign a user to as part of the rule.
        /// </summary>
        [JsonPropertyName(ASSIGN_USER_TO_GROUPS)]
        public GroupRuleAssignUserToGroupsJson AssignUserToGroups { get; init; } = default!;
    }
}
