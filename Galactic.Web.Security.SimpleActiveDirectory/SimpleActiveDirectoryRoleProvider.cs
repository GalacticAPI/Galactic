using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.IO;
using System.Web.Hosting;
using System.Web.Security;
using Galactic.ActiveDirectory;
using AD = Galactic.ActiveDirectory.ActiveDirectory;
using Galactic.Configuration;

namespace Galactic.Web.Security.SimpleActiveDirectory
{
    /// <summary>
    /// A role provider for ASP.NET applications that maps application roles to existing Active Directory groups.
    /// Note: This role provider does not implement methods that create or delete roles.
    /// </summary>
    public class SimpleActiveDirectoryRoleProvider : RoleProvider
    {
        // ----- CONSTANTS -----

        // The name of the configuration item specified in the WEB.CONFIG file containing the absolute path to a folder containing configuration items used by the application.
        private const string CONFIG_ITEM_DIRECTORY_NAME = "applicationConfigurationItemDirectory";

        // The name of the configuration item that contains the information required to initialize the role provider.
        private const string SIMPLE_AD_ROLE_PROVIDER_CONFIGURATION_ITEM_NAME = "SimpleActiveDirectoryRoleProvider";

        // ----- VARIABLES -----

        // The name of the application using the role information specified in the configuration file (Web.config).
        private string applicationName = "";

        // The absolute path to the directory contaning configuration items used by the application.
        private string configurationItemDirectoryPath = "";
        
        // The maximum length that a role name may be.
        private int maxRoleNameLength = 0;

        // The name of the role provider, along with its default value.
        private string providerName = "SimpleActiveDirectoryRoleProvider";

        // A connection to active directory that can be used by the role provider.
        private static AD ad;

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

        public SimpleActiveDirectoryRoleProvider()
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
                if (!string.IsNullOrWhiteSpace(config[CONFIG_ITEM_DIRECTORY_NAME]))
                {
                    configurationItemDirectoryPath = config[CONFIG_ITEM_DIRECTORY_NAME];

                    // Get the configuration item with the setup information from a file.
                    ConfigurationItem configItem = new ConfigurationItem(HostingEnvironment.MapPath(configurationItemDirectoryPath), SIMPLE_AD_ROLE_PROVIDER_CONFIGURATION_ITEM_NAME, true);

                    StringReader reader = new StringReader(configItem.Value);

                    // Get the name of the application using this role provider.
                    applicationName = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(applicationName))
                    {
                        throw new ProviderException("The application name was not supplied in the configuration file.");
                    }

                    // Get the name of the configuration item containing connection information for Active Directory.
                    string activeDirectoryConfigurationItemName = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(activeDirectoryConfigurationItemName))
                    {
                        // Setup a connection to active directory for the role provider.
                        ad = new AD(HostingEnvironment.MapPath(configurationItemDirectoryPath), activeDirectoryConfigurationItemName);
                        if (ad == null)
                        {
                            throw new ProviderException("Couldn't connect to Active Directory with configuration provided.");
                        }

                    }
                    else
                    {
                        throw new ProviderException("The Active Directory configuration item name was not supplied in the configuration file.");
                    }

