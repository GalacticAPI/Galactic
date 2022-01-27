﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Galactic.Identity
{
    /// <summary>
    /// IdentityObject is an abstract class that defines base functionality that all identity
    /// object classes (Users, Groups, etc.) implemented by the Galactic API should
    /// support.
    /// </summary>
    public abstract class IdentityObject : IComparable<IdentityObject>, IEquatable<IdentityObject>
    {
        // ----- CONSTANTS -----

        // ----- FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public abstract DateTime? CreationTime { get; }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public abstract List<Group> Groups { get; }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public abstract string UniqueId { get; }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Addes the object to the supplied group.
        /// </summary>
        /// <param name="group">The group to add the object to.</param>
        /// <returns>Thrus if the object was added, false otherwise.</returns>
        public virtual bool AddToGroup(Group group)
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
        public virtual int CompareTo(IdentityObject other)
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
        public virtual int CompareTo<T>(IdentityObject other, string attributeName, string otherAttributeName) where T : IComparable<T>
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
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
        public virtual bool Equals(IdentityObject other)
        {
            if (other != null)
            {
                return UniqueId.Equals(other.UniqueId);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether this object is equal to another using the supplied attribute names as the attributes for checking equality against.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to compare.</typeparam>
        /// <param name="other">The identity object to compare against.</param>
        /// <param name="attributeName">The name of the attribute in this object to use when comparing.</param>
        /// <param name="otherAttributeName">The name of the attribute in the other object to use when comparing.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public virtual bool Equals<T>(IdentityObject other, string attributeName, string otherAttributeName)
        {
            if (other != null && !string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(otherAttributeName))
            {
                // Get the attributes of each object.
                List<IdentityAttribute<object>> attributes = GetAttributes(new() { attributeName });
                List<IdentityAttribute<object>> otherAttributes = other.GetAttributes(new() { attributeName });

                // Verify that attributes were found with the supplied names.
                if (attributes.Count == 1 && otherAttributes.Count == 1)
                {
                    T attribute = (T)attributes[0].Value;
                    T otherAttribute = (T)otherAttributes[0].Value;
                    return attribute.Equals(otherAttribute);
                }
                else
                {
                    // One or more of the attribute names supplied did not exist in the attribute list of the object.
                    return false;
                }
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
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public virtual List<IdentityAttribute<Object>> GetAttributes(List<string> names)
        {
            // Create a list of IdentityAttributes to return.
            List<IdentityAttribute<object>> attributes = new();

            if (names != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(User).GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (DirectorySystemPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<DirectorySystemPropertyNameAttribute>())
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
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public virtual int GetHashCode(IdentityObject obj)
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
        public virtual bool MemberOfGroup(Group group, bool recursive)
        {
            if (group != null)
            {
                // Search all the groups which this object is a member.
                foreach (Group memberGroup in Groups)
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
        public virtual bool RemoveFromGroup(Group group)
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
        public virtual List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes)
        {
            // Create a list of IdentityAttributes to return with success or failure.
            List<IdentityAttribute<bool>> attributeResults = new();

            if (attributes != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = GetType().GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (DirectorySystemPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<DirectorySystemPropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }

                // Iterate over all the attributes supplied, setting their values and marking success or failure in the attribute list to return.
                foreach (IdentityAttribute<object> attribute in attributes)
                {
                    // Check if the attribute supplied matches a property of the object.
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
    }
}