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
    public class Group : IGroup
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
        [GoogleWorkspacePropertyName(ALIASES)]
        public List<string> Aliases
        {
            get => new(group.Aliases);
        }

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public List<IUser> AllUserMembers => UserMembers;

        /// <summary>
        /// Timestamp when Group was created.
        /// </summary>
        public DateTime? Created => throw new NotImplementedException();

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => Created;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [GoogleWorkspacePropertyName(DESCRIPTION)]
        public string Description
        {
            get => group.Description;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The group's email address.
        /// </summary>
        [GoogleWorkspacePropertyName(EMAIL)]
        public string Email
        {
            get => group.Email;
            set => throw new NotImplementedException();
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
        public List<IGroup> Groups => throw new NotImplementedException();

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public List<IGroup> GroupMembers => throw new NotImplementedException();

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [GoogleWorkspacePropertyName(ID)]
        public string Id => group.Id;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        public DateTime? LastMembershipUpdated => throw new NotImplementedException();

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
        public DateTime? LastUpdated => throw new NotImplementedException();

        /// <summary>
        /// The members of the group.
        /// </summary>
        public List<IIdentityObject> Members => UserMembers.ConvertAll<IIdentityObject>(member => member);

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        public int MemberCount => UserMembers.Count;

        /// <summary>
        /// The name of the group.
        /// </summary>
        [GoogleWorkspacePropertyName(NAME)]
        public string Name
        {
            get => group.Name;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Determines how a Group's Profile and memberships are managed.
        /// </summary>
        [GoogleWorkspacePropertyName(KIND)]
        public string Type => group.Kind;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId => Id;

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public List<IUser> UserMembers
        {
            get => throw new NotImplementedException();
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
        public bool AddMembers(List<IIdentityObject> members)
        {
            throw new NotImplementedException();
            /*
            if (members != null)
            {
                foreach (IIdentityObject member in members)
                {
                    if (member is User)
                    {
                        // The user was an Google Workspace User.
                        if (!gws.AddUserToGroup(member.UniqueId, UniqueId))
                        {
                            // The user wasn't added.
                            return false;
                        }
                    }
                }
            }
            // All Google Users were added, or none were supplied.
            return true;
            */
        }

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public bool ClearMembership()
        {
            return RemoveMembers(Members);
        }

        /// <summary>
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public int CompareTo(IIdentityObject other)
        {
            return ((IIdentityObject)this).CompareTo(other);
        }

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            return ((IIdentityObject)this).Equals(x, y);
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>

        public List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            // Create a list of IdentityAttributes to return.
            List<IdentityAttribute<object>> attributes = new();

            if (names != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(Group).GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (GoogleWorkspacePropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<GoogleWorkspacePropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }

                // Fill the list of IdentityAttributes with the name and value of the attribute with the supplied name.
                foreach (string name in names)
                {
                    if (properties.ContainsKey(name))
                    {
                        attributes.Add(new(name, properties[name].GetValue(this)));
                    }
                }
            }

            // Return the attributes found.
            return attributes;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<IIdentityObject> GetEnumerator()
        {
            return ((IGroup)this).GetEnumerator();
        }

        /// <summary>
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public int GetHashCode([DisallowNull] IIdentityObject obj)
        {
            return IIdentityObject.GetHashCode(obj);
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive) => throw new NotImplementedException();

        /// <summary>
        /// Removes identity objects from the group. (Skips any non-Google Workspace members supplied.)
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public bool RemoveMembers(List<IIdentityObject> members)
        {
            throw new NotImplementedException();
            /*
            if (members != null)
            {
                foreach (IIdentityObject member in members)
                {
                    if (member is User)
                    {
                        if (!gws.RemoveUserFromGroup(member.UniqueId, UniqueId))
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
            */
        }

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<object>> attributes)
        {
            // Create a list of IdentityAttributes to return with success or failure.
            List<IdentityAttribute<bool>> attributeResults = new();

            if (attributes != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(Group).GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (GoogleWorkspacePropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<GoogleWorkspacePropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }

                // Iterate over all the attributes supplied, setting their values and marking success or failure in the attribute list to return.
                foreach (IdentityAttribute<object> attribute in attributes)
                {
                    // Check if the attribute supplied matches a property of the User.
                    if (properties.ContainsKey(attribute.Name))
                    {
                        // Set the property with the attribute value supplied.
                        try
                        {
                            properties[attribute.Name].SetValue(this, attribute.Value);
                            attributeResults.Add(new(attribute.Name, true));
                        }
                        catch
                        {
                            // There was an error setting the attribute's value.
                            attributeResults.Add(new(attribute.Name, false));

                        }
                    }
                }
            }

            // Return the success / failure results of settings the attributes.
            return attributeResults;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
