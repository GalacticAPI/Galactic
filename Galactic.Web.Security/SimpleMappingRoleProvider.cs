using Galactic.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using AD = Galactic.ActiveDirectory.ActiveDirectory;
using Galactic.Configuration;

namespace Galactic.Web.Security
{
    /// <summary>
    /// A role provider for ASP.NET applications that maps users to roles in a configuration item file.
    /// </summary>
    public class SimpleMappingRoleProvider : RoleProvider
    {
        // ----- CONSTANTS -----

        // The name of the configuration item specified in the WEB.CONFIG file containing the absolute path to a folder containing configuration items used by the application.
        private const string CONFIG_ITEM_DIRECTORY_NAME = "applicationConfigurationItemDirectory";

        // The name of the configuration item that contains the information required to initialize the role provider.
        private const string SIMPLE_MAPPING_ROLE_PROVIDER_CONFIGURATION_ITEM_NAME = "SimpleMappingRoleProvider";
        
        // ----- VARIABLES -----

        // The name of the application using the role information specified in the configuration file (Web.config).
        private string applicationName = "";

        // The absolute path to the directory contaning configuration items used by the application.
        private string configurationItemDirectoryPath = "";

        // The maximum length that a role name may be.
        private const int maxRoleNameLength = 60;

        // The name of the role provider, along with its default value.
        private string providerName = "SimpleMappingRoleProvider";

        // The configuration item used by this role provider.
        private static ConfigurationItem configItem = null;

        // A dictionary keyed on role name, containing a list of the users in each role.
        private static Dictionary<string, List<string>> roles = new Dictionary<string, List<string>>();

        // ----- PROPERTIES -----

        public override string Name
        {
            get
            {
                return providerName;
            }
        }

        /// <summary>
        /// The name of the application using the role information specified in the configuration file (Web.config).
        /// The ApplicationName is stored in the data source with related user information and used when querying for user information.
        /// This property is read-write and defaults to the ApplicationPath if not specified explicitly.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        /// <summary>
        /// The maximum number of characters that a role name may contain.
        /// </summary>
        public int MaxRoleNameLength
        {
            get
            {
                return maxRoleNameLength;
            }
        }

        // ----- CONSTRUCTORS -----

        public SimpleMappingRoleProvider()
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Takes as input the name of the provider and a NameValueCollection of configuration settings.
        /// Used to set property values for the provider instance including implementation-specific values
        /// and options specified in the configuration file (Machine.config or Web.config).
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        /// <param name="config">Collection of configuration settings.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            providerName = name;