                    // Set the maximum length of role names for this provider.
                    maxRoleNameLength = AD.GROUP_NAME_MAX_CHARS;
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
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        if (username == null)
                        {
                            throw new ArgumentNullException(nameof(usernames));
                        }
                        else
                        {
                            throw new ArgumentException("One of the usernames provided was an empty string.");
                        }
                    }
                }
                foreach (string roleName in roleNames)
                {
                    if (string.IsNullOrWhiteSpace(roleName))
                    {
                        if (roleName == null)
                        {
                            throw new ArgumentNullException(nameof(roleNames));
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
                    // Check whether the roleName exists as the SAMAccountName of a group in Active Directory.
                    Group roleGroup = null;
                    try
                    {
                        roleGroup = new Group(ad, ad.GetGUIDBySAMAccountName(roleName));
                    }
                    catch (ArgumentException)
                    {
                        throw new ArgumentException("The " + roleName + " group does not exist in Active Directory.");
                    }

                    // There is a group corresponding with the roleName provided.
                    foreach (string username in usernames)
                    {
                        // Check that the username is attached to an object in Active Directory.
                        User user = null;
                        try
                        {
                            user = new User(ad, ad.GetGUIDBySAMAccountName(username));
                        }
                        catch (ArgumentException)
                        {
                            throw new ArgumentException("The user named " + username + " does not exist in Active Directory.");
                        }

                        // The account name is attached to an object in Active Directory.
                        if (!user.AddToGroup(roleGroup.GUID))
                        {
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
                    throw new ArgumentNullException(nameof(usernames));
                }
                else
                {
                    throw new ArgumentNullException(nameof(roleNames));
                }
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// Note: This is not implemented for this Role Provider.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            // Takes as input the name of a role and adds the specified role to the data source for the configured ApplicationName.
            // You should throw a ProviderException if the specified role name already exists for the configured ApplicationName.
            // You should throw an ArgumentException if the specified role name is an empty string, contains a comma, or exceeds the
            // maximum length allowed by the data source, and an ArgumentNullException if the specified role name is null (Nothing in Visual Basic).

            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// Note: This is not implemented for this Role Provider.
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
            
            throw new NotImplementedException();
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
            if (!string.IsNullOrWhiteSpace(usernameToMatch))
            {
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    // Get the group from Active Directory.
                    Group group;
                    try
                    {
                        group = new Group(ad, ad.GetGUIDBySAMAccountName(roleName));
                    }
                    catch (ArgumentException)
                    {
                        throw new ProviderException("Unable to locate " + roleName + " group in Active Directory.");
                    }

                    // Create a list of the SAM Account Names of the users in the group.
                    List<string> userNames = new List<string>();
                    foreach (User user in group.UserMembers)
                    {
                        // Check that the user's name contains the usernameToMatch.
                        if (user.SAMAccountName.Contains(usernameToMatch))
                        {
                            // The user name matches, add them to the list.
                            userNames.Add(user.SAMAccountName);
                        }
                    }

                    return userNames.ToArray();
                }
                else
                {
                    throw new ArgumentNullException(nameof(roleName));
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(usernameToMatch));
            }
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            // Get all the groups in Active Directory.
            List<System.DirectoryServices.Protocols.SearchResultEntry> entries = ad.GetEntries("(objectCategory = group)");

            // Check that entries were returned.
            if (entries != null)
            {
                // There are entries in the list.

                // Create a list to hold the names of the roles found.
                List<string> roleNames = new List<string>();

                // Create groups from the entries.
                foreach (System.DirectoryServices.Protocols.SearchResultEntry entry in entries)
                {
                    SecurityPrincipal securityPrincipal = new SecurityPrincipal(ad, entry);
                    roleNames.Add(securityPrincipal.SAMAccountName);
                }

                return roleNames.ToArray();
            }
            else
            {
                // No entries were returned.
                return new string[] { };
            }
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

            if (!string.IsNullOrWhiteSpace(username))
            {
                // The list of roles that the user belongs to.
                List<string> roles = new List<string>();

                // Check whether the user's GUID is found.
                // If there is an error accessing the user's information all role processing will be skipped.
                Guid userGuid = ad.GetGUIDBySAMAccountName(username);
                if (userGuid != Guid.Empty)
                {
                    User user = new User(ad, userGuid);
                    List<string> groupDns = user.Groups;

                    // Get the SAMAccountName of each group the user is a member of and add it as a role.
                    foreach (string groupDn in groupDns)
                    {
                        // Check that the group's entry was found by its distinguished name.
                        // If there is an error accessing the group's information, the group will not be added to the list of roles.
                        System.DirectoryServices.Protocols.SearchResultEntry groupEntry = ad.GetEntryByDistinguishedName(groupDn);
                        if (groupEntry != null)
                        {
                            Group group = new Group(ad, groupEntry);
                            roles.Add(group.SAMAccountName);
                        }
                    }
                }

                // Return the list of roles to which the user belongs.
                return roles.ToArray();
            }
            else
            {
                if (username != null)
                {
                    throw new ArgumentException("username can not be empty.", nameof(username));
                }
                else
                {
                    throw new ArgumentNullException(nameof(username));
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
            if (!string.IsNullOrWhiteSpace(roleName) && !roleName.Contains(',') && !(roleName.Length > MaxRoleNameLength))
            {
                // The roleName is formatted correctly.
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("A role named " + roleName + " does not exist.");
                }

                // Get the list of user names that are members of the role.
                Group group = new Group(ad, ad.GetGUIDBySAMAccountName(roleName));
                List<string> userNames = new List<string>();
                foreach (User user in group.AllUserMembers)
                {
                    userNames.Add(user.SAMAccountName);
                }
                return userNames.ToArray();
            }
            else
            {
                if (roleName != null)
                {
                    throw new ArgumentException("roleName can not be empty, contain a comma, or be longer than " + MaxRoleNameLength + " characters.");
                }
                else
                {
                    throw new ArgumentNullException(nameof(roleName));
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

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(roleName))
            {
                // Check if the supplied role exists.
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("The roleName supplied does not exist.");
                }

                User user;

                // Check if the username supplied exists.
                try
                {
                    user = new User(ad, ad.GetGUIDBySAMAccountName(username));
                }
                catch (ArgumentException)
                {
                    throw new ProviderException("The username supplied does not exist.");
                }

                // Return whether the user is a member of the group corresponding with the roleName supplied.
                return user.MemberOfGroup(ad.GetGUIDBySAMAccountName(roleName), true);
            }
            else
            {
                if (username != null || roleName != null)
                {
                    if (username != null)
                    {
                        throw new ArgumentException("username can not be empty.", nameof(username));
                    }
                    else
                    {
                        throw new ArgumentException("roleName can not be empty.", nameof(roleName));
                    }
                }
                else
                {
                    if (username == null)
                    {
                        throw new ArgumentNullException(nameof(username));
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(roleName));
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
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        if (username != null)
                        {
                            throw new ArgumentException("A username provided was empty.");
                        }
                        else
                        {
                            throw new ArgumentNullException(nameof(usernames));
                        }
                    }
                }

                // Check for empty roleNames.
                foreach (string roleName in roleNames)
                {
                    if (string.IsNullOrWhiteSpace(roleName))
                    {
                        if (roleName != null)
                        {
                            throw new ArgumentException("A roleName provided was empty.");
                        }
                        else
                        {
                            throw new ArgumentNullException(nameof(roleNames));
                        }
                    }
                }

                List<SecurityPrincipal> securityPrincipals = new List<SecurityPrincipal>();

                // Check whether all the usernames exist as users in Active Directory.
                foreach (string username in usernames)
                {
                    try
                    {
                        User user = new User(ad, ad.GetGUIDBySAMAccountName(username));
                        securityPrincipals.Add(user);
                    }
                    catch (ArgumentException)
                    {
                        // The user doesn't exist in Active Directory.
                        throw new ProviderException(username + " is not a user in Active Directory.");
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
                    try
                    {
                        Group roleGroup = new Group(ad, ad.GetGUIDBySAMAccountName(roleName));
                        if (!roleGroup.RemoveMembers(securityPrincipals))
                        {
                            throw new ProviderException("Can't remove users from " + roleName + ".");
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        // Active Directory could not be reached.
                        throw new ProviderException("Can't remove users from " + roleName + ".");
                    }
                }
            }
            else
            {
                if (usernames == null)
                {
                    throw new ArgumentNullException(nameof(usernames));
                }
                else
                {
                    throw new ArgumentNullException(nameof(roleNames));
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

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // Check if a group representing the role, exists in Active Directory.
                try
                {
                    Group group = new Group(ad, ad.GetGUIDBySAMAccountName(roleName));

                    // The group exists.
                    return true;
                }
                catch
                {
                    // The group does not exist.
                    return false;
                }
            }
            else
            {
                if (roleName != null)
                {
                    throw new ArgumentException("roleName can not be empty.");
                }
                else
                {
                    throw new ArgumentNullException(nameof(roleName));
                }
            }
        }
    }
}
