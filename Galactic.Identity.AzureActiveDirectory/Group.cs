using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GraphGroup = Microsoft.Graph.Group;

namespace Galactic.Identity.AzureActiveDirectory
{
    public class Group : IGroup
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        protected AzureActiveDirectoryClient aad = null;

        protected GraphGroup graphGroup = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public List<IUser> AllUserMembers
        {
            get
            {
                return aad.GetUserMembers(UniqueId, true);
            }
        }

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public List<IGroup> GroupMembers
        {
            get
            {
                return aad.GetGroupMembers(UniqueId, true);
            }
        }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public List<IIdentityObject> Members
        {
            get
            {
                //Members.AddRange(UserMembers.ConvertAll<IIdentityObject>(member => member));
                //Members.AddRange(GroupMembers.ConvertAll<IIdentityObject>(member => member));
                List <IIdentityObject> temp = new ();
                temp.AddRange(UserMembers.ConvertAll<IIdentityObject>(member => member));
                temp.AddRange(GroupMembers.ConvertAll<IIdentityObject>(member => member));


                return temp;
            }
        }

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        public int MemberCount
        {
            get
            {
                return Members.Count;
            }
        }

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public List<IUser> UserMembers
        {
            get
            {
                return aad.GetUserMembers(UniqueId, false);
            }
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime
        {
            get
            {
                return graphGroup.CreatedDateTime.Value.DateTime;
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public List<IGroup> Groups
        {
            get
            {
                return aad.GetGroupMembership(UniqueId, true);
            }
        }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public string Type
        {
            get
            {
                return "Group";
            }
        }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId
        {
            get
            {
                return graphGroup.Id;
            }
        }

        /// <summary>
        /// A description of the object.
        /// </summary>
        public string Description
        {
            get
            {
                return graphGroup.Description;
            }
            set
            {

            }
        }

        /// <summary>
        /// The name of the group.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return graphGroup.DisplayName;
            }
        }

        // ----- CONSTRUCTORS -----

        public Group(AzureActiveDirectoryClient aad, GraphGroup graphGroup)
        {
            if (aad != null && graphGroup != null)
            {
                // Initialize the client.
                this.aad = aad;

                // Initialize the source GraphUser data.
                this.graphGroup = graphGroup;
            }
            else
            {
                if (aad == null)
                {
                    throw new ArgumentNullException(nameof(aad));
                }
                else
                {
                    throw new ArgumentNullException(nameof(graphGroup));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public bool AddMembers(List<IIdentityObject> members)
        {
            return false;
        }

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public bool ClearMembership()
        {
            return false;
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public List<IdentityAttribute<Object>> GetAttributes(List<string> names)
        {
            return null;
        }

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes)
        {
            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator<IIdentityObject> IEnumerable<IIdentityObject>.GetEnumerator()
        {
            return ((IGroup)this).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<IIdentityObject> GetEnumerator()
        {
            foreach (IIdentityObject member in Members)
            {
                yield return member;
            }
        }

        /// <summary>
        /// Removes identity objects from the group.
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public bool RemoveMembers(List<IIdentityObject> members)
        {
            return false;
        }

        /// <summary>
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 iif the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public int CompareTo(IIdentityObject other)
        {
            return ((IIdentityObject)this).CompareTo(other);
        }

        /// <summary>
        /// Compares this User to another User.
        /// </summary>
        /// <param name="other">The other User to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(Group other)
        {
            if (other != null)
            {
                return string.Compare(UniqueId.ToString(), other.UniqueId.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("other");
            }
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
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first User to check.</param>
        /// <param name="y">The second User to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(Group x, Group y)
        {
            if (x != null && y != null)
            {
                return x.UniqueId.Equals(y.UniqueId);
            }
            else
            {
                if (x == null)
                {
                    throw new ArgumentNullException("x");
                }
                else
                {
                    throw new ArgumentNullException("y");
                }
            }
        }

        /// <summary>
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public int GetHashCode(IIdentityObject obj) => IIdentityObject.GetHashCode(obj);

        /// <summary>
        /// Generates a hash code for the User supplied.
        /// </summary>
        /// <param name="obj">The User to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(Group obj)
        {
            if (obj != null)
            {
                return obj.UniqueId.GetHashCode();
            }
            else
            {
                throw new ArgumentNullException("obj");
            }
        }

    }
}

