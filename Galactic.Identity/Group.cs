using System;
using System.Collections;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// Group is an abstract class that defines base functionality that all group classes
    /// implemented by the Galactic API should support.
    /// </summary>
    public abstract class Group : IdentityObject, IDescriptionSupportedObject, IEnumerable<IdentityObject>
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public abstract List<User> AllUserMembers { get; }

        /// <summary>
        /// A description of the object.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public abstract List<Group> GroupMembers { get; }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public abstract List<IdentityObject> Members { get; }

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        public virtual int MemberCount => Members.Count;

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public abstract List<User> UserMembers { get; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public abstract bool AddMembers(List<IdentityObject> members);

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public virtual bool ClearMembership() => RemoveMembers(Members);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<IdentityObject> GetEnumerator()
        {
            foreach (IdentityObject member in Members)
            {
                yield return member;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes identity objects from the group.
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public abstract bool RemoveMembers(List<IdentityObject> members);
    }
}
