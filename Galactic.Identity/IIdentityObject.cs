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
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public new virtual int CompareTo(IIdentityObject other)
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

        /// <summary>
        /// Compares the identity object to another identity object using the supplied attribute names as the attributes to compare against.
        /// </summary>
        /// <typeparam name="T">The type of the attributes to compare. Both attributes must be of the same type.</typeparam>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <param name="attributeName">The name of the attribute in this object to use when comparing.</param>
        /// <param name="otherAttributeName">The nameof the attribute in the other object to use when comparing.</param>
        /// <returns>1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public virtual int CompareTo<T>(IIdentityObject other, string attributeName, string otherAttributeName) where T : IComparable<T>
        {
            // Verify that all parameters were supplied.
            if (other != null && !string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(otherAttributeName))
            {
                // Populate the Compare method parameters.
                List<Object> parameters = new();
                parameters.Add(GetAttributes(new() { attributeName }));
                parameters.Add(other.GetAttributes(new() { otherAttributeName }));
                // Ignore case in string compares.
                if (typeof(string) is T)
                {
                    parameters.Add(StringComparison.OrdinalIgnoreCase);
                }

                // Call the type's Compare method.
                return (int)typeof(T).GetMethod("Compare").Invoke(null, parameters.ToArray());
            }
            else
            {
                if (other == null)
                {
                    throw new ArgumentNullException(nameof(other));
                }
                else if (string.IsNullOrWhiteSpace(attributeName))
                {
                    throw new ArgumentNullException(nameof(attributeName));
                }
                else
                {
                    throw new ArgumentNullException(nameof(otherAttributeName));
                }
            }
        }

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public new virtual bool Equals(IIdentityObject x, IIdentityObject y)
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
        /// Checks whether x and y are equal using the supplied attribute names as the attributes for checking equality against.
        /// </summary>
        /// <typeparam name="T">The type of the attributes to compare. Both attributes must be of the same type.</typeparam>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="xAttributeName">The name of the attribute in x object to use when comparing.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <param name="yAttributeName">The name of the attribute in y object to use when comparing.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public virtual bool Equals<T>(IIdentityObject x, string xAttributeName, IIdentityObject y, string yAttributeName)
        {
            if (x != null & y != null & !string.IsNullOrWhiteSpace(xAttributeName) && !string.IsNullOrWhiteSpace(yAttributeName))
            {
                // Get the attributes of each object.
                List<IdentityAttribute<object>> xAttributes = x.GetAttributes(new() { xAttributeName });
                List<IdentityAttribute<object>> yAttributes = y.GetAttributes(new() { yAttributeName });

                // Verify that attributes were found with the supplied names.
                if (xAttributes.Count == 1 && yAttributes.Count == 1)
                {
                    T xAttribute = (T)xAttributes[0].Value;
                    T yAttribute = (T)yAttributes[0].Value;
                    return xAttribute.Equals(yAttribute);
                }
                else
                {
                    // One or more of the attribute names supplied did not exist in the attribute list of the object.
                    return false;
                }    
            }
            else
            {
                if (x == null)
                {
                    throw new ArgumentNullException(nameof(x));
                }
                else if (y == null)
                {
                    throw new ArgumentNullException(nameof(y));
                }
                else if (string.IsNullOrWhiteSpace(xAttributeName))
                {
                    throw new ArgumentNullException(nameof(xAttributeName));
                }
                else
                {
                    throw new ArgumentNullException(nameof(yAttributeName));
                }
            }
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public List<IdentityAttribute<Object>> GetAttributes(List<string> names);

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

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            if (group != null)
            {
                // Search all the groups which this object is a member.
                foreach (IGroup memberGroup in Groups)
                {
                    if (group.UniqueId == memberGroup.UniqueId)
                    {
                        // A group with the same unique id was found.
                        return true;
                    }
                }
                // No groups were found that matched by unique id.
                return false;
            }
            else
            {
                throw new ArgumentNullException(nameof(group));
            }
        }

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
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes);
    }
}
