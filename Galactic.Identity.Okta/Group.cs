using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.Okta
{
    public class Group : Identity.Group
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the Group in Okta.
        /// </summary>
        protected GroupJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public override List<Identity.User> AllUserMembers => UserMembers;

        /// <summary>
        /// Logins of all users that are a member of this group or a subgroup.
        /// </summary>
        public override List<string> AllUserMemberNames => UserMemberNames;

        /// <summary>
        /// Timestamp when Group was created.
        /// </summary>
        [DirectorySystemPropertyName(GroupJson.CREATED)]
        public DateTime? Created => json.Created;

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public override DateTime? CreationTime => Created;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [DirectorySystemPropertyName(GroupProfileJson.DESCRIPTION)]
        public override string Description
        {
            get => json.Profile.Description;
            set
            {
                // Create the profile object with the value set.
                GroupProfileJson profile = new()
                {
                    Description = value,
                    Name = Name
                };

                // Update the group with the new value.
                okta.UpdateGroup(UniqueId, profile);
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public override List<Identity.Group> Groups => new();    // Okta doesn't support nested groups.

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public override List<Identity.Group> GroupMembers => new();    // Okta doesn't support nested groups.

        /// <summary>
        /// Names of groups that are a member of the group.
        /// </summary>
        public override List<string> GroupMemberNames => new();

        /// <summary>
        /// The enumerated value of the group's Okta group type.
        /// </summary>
        public OktaClient.GroupType GroupType
        {
            get
            {
                if (Type == "APP_GROUP")
                {
                    return OktaClient.GroupType.APP_GROUP;
                }
                else if (Type == "BUILT_IN")
                {
                    return OktaClient.GroupType.BUILT_IN;
                }
                else
                {
                    return OktaClient.GroupType.OKTA_GROUP;
                }
            }
        }

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [DirectorySystemPropertyName(GroupJson.ID)]
        public string Id => json.Id;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        [DirectorySystemPropertyName(GroupJson.LAST_MEMBERSHIP_UPDATED)]
        public DateTime? LastMembershipUpdated => json.LastMembershipUpdated;

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
        [DirectorySystemPropertyName(GroupJson.LAST_UPDATED)]
        public DateTime? LastUpdated => json.LastUpdated;

        /// <summary>
        /// The members of the group.
        /// </summary>
        public override List<IdentityObject> Members => UserMembers.ConvertAll(member => (IdentityObject)member);

        /// <summary>
        /// Logins of the members of the group.
        /// </summary>
        public override List<string> MemberNames => UserMemberNames;

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        public override int MemberCount => UserMembers.Count;

        /// <summary>
        /// The name of the group.
        /// </summary>
        [DirectorySystemPropertyName(GroupProfileJson.NAME)]
        public override string Name
        {
            get => json.Profile.Name;
            set
            {
                // Create the profile object with the value set.
                GroupProfileJson profile = new()
                {
                    Description = Description,
                    Name = value
                };

                // Update the group with the new value.
                okta.UpdateGroup(UniqueId, profile);
            }
        }

        /// <summary>
        /// Determines how a Group's Profile and memberships are managed.
        /// </summary>
        [DirectorySystemPropertyName(GroupJson.TYPE)]
        public override string Type => json.Type;

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
                List<Identity.User> users = new();
                foreach (UserJson userJson in okta.GetGroupMembership(UniqueId))
                {
                    users.Add(new User(okta, userJson));
                }
                return users;
            }
        }

        /// <summary>
        /// Login of users that are a member of the group.
        /// </summary>
        public override List<string> UserMemberNames
        {
            get
            {
                List<string> names = new List<string>();

                foreach (UserJson userJson in okta.GetGroupMembership(UniqueId))
                {
                    names.Add(userJson.Profile.Login);
                }

                return names;
            }
        }

        /// <summary>
        /// A hashed version of the group's Name to allow for faster compare operations.
        /// </summary>
        public override int HashedIdentifier
        {
            get
            {
                return Name.GetHashCode();
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta group from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the group.</param>
        /// <param name="json">An object representing this group's JSON properties.</param>
        public Group(OktaClient okta, GroupJson json)
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
        /// Adds members to the group. (Skips any non-Okta members supplied.)
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public override bool AddMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                foreach (IdentityObject member in members)
                {
                    // Skip non-Okta identity objects.
                    if (member is User)
                    {
                        // The user was an Okta User.
                        if (!okta.AddUserToGroup(member.UniqueId, UniqueId))
                        {
                            // The user wasn't added.
                            return false;
                        }
                    }
                }
            }
            // All Okta Users were added, or none were supplied.
            return true;
        }

        /// <summary>
        /// Gets the correct property name to use when searching or filtering for the property with the supplied name.
        /// </summary>
        /// <param name="name">The name of the Group property to get the search name of.</param>
        /// <param name="groupType">The Okta type of the group.</param>
        /// <returns>The property's name to use while searching, or null if that property is not supported.</returns>
        public static string GetSearchPropertyName(string name, OktaClient.GroupType groupType)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Check where the property is sourced from.
                if (OktaClient.GetAllJsonPropertyNames(typeof(GroupJson)).Contains(name))
                {
                    // The property is sourced from UserJson.
                    switch (name)
                    {
                        case GroupJson.CREATED:
                            return GroupJson.CREATED;
                        case GroupJson.ID:
                            return GroupJson.ID;
                        case GroupJson.LAST_UPDATED:
                            return GroupJson.LAST_UPDATED;
                        case GroupJson.LAST_MEMBERSHIP_UPDATED:
                            return GroupJson.LAST_MEMBERSHIP_UPDATED;
                        case UserJson.TYPE:
                            return GroupJson.TYPE;
                        default:
                            return null;
                    }
                }
                else if (OktaClient.GetAllJsonPropertyNames(typeof(GroupProfileJson)).Contains(name))
                {
                    // A prefix to use before all profile properties.
                    const string PROFILE_PREFIX = "profile.";

                    // The property is sourced from GroupProfileJson.
                    return PROFILE_PREFIX + name;
                }
                else if (name == "source" && groupType == OktaClient.GroupType.APP_GROUP)
                {
                    // Returns the property name for the property with the application source id of the group.
                    return "source.id";
                }
                else
                {
                    // Who knows where this is sourced?
                    return null;
                }
            }
            else
            {
                // No property name supplied.
                return null;
            }
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group. (Okta doesn't support nested groups.)
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public override bool MemberOfGroup(Identity.Group group, bool recursive)
        {
            // Okta doesn't support nested groups.
            return false;
        }

        /// <summary>
        /// Removes identity objects from the group. (Skips any non-Okta members supplied.)
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public override bool RemoveMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                foreach (IdentityObject member in members)
                {
                    // Skip non-Okta identity objects.
                    if (member is User)
                    {
                        if (!okta.RemoveUserFromGroup(member.UniqueId, UniqueId))
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
