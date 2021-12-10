using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.Okta
{
    public class Group : IGroup
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
        public List<IUser> AllUserMembers => UserMembers;

        /// <summary>
        /// Timestamp when Group was created.
        /// </summary>
        [OktaPropertyName(GroupJson.CREATED)]
        public DateTime? Created => json.Created;

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => Created;

        /// <summary>
        /// The description of the group.
        /// </summary>
        [OktaPropertyName(GroupProfileJson.DESCRIPTION)]
        public string Description
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
        public List<IGroup> Groups => new();    // Okta doesn't support nested groups.

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public List<IGroup> GroupMembers => new();    // Okta doesn't support nested groups.

        /// <summary>
        /// Unique key for Group.
        /// </summary>
        [OktaPropertyName(GroupJson.ID)]
        public string Id => json.Id;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        [OktaPropertyName(GroupJson.LAST_MEMBERSHIP_UPDATED)]
        public DateTime LastMembershipUpdated => json.LastMembershipUpdated;

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
        [OktaPropertyName(GroupJson.LAST_UPDATED)]
        public DateTime LastUpdated => json.LastUpdated;

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
        [OktaPropertyName(GroupProfileJson.NAME)]
        public string Name
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
        [OktaPropertyName(GroupJson.TYPE)]
        public string Type => json.Type;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId => Id;

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public List<IUser> UserMembers
        {
            get
            {
                List<IUser> users = new();
                foreach (UserJson userJson in okta.GetGroupMembership(UniqueId))
                {
                    users.Add(new User(okta, userJson));
                }
                return users;
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
        public bool AddMembers(List<IIdentityObject> members)
        {
            if (members != null)
            {
                foreach (IIdentityObject member in members)
                {
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
                    foreach (OktaPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<OktaPropertyNameAttribute>())
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
        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            // Okta doesn't support nested groups.
            return false;
        }

        /// <summary>
        /// Removes identity objects from the group. (Skips any non-Okta members supplied.)
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public bool RemoveMembers(List<IIdentityObject> members)
        {
            if (members != null)
            {
                foreach (IIdentityObject member in members)
                {
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
                    foreach (OktaPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<OktaPropertyNameAttribute>())
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
