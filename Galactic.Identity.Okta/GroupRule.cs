using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.Okta
{
    public class GroupRule
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The Okta object type of a group rule.
        /// </summary>
        private const string TYPE = "group_rule";

        /// <summary>
        /// The schema type of expressions making up group rules.
        /// </summary>
        private const string EXPRESSION_TYPE = "urn:okta:expression:1.0";

        /// <summary>
        /// The value of the Status property when the rule is active.
        /// </summary>
        public const string STATUS_ACTIVE = "ACTIVE";

        /// <summary>
        /// The value of the Status property when the rule is inactive.
        /// </summary>
        public const string STATUS_INACTIVE = "INACTIVE";

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the Group Rule in Okta.
        /// </summary>
        protected GroupRuleJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Actions the rule undertakes.
        /// </summary>
        public GroupRuleActionsJson Actions => json.Actions;

        /// <summary>
        /// Whether this rule has an active status.
        /// </summary>
        public bool Active
        {
            get
            {
                if (Status == STATUS_ACTIVE)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// List of group Ids of the groups that this rule assigns users to.
        /// </summary>
        public List<string> AssignedGroupIds => new(json.Actions.AssignUserToGroups.GroupIds);

        /// <summary>
        /// Conditions that apply to the rule.
        /// </summary>
        public GroupRuleConditionsJson Conditions => json.Conditions;

        /// <summary>
        /// Timestamp when the Group Rule was created.
        /// </summary>
        public DateTime? Created => json.Created;

        /// <summary>
        /// List of user Ids of users excluded from this rule.
        /// </summary>
        public List<string> ExcludedUserIds => new(json.Conditions.People.Users.Exclude);

        /// <summary>
        /// The Okta Expression Language expression that assigns users to this rule.
        /// </summary>
        public string Expression => json.Conditions.Expression.Value;

        /// <summary>
        /// Unique key for the Group rule.
        /// </summary>
        public string Id => json.Id;

        /// <summary>
        /// Timestamp when the Group Rule was last updated.
        /// </summary>
        public DateTime? LastUpdated => json.LastUpdated;

        /// <summary>
        /// The human friendly name of the Group Rule.
        /// </summary>
        public string Name => json.Name;

        /// <summary>
        /// The Group Rule's status. (ACTIVE, INACTIVE)
        /// </summary>
        public string Status => json.Status;

        /// <summary>
        /// The type of object that the Group Rule is within Okta.
        /// </summary>
        public static string Type => TYPE;


        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta group rule from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the group rule.</param>
        /// <param name="json">An object representing this group rule's JSON properties.</param>
        public GroupRule(OktaClient okta, GroupRuleJson json)
        {
            if (okta != null && json != null)
            {
                // Initialize the client.
                this.okta = okta;

                // Initialize the backing JSON data.
                this.json = json;
            }
            else
            {
                if (okta == null)
                {
                    throw new ArgumentNullException(nameof(okta));
                }
                else
                {
                    throw new ArgumentNullException(nameof(json));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Activates this group rule within Okta.
        /// </summary>
        /// <returns>True if the rule was activated, false otherwise.</returns>
        public bool Activate()
        {
            return okta.ActivateGroupRule(Id);
        }

        /// <summary>
        /// Deactivates this group rule within Okta.
        /// </summary>
        /// <returns>True if the rule was deactivated, false otherwise.</returns>
        public bool Deactivate()
        {
            return okta.DeactivateGroupRule(Id);
        }

    }
}
