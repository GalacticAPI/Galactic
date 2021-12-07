using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        public DateTime? Created => json.Created;

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => Created;

        /// <summary>
        /// The description of the group.
        /// </summary>
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
        public string Id => json.Id;

        /// <summary>
        /// Timestamp when Group's memberships were last updated.
        /// </summary>
        public DateTime LastMembershipUpdated => json.LastMembershipUpdated;

        /// <summary>
        /// Timestamp when Group's profile was last updated.
        /// </summary>
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
        
        public bool AddMembers(List<IIdentityObject> members)
        {
            throw new NotImplementedException();
        }

        public bool ClearMembership()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IIdentityObject other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            throw new NotImplementedException();
        }

        public List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IIdentityObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] IIdentityObject obj)
        {
            throw new NotImplementedException();
        }

        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            throw new NotImplementedException();
        }

        public bool RemoveMembers(List<IIdentityObject> members)
        {
            throw new NotImplementedException();
        }

        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<object>> attributes)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
