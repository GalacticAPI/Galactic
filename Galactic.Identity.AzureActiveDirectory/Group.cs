using System;
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

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime { get; }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public List<IGroup> Groups { get; }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId { get; }

        /// <summary>
        /// A description of the object.
        /// </summary>
        public string Description { get; set; }

        // ----- CONSTRUCTORS -----

        public Group()
        {
        }

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
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        new public IEnumerator<IIdentityObject> GetEnumerator()
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
        public bool RemoveMembers(List<IIdentityObject> members);
        
	}
}

