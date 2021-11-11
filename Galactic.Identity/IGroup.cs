using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// Group is an interface that defines functionality that all group classes
    /// implemented by the Galactic API should support.
    /// </summary>
    public interface IGroup : IIdentityObject, IDescriptionSupportedObject, IEnumerable<IIdentityObject>
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        public List<IUser> AllUserMembers { get; }

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public List<IGroup> GroupMembers { get; }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public List<IIdentityObject> Members { get; }

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        public int MemberCount { get; }

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public List<IUser> UserMembers { get; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public bool AddMembers(List<IIdentityObject> members);

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public bool ClearMembership();

        /// <summary>
        /// Removes identity objects from the group.
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public bool RemoveMembers(List<IIdentityObject> members);

        // ----- IENUMERABLE METHODS -----

        new public IEnumerator<IIdentityObject> GetEnumerator()
        {
            foreach (IIdentityObject member in Members)
            {
                yield return member;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (IIdentityObject member in Members)
            {
                yield return member;
            }
        }

        // ----- END IENUMERABLE METHODS -----
    }
}
