using SearchOperatorType = Galactic.Identity.GoogleWorkspace.GoogleWorkspaceClient.SearchOperatorType;
using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
using GoogleUser = Google.Apis.Admin.Directory.directory_v1.Data.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.GoogleWorkspace
{
    public class Group : Identity.Group
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// AdminCreated property name.
        /// </summary>
        public const string ADMIN_CREATED = "adminCreated";

        /// <summary>
        /// Aliases property name.
        /// </summary>
        public const string ALIASES = "aliases";

        /// <summary>
        /// Description property name.
        /// </summary>
        public const string DESCRIPTION = "description";

        /// <summary>
        /// DirectMembersCount property name.
        /// </summary>
        public const string DIRECT_MEMBERS_COUNT = "directMembersCount";

        /// <summary>
        /// Email property name.
        /// </summary>
        public const string EMAIL = "email";

        /// <summary>
        /// ETag property name.
        /// </summary>
        public const string ETAG = "etag";

        /// <summary>
        /// Id property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Kind property name.
        /// </summary>
        public const string KIND = "kind";

        /// <summary>
        /// Name property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// NonEditableAliases property name.
        /// </summary>
        public const string NON_EDITABLE_ALIASES = "nonEditableAliases";

        // Search fields.

        /// <summary>
        /// The group's email address. (Note: Does not include aliases.)
        /// </summary>
        public const string SEARCH_EMAIL = "email";

        /// <summary>
        /// The group's display name.
        /// </summary>
        public const string SEARCH_NAME = "name";

        /// <summary>
        /// Returns all groups for which a user or group has a membership.
        /// This value can be any of the user's primary or alias email address,
        /// a group's primary or alias email address, or a user's unique ID.
        /// </summary>
        public const string SEARCH_MEMBER_KEY = "memberKey";

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Google.
        /// </summary>
        protected GoogleWorkspaceClient gws = null;

        /// <summary>
        /// The backing native data representing the Group in Google Workspace.
        /// </summary>
        protected GoogleGroup group = null;

        /// <summary>
        /// The type of search operators supported by each searh field.
        /// </summary>
        public static Dictionary<string, SearchOperatorType[]> SearchOperatorsSupported = new()
        {
            [SEARCH_NAME] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Starts },
            [SEARCH_EMAIL] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Starts },
            [SEARCH_MEMBER_KEY] = new SearchOperatorType[] { SearchOperatorType.Exact }
        };

        // ----- PROPERTIES -----

        /// <summary>
        /// A list of the group's aliases.
        /// </summary>
        [DirectorySystemPropertyName(ALIASES)]
        public List<string> Aliases
        {
            get => new(group.Aliases);
        }

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public override List<Identity.User> AllUserMembers => UserMembers;

        /// <summary>
        /// (Google: Not supported.) The date and time that the object was created.
        /// </summary>
        public override DateTime? CreationTime => null;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [DirectorySystemPropertyName(DESCRIPTION)]
        public override string Description
        {
            get => group.Description;
            set => group = gws.UpdateGroup(UniqueId, new() { new(DESCRIPTION, value) });
        }

        /// <summary>
        /// The group's email address.
        /// </summary>
        [DirectorySystemPropertyName(EMAIL)]
        public string Email
        {
            get => group.Email;
            set => group = gws.UpdateGroup(UniqueId, new() { new(EMAIL, value) });
        }

        /// <summary>
        /// A list of the group's email addresses.
        /// The group's e-mail address will always be first in the list followed by aliases.
        /// </summary>
        public List<string> Emails
        {
            get
            {
                // Create a new list of e-mail addresses to return.
                List<string> emails = new();

                // Add the group's e-mail address.
                emails.Add(group.Email);

                // Add any other aliases the group is known by.
                if (group.Aliases != null)
                {
                    foreach (string alias in group.Aliases)
                    {
                        emails.Add(alias);
                    }
                }

                // Return the list of e-mail addresses.
                return emails;
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public override List<Identity.Group> Groups => gws.GetMemberGroups(UniqueId);

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public override List<Identity.Group> GroupMembers
        {
            get
            {
                // Create a list of groups to return.
                List<Identity.Group> groups = new();

                // Add the group members to the list.
                foreach (IdentityObject member in Members)
                {
                    if (member is Group)
                    {
                        groups.Add((Identity.Group)member);
                    }
                }

                // Return the list.
                return groups;
            }
        }

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [DirectorySystemPropertyName(ID)]
        public string Id => group.Id;

        /// <summary>
        /// The members of the group.
        /// </summary>
        public override List<IdentityObject> Members => gws.GetGroupMembership(UniqueId);

        /// <summary>
        /// The name of the group.
        /// </summary>
        [DirectorySystemPropertyName(NAME)]
        public override string Name
        {
            get => group.Name;
            set => group = gws.UpdateGroup(UniqueId, new() { new(NAME, value) });
        }

        /// <summary>
        /// Determines how a Group's Profile and memberships are managed.
        /// </summary>
        [DirectorySystemPropertyName(KIND)]
        public override string Type => group.Kind;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public override string UniqueId => Id;

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public override List<Identity.User> UserMembers
        {
            get
            {
                // Create a list of users to return.
                List<Identity.User> users = new();

                // Add the user members to the list.
                foreach (IdentityObject member in Members)
                {
                    if (member is User)
                    {
                        users.Add((Identity.User)member);
                    }
                }

                // Return the list.
                return users;
            }
        }


        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a Google Workspace group from a native object representing its properties.
        /// </summary>
        /// <param name="gws">A Google Workspace client object used to query and manipulate the group.</param>
        /// <param name="group">A Google Workspace native object representing this group's properties.</param>
        public Group(GoogleWorkspaceClient gws, GoogleGroup group)
        {
            if (gws != null && group != null)
            {
                // Initialize the client.
                this.gws = gws;

                // Initialize the group data from the native object supplied.
                this.group = group;
            }
            else
            {
                if (gws == null)
                {
                    throw new ArgumentNullException(nameof(gws));
                }
                else
                {
                    throw new ArgumentNullException(nameof(group));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Adds members to the group. (Skips any non-Google Workspace members supplied.)
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public override bool AddMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                foreach (IdentityObject member in members)
                {
                    if (member is User || member is Group)
                    {
                        // The member was an Google Workspace User or Group.
                        if (!gws.AddMemberToGroup(member.UniqueId, UniqueId))
                        {
                            // The member wasn't added.
                            return false;
                        }
                    }
                }
            }
            // All Google members were added, or none were supplied.
            return true;
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public override bool MemberOfGroup(Identity.Group group, bool recursive) => gws.GetMemberOfGroup(this, group, recursive);

        /// <summary>
        /// Removes identity objects from the group. (Skips any non-Google Workspace members supplied.)
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public override bool RemoveMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                foreach (IdentityObject member in members)
                {
                    if (member is User || member is Group)
                    {
                        if (!gws.RemoveMemberFromGroup(member.UniqueId, UniqueId))
                        {
                            // The user was not removed.
                            return false;
                        }
                    }
                }
                // All users were removed.
                return true;
            }
            // There were no objects to remove.
            return false;
        }
    }
}