            // Get the location of the configuration items used by the application.
            try
            {
                if (!String.IsNullOrWhiteSpace(config[CONFIG_ITEM_DIRECTORY_NAME]))
                {
                    configurationItemDirectoryPath = config[CONFIG_ITEM_DIRECTORY_NAME];

                    // Get the configuration item with the setup information from a file.
                    configItem = new ConfigurationItem(HostingEnvironment.MapPath(configurationItemDirectoryPath), SIMPLE_MAPPING_ROLE_PROVIDER_CONFIGURATION_ITEM_NAME, true);

                    // Read the roles from the configuration item.
                    roles = ReadRoles(new StringReader(configItem.Value));
                    
                    if (roles == null)
                    {
                        throw new ProviderException("Couldn't read roles from configuration item.");
                    }
                }
                else
                {
                    throw new ProviderException("The configuration items folder specified for use by the role provider could not be found in the supplied configuration data.");
                } 
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ProviderException("The configuration items folder specified for use by the role provider could not be found in the supplied configuration data.");
            }
        }

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames != null && roleNames != null)
            {
                // Takes as input a list of user names and a list of role names, and associates the specified users with the specified roles at the data source for the configured ApplicationName.
                // You should throw a ProviderException if any of the role names or user names specified do not exist for the configured ApplicationName.
                // You should throw an ArgumentException if any of the specified user names or role names is an empty string and an ArgumentNullException if any of the specified user names or role
                // names is null (Nothing in Visual Basic).
                // If your data source supports transactions, you should include each add operation in a transaction and roll back the transaction and throw an exception if any add operation fails.

                // Check that none of the usernames or roleNames are empty or null.
                foreach (string username in usernames)
                {
                    if (String.IsNullOrWhiteSpace(username))
                    {
                        if (username == null)
                        {
                            throw new ArgumentNullException("usernames");
                        }
                        else
                        {
                            throw new ArgumentException("One of the usernames provided was an empty string.");
                        }
                    }
                }
                foreach (string roleName in roleNames)
                {
                    if (String.IsNullOrWhiteSpace(roleName))
                    {
                        if (roleName == null)
                        {
                            throw new ArgumentNullException("roleNames");
                        }
                        else
                        {
                            throw new ArgumentException("One of the roleNames provided was an empty string.");
                        }
                    }
                }

                // Add the usernames to the specified roles.
                foreach (string roleName in roleNames)
                {
                    // Check whether the roleName exists.
                    if (!roles.ContainsKey(roleName))
                    {
                        throw new ArgumentException("The " + roleName + " role does not exist.");
                    }

                    // There is a role corresponding with the roleName provided.
                    foreach (string username in usernames)
                    {
                        // Add the user to the role.
                        if (!roles[roleName].Contains(username))
                        {
                            roles[roleName].Add(username);
                        }

                        // Save the updated role to the configuration item.
                        if (!SaveRoles())
                        {
                            // Remove the user added to the dictionary.
                            roles[roleName].Remove(username);

                            // The user was not added to the group.
                            throw new ProviderException("Unable to add " + username + " to " + roleName + " group in Active Directory.");
                        }
                    }
                }
            }
            else
            {
                if (usernames == null)
                {
                    throw new ArgumentNullException("usernames");
                }
                else
                {
                    throw new ArgumentNullException("roleNames");
                }
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            // Takes as input the name of a role and adds the specified role to the data source for the configured ApplicationName.
            // You should throw a ProviderException if the specified role name already exists for the configured ApplicationName.
            // You should throw an ArgumentException if the specified role name is an empty string, contains a comma, or exceeds the
            // maximum length allowed by the data source, and an ArgumentNullException if the specified role name is null (Nothing in Visual Basic).

            if (!String.IsNullOrWhiteSpace(roleName) && !roleName.Contains(',') && roleName.Length <= MaxRoleNameLength)
            {   
                // Check whether the role already exists.
                if (roles.ContainsKey(roleName))
                {
                    throw new ProviderException("Role already exists.");
                }

                // Create a new role.
                roles.Add(roleName, new List<string>());
                if (!SaveRoles())
                {
                    roles.Remove(roleName);
                    throw new ProviderException("Unable to create new role.");
                }

            }
            else
            {
                throw new ArgumentNullException("roleName");
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">A boolean indicating whether an exception should be thrown if there are still users contained in the role.</param>
        /// <returns>True if the role was deleted, false otherwise.</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            // Takes as input the name of a role and a Boolean value that indicates whether to throw an exception if there are still users associated
            // with the role. The DeleteRole deletes the specified role from the data source for the configured ApplicationName.
            // If the throwOnPopulatedRole parameter is true, and the role identified by the role name parameter has one or more members, throw a
            // ProviderException and do not delete the role. If the throwOnPopulatedRole parameter is false, then delete the role whether it is empty or not.
            // When you delete a role from the data source, ensure that you also delete any associations between a user name and the deleted role for the configured ApplicationName.
            // You should throw an ArgumentException if the specified role name does not exist, or is an empty string. You should throw an ArgumentNullException
            // if the specified role name is null (Nothing in Visual Basic).

            if (roleName != null)
            {
                if (!string.IsNullOrWhiteSpace(roleName) && roles.ContainsKey(roleName))
                {
                    if (throwOnPopulatedRole)
                    {
                        // Check whether the group is empty.
                        if (roles[roleName].Count > 0)
                        {
                            // The group isn't empty. Throw an exception.
                            throw new ProviderException("Role not empty.");
                        }
                    }

                    // Save the list of members in case the role does not delete correctly.
                    List<string> members = roles[roleName];

                    // Delete the role.
                    roles.Remove(roleName);
                    if (SaveRoles())
                    {
                        return true;
                    }
                    else
                    {
                        // Restore the role that didn't delete correctly.
                        roles.Add(roleName, members);
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException("roleName");
                }
            }
            else
            {
                throw new ArgumentNullException("roleName");
            }
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The username to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            // Takes a role name and a string value and returns a collection of user names in the role that contain the provided string value.
            // Wildcard support is included based on the data source. Users are returned in alphabetical order by user name.
            // It is recommended that you throw a ProviderException if the role name specified does not exist in the data source.
            if (!String.IsNullOrWhiteSpace(usernameToMatch))
            {
                if (!String.IsNullOrWhiteSpace(roleName))
                {
                    if (roles.ContainsKey(roleName))
                    {
                        if (usernameToMatch.EndsWith("*"))
                        {
                            // Find all users that start with characters up to the wildcard.
                            List<string> usernames = (List<string>)roles[roleName].Where(userNameToMatch => usernameToMatch.StartsWith(usernameToMatch.Substring(0, usernameToMatch.Length - 1)));
                            
                            // Sort the usernames.
                            usernames.Sort(StringComparer.OrdinalIgnoreCase);

                            return usernames.ToArray();
                        }
                        else
                        {
                            // Find a single user.
                            if (roles[roleName].Contains(usernameToMatch))
                            {
                                return new string[] { usernameToMatch };
                            }
                            else
                            {
                                return new string[] { };
                            }
                        }
                    }
                    else
                    {
                        throw new ProviderException("Role does not exist.");
                    }
                }
                else
                {
                    throw new ArgumentNullException("roleName");
                }
            }
            else
            {
                throw new ArgumentNullException("usernameToMatch");
            }
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            return roles.Keys.ToArray();
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName. 
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            // Takes as input a user name and returns the role names that the specified user is associated with, from the data source. Only the roles for the configured ApplicationName are retrieved.
            // If no roles exist for the specified user for the configured ApplicationName, you should return a string array with no elements.
            // You should throw an ArgumentException if the specified user name is an empty string. You should throw an ArgumentNullException if the specified user name is null (Nothing in Visual Basic).

            if (!String.IsNullOrWhiteSpace(username))
            {
                string[] roleNames = GetAllRoles();

                // The list of roles that the user belongs to.
                List<String> roles = new List<String>();

                // Check whether the user is a member of each role.
                foreach (string role in roleNames)
                {
                    if (IsUserInRole(username, role))
                    {
                        roles.Add(role);
                    }
                }

                // Return the list of roles to which the user belongs.
                return roles.ToArray();
            }
            else
            {
                if (username != null)
                {
                    throw new ArgumentException("username can not be empty.", "username");
                }
                else
                {
                    throw new ArgumentNullException("username");
                }
            }
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the specified role for the configured applicationName.</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            // Takes as input a role name and returns the user names associated with a role from the data source. Only the roles for the configured ApplicationName are retrieved.
            // If the specified role name does not exist for the configured ApplicationName, you should throw a ProviderException.
            // If no users are associated with the specified role for the configured ApplicationName, you should return a string array with no elements.
            // You should throw an ArgumentException if the specified role name is an empty string, contains a comma, or exceeds the maximum length for a role name allowed by your data source.
            // You should throw an ArgumentNullException if the specified role name is null (Nothing in Visual Basic).
            if (!String.IsNullOrWhiteSpace(roleName) && !roleName.Contains(',') && !(roleName.Length > MaxRoleNameLength))
            {
                // The roleName is formatted correctly.
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("A role named " + roleName + " does not exist.");
                }

                // Get the list of user names that are members of the role.
                return roles[roleName].ToArray();
            }
            else
            {
                if (roleName != null)
                {
                    throw new ArgumentException("roleName can not be empty, contain a comma, or be longer than " + MaxRoleNameLength + " characters.");
                }
                else
                {
                    throw new ArgumentNullException("roleName");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the configured applicationName; otherwise, false.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            // Takes as input a user name and a role name and determines whether the specified user is associated with a role from the data source for the configured ApplicationName.
            // You should throw a ProviderException if the role name or user name specified does not exist for the configured ApplicationName.
            // You should throw an ArgumentException if the specified user name or role name is an empty string and an ArgumentNullException if the specified user name or role name
            // is null (Nothing in Visual Basic).

            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(roleName))
            {
                // Check if the supplied role exists.
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("The roleName supplied does not exist.");
                }

                // Check if the supplied username exists.
                if (!UserExists(username))
                {
                    throw new ProviderException("The username supplied does not exist.");
                }

                // Return whether the user is a member of the role.
                return roles[roleName].Contains(username);
            }
            else
            {
                if (username != null || roleName != null)
                {
                    if (username != null)
                    {
                        throw new ArgumentException("username can not be empty.", "username");
                    }
                    else
                    {
                        throw new ArgumentException("roleName can not be empty.", "roleName");
                    }
                }
                else
                {
                    if (username == null)
                    {
                        throw new ArgumentNullException("username");
                    }
                    else
                    {
                        throw new ArgumentNullException("roleName");
                    }
                }
            }
        }

        // Parses roles from the provided string reader.
        // Returns a dictionary keyed on the role name containing a list of users in each role, or null if there was an error reading the string.
        private static Dictionary<string, List<string>> ReadRoles(StringReader reader)
        {
            if (reader != null)
            {
                try
                {
                    // A dictionary keyed on role name that will contain a list of users in each role.
                    Dictionary<string, List<string>> roles = new Dictionary<string, List<string>>();

                    // Parse each line of the string.
                    string line = reader.ReadLine();
                    string roleName = "";
                    while (!string.IsNullOrEmpty(line))
                    {
                        line = line.Trim();
                        if (line.EndsWith("["))
                        {
                            // This is a role definition.
                            roleName = line.Split('=')[0].Trim();

                            // Create a new entry with the role's name in the dictionary.
                            roles.Add(roleName, new List<string>());
                        }
                        if (line.EndsWith("]"))
                        {
                            // This is the end of a role definition.
                            roleName = "";
                        }
                        // Check that a role has been defined on a previous line.
                        if (!string.IsNullOrWhiteSpace(roleName))
                        {
                            // A role has been defined.
                            // Add the username on this line to the currently defined role.
                            roles[roleName].Add(line);
                        }
                        line = reader.ReadLine();
                    }

                    // Return the roles found.
                    return roles;
                }
                catch
                {
                    // There was an error reading the string.
                    return null;
                }
            }
            else
            {
                // No reader was provided.
                return null;
            }
        }

        /// <summary>
        /// Removes a user from a role.
        /// </summary>
        /// <param name="username">The name of the user to remove from the role.</param>
        /// <param name="roleName">The name of the role to remove the user from.</param>
        /// <returns>True if the user was removed. False, otherwise.</returns>
        public bool RemoveUser(string username, string roleName)
        {
            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(roleName))
            {
                // Check if the supplied role exists.
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("The roleName supplied does not exist.");
                }

                // Remove the user from the role.
                roles[roleName].Remove(username);
                if (SaveRoles())
                {
                    return true;
                }
                else
                {
                    // The roles couldn't be saved.
                    // Restore the user to the role and return false.
                    roles[roleName].Add(username);
                    return false;
                }
            }
            else
            {
                if (username != null || roleName != null)
                {
                    if (username != null)
                    {
                        throw new ArgumentException("username can not be empty.", "username");
                    }
                    else
                    {
                        throw new ArgumentException("roleName can not be empty.", "roleName");
                    }
                }
                else
                {
                    if (username == null)
                    {
                        throw new ArgumentNullException("username");
                    }
                    else
                    {
                        throw new ArgumentNullException("roleName");
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // Takes as input a list of user names and a list of role names and removes the association for the specified users from the specified roles at the data source for the configured ApplicationName.
            // You should throw a ProviderException if any of the role names or user names specified does not exist for the configured ApplicationName.
            // You should throw an ArgumentException if any of the specified user names or role names is an empty string and an ArgumentNullException if any of the specified user names or role names is null (Nothing in Visual Basic).
            // If your data source supports transactions, you should include each remove operation in a transaction and roll back the transaction and throw an exception if any remove operation fails.

            if (usernames != null && roleNames != null)
            {
                // Check for empty usernames.
                foreach (string username in usernames)
                {
                    if (String.IsNullOrWhiteSpace(username))
                    {
                        if (username != null)
                        {
                            throw new ArgumentException("A username provided was empty.");
                        }
                        else
                        {
                            throw new ArgumentNullException("usernames");
                        }
                    }
                }

                // Check for empty roleNames.
                foreach (string roleName in roleNames)
                {
                    if (String.IsNullOrWhiteSpace(roleName))
                    {
                        if (roleName != null)
                        {
                            throw new ArgumentException("A roleName provided was empty.");
                        }
                        else
                        {
                            throw new ArgumentNullException("roleNames");
                        }
                    }
                }

                // Check wheterh all usernames exist in the application.
                foreach (string username in usernames)
                {
                    if (!UserExists(username))
                    {
                        // The user doesn't exist.
                        throw new ProviderException(username + " doesn't exist.");
                    }
                }

                // Check whether all roles exist.
                foreach (string roleName in roleNames)
                {
                    if (!RoleExists(roleName))
                    {
                        // The role doesn't exist.
                        throw new ProviderException(roleName + " doesn't exist.");
                    }
                }

                // Remove all users from the specified roles.
                foreach (string roleName in roleNames)
                {
                    foreach (string username in usernames)
                    {
                        if (!RemoveUser(username, roleName))
                        {
                            throw new ProviderException("Can't remove users from " + roleName + ".");
                        }
                    }
                }
            }
            else
            {
                if (usernames == null)
                {
                    throw new ArgumentNullException("usernames");
                }
                else
                {
                    throw new ArgumentNullException("roleNames");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the configured applicationName; otherwise, false.</returns>
        public override bool RoleExists(string roleName)
        {
            // Takes as input a role name and determines whether the role name exists in the data source for the configured ApplicationName.
            // You should throw an ArgumentException if the specified role name is an empty string. It is recommended that you throw an ArgumentNullException
            // if the specified role name is null (Nothing in Visual Basic).

            if (!String.IsNullOrWhiteSpace(roleName))
            {
                return roles.ContainsKey(roleName);
            }
            else
            {
                if (roleName != null)
                {
                    throw new ArgumentException("roleName can not be empty.");
                }
                else
                {
                    throw new ArgumentNullException("roleName");
                }
            }
        }

        // Saves the currently defined roles and membership to the supplied configuration item.
        private static bool SaveRoles()
        {
            if (configItem != null)
            {
                if (roles != null)
                {
                    try
                    {
                        // Build a string from the role data in the application.
                        StringBuilder builder = new StringBuilder();
                        foreach (string roleName in roles.Keys)
                        {
                            builder.Append(roleName + "=[\n");
                            foreach (string memberName in roles[roleName])
                            {
                                builder.Append(memberName + "\n");
                            }
                            builder.Append("]\n");
                        }
                        // Save the role data to the configuration item.
                        configItem.Value = builder.ToString();

                        // The role data was successfully saved.
                        return true;
                    }
                    catch
                    {
                        // There was an error saving the role data to the configuration item.
                        return false;
                    }
                }
            }
            // The configuration item or roles dictionary for this provider is not defined.
            return false;
        }

        // Determines whether the user exists within the application (is assigned to any role).
        private bool UserExists(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                bool exists = false;
                foreach (string role in GetAllRoles())
                {
                    if (roles[role].Contains(username))
                    {
                        exists = true;
                        break;
                    }
                }
                return exists;
            }
            else
            {
                // A username was not supplied.
                return false;
            }
        }
    }
}
