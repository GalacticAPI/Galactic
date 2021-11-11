using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// IIdentityObject is an interface that defines functionality that all identity
    /// object classes (Users, Groups, etc.) implemented by the Galactic API should
    /// support.
    /// </summary>
    public interface IIdentityObject : IComparable<IIdentityObject>, IEqualityComparer<IIdentityObject>
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public List<IGroup> Groups { get; }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// The type or category of the object.
        /// </summary>
        public string Type { get; set; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Addes the object to the supplied group.
        /// </summary>
        /// <param name="group">The group to add the object to.</param>
        /// <returns>Thrus if the object was added, false otherwise.</returns>
        public bool AddToGroup(IGroup group)
        {
            if (group != null)
            {
                return group.AddMembers(new() { this });
            }
            return false;
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public List<IdentityAttribute<Object>> GetAttributes(List<string> names);

        /// <summary>
        /// Removes the identity object from the supplied group.
        /// </summary>
        /// <param name="group">The group to remove the object from.</param>
        /// <returns>True if the object was removed, false otherwise.</returns>
        public bool RemoveFromGroup(IGroup group)
        {
            if (group != null)
            {
                return group.RemoveMembers(new() { this });
            }
            return false;
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive);

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes);

        // ----- IEQUALITYCOMPARER METHODS -----

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public new bool Equals(IIdentityObject x, IIdentityObject y)
        {
            if (x != null && y != null)
            {
                return x.UniqueId.Equals(y.UniqueId);
            }
            else
            {
                if (x == null)
                {
                    throw new ArgumentNullException(nameof(x));
                }
                else
                {
                    throw new ArgumentNullException(nameof(y));
                }
            }
        }

        /// <summary>
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        static new public int GetHashCode(IIdentityObject obj)
        {
            if (obj != null)
            {
                return obj.UniqueId.GetHashCode();
            }
            else
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }

        // ----- END IEQUALITYCOMPARER METHODS -----

        // ----- ICOMPARABLE METHODS -----

        /// <summary>
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 iif the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public new int CompareTo(IIdentityObject other)
        {
            if (other != null)
            {
                return string.Compare(UniqueId, other.UniqueId, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentNullException(nameof(other));
            }
        }

        // ----- END ICOMPARABLE METHODS -----
    }
}
