using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// Directory System is an interface that defines functionality that all
    /// directory system classes implemented by the Galactic API should support.
    /// </summary>
    public interface IDirectorySystem
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC VARIABLES -----

        // ----- PROPERTIES -----

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
        /// </summary>
        /// <param name="name">The proposed name of the group.</param>
        /// <param name="type">The type of group to create.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Optional) Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object, or null if it could not be created.</returns>
        public IGroup CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null);

        /// <summary>
        /// Creates a user within the directory system given it's login, and other options attributes.
        /// </summary>
        /// <param name="login">The proposed login of the user.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
        /// <returns>The newly creaTed user object, or null if it could not be created.</returns>
        public IUser CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null);

        /// <summary>
        /// Deletes an object with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to delete.</param>
        /// <returns>True if the object was deleted, false otherwise.</returns>
        public bool DeleteObject(string uniqueId);

        /// <summary>
        /// Get's all users in the directory system.
        /// </summary>
        /// <returns>A list of all users in the directory system.</returns>
        public List<IUser> GetAllUsers();

        /// <summary>
        /// Gets a list of the types of groups supported by the directory system.
        /// </summary>
        /// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
        public List<string> GetGroupTypes();

        /// <summary>
        /// Gets identity objects that match wildcarded (*) attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Optional) The attributes that should be returned in the object found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of idenity objects that match the attribute value supplied.</returns>
        public List<IIdentityObject> GetObjectsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null);

        /// <summary>
        /// Moves an object in the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to move.</param>
        /// <param name="parentUniqueId">The unique id of the object that will be the new parent of the object.</param>
        /// <returns>True if the object was moved, false otherwise.</returns>
        public bool MoveObject(string uniqueId, string parentUniqueId);

        /// <summary>
        /// Renames an object in the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to rename.</param>
        /// <param name="name"The new name of the object.</param>
        /// <returns>True if the object was renamed, false otherwise.</returns>
        public bool RenameObject(string uniqueId, string name);
    }
}
