using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.Okta
{
    public class Application
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the Application in Okta.
        /// </summary>
        protected ApplicationJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Gets list of Sign On Attribute Statements.
        /// </summary>
        public List<ApplicationSignOnAttributeStatementJson> AttributeStatements
        {
            get
            {
                if(json.Settings != null && json.Settings.SignOn != null && json.Settings.SignOn.AttributeStatements != null)
                {
                    return json.Settings.SignOn.AttributeStatements.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// List of group assignments for an application.
        /// </summary>
        public List<Identity.Group> GroupAssignments
        {
            get
            {
                return okta.GetApplicationGroupAssignments(Id);
            }
        }

        /// <summary>
        /// Unique key for the Application.
        /// </summary>
        [DirectorySystemPropertyName(ApplicationJson.ID)]
        public string Id => json.Id;

        /// <summary>
        /// User defined display name for app.
        /// </summary>
        [DirectorySystemPropertyName(ApplicationJson.LABEL)]
        public string Label => json.Label;

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta application from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the application.</param>
        /// <param name="json">An object representing this application's JSON properties.</param>
        public Application(OktaClient okta, ApplicationJson json)
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
        /// Assigns a group to the application.
        /// </summary>
        /// <param name="groupId">The unique id of the group to assign.</param>
        /// <returns>True if the group was assigned, false otherwise.</returns>
        public bool AssignGroup(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                return okta.AssignGroupToApplication(groupId, Id);
            }
            else
            {
                // A group id was not provided.
                return false;
            }
        }

        /// <summary>
        /// Gets the application assignment group JSON object associated with the specified group assigned to the application.
        /// </summary>
        /// <param name="groupId">The ID of the group to retrieve assignment information about.</param>
        /// <returns>An ApplicationApplicationGroupJson object or null if not found or there was error.</returns>
        public ApplicationApplicationGroupJson GetAssignedGroupJson(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                return okta.GetAssignedGroupForApplication(Id, groupId);
            }
            else
            {
                // A group id was not provided.
                return null;
            }
        }

        /// <summary>
        /// Returnes whether the supplied group is assigned to the application.
        /// </summary>
        /// <param name="groupId">The unique id of the group to check for assignment.</param>
        /// <returns>True if the group is assigned, false otherwise.</returns>
        public bool IsGroupAssigned(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                if (GetAssignedGroupJson(groupId) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(groupId));
            }
        }

        /// <summary>
        /// Removes a group assignment from the application.
        /// </summary>
        /// <param name="groupId">The unique id of the group to remove.</param>
        /// <returns>True if the group was removed, false otherwise.</returns>
        public bool RemoveGroupAssignment(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                return okta.RemoveGroupAssignmentFromApplication(groupId, Id);
            }
            else
            {
                // A group id was not provided.
                return false;
            }
        }
    }
}
