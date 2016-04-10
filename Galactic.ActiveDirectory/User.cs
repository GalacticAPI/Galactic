using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Text;

namespace Galactic.ActiveDirectory
{
    /// <summary>
    /// User is a class that allows for the query and manipulation of
    /// Active Directory user objects.
    /// </summary>
    public class User : SecurityPrincipal, IComparable<User>, IEqualityComparer<User>
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The list of specific attributes that should be retrieved when searching for the entry in AD. The attributes of parent objects should be included as well.
        /// </summary>
        static protected new string[] AttributeNames = { "department" };

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The number of times the user has entered a bad password.
        /// Returns a negative number is there is an error retrieving the value.
        /// </summary>
        public int BadPasswordCount
        {
            get
            {
                try
                {
                    return Int32.Parse(GetStringAttributeValue("badPwdCount"));
                }
                catch
                {
                    // There was an error parsing the value.
                    return -1;
                }
            }
        }

        /// <summary>
        /// The last time that the user entered a bad password when authenticating to AD.
        /// </summary>
        public DateTime? BadPasswordTime
        {
            get
            {
                DateTime? returnValue = GetIntervalAttributeValue("badPasswordTime");
                if (returnValue.HasValue)
                {
                    return returnValue.Value.ToLocalTime();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The Department the user belongs to.
        /// </summary>
        public string Department
        {
            get
            {
                return GetStringAttributeValue("department");
            }
            set
            {
                SetStringAttribute("department", value);
            }
        }

        /// <summary>
        /// The employee number of the user.
        /// </summary>
        public string EmployeeNumber
        {
            get
            {
                return GetStringAttributeValue("employeeNumber");
            }
            set
            {
                SetStringAttribute("employeeNumber", value);
            }
        }

        /// <summary>
        /// Whether the user's account is disabled in Active Directory.
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return ActiveDirectory.UserAccountControlContains(UserAccountControl, ActiveDirectory.UserAccountControl.Accountdisable);
            }
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return GetStringAttributeValue("displayName");
            }
            set
            {
                SetStringAttribute("displayName", value);
            }
        }

        /// <summary>
        /// The user's division in the organization.
        /// </summary>
        public string Division
        {
            get
            {
                return GetStringAttributeValue("division");
            }
            set
            {
                SetStringAttribute("division", value);
            }
        }

        /// <summary>
        /// The Employee Id of the user.
        /// </summary>
        public string EmployeeId
        {
            get
            {
                return GetStringAttributeValue("employeeID");
            }
            set
            {
                SetStringAttribute("employeeID", value);
            }
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return GetStringAttributeValue("givenName");
            }
            set
            {
                SetStringAttribute("givenName", value);
            }
        }

        /// <summary>
        /// The FTP Directory for the user off the FTP root.
        /// </summary>
        public string FTPDirectory
        {
            get
            {
                return GetStringAttributeValue("msIIS-FTPDir");
            }
            set
            {
                SetStringAttribute("msIIS-FTPDir", value);
            }
        }

        /// <summary>
        /// The root directory for FTP access by the user.
        /// </summary>
        public string FTPRoot
        {
            get
            {
                return GetStringAttributeValue("msIIS-FTPRoot");
            }
            set
            {
                SetStringAttribute("msIIS-FTPRoot", value);
            }
        }

        /// <summary>
        /// The path to the user's home directory.
        /// </summary>
        public string HomeDirectory
        {
            get
            {
                return GetStringAttributeValue("homeDirectory");
            }
            set
            {
                SetStringAttribute("homeDirectory", value);
            }
        }

        /// <summary>
        /// The user's home drive letter.
        /// </summary>
        public string HomeDrive
        {
            get
            {
                return GetStringAttributeValue("homeDrive");
            }
            set
            {
                SetStringAttribute("homeDrive", value);
            }
        }

        /// <summary>
        /// The user's home page URL.
        /// </summary>
        public string HomePage
        {
            get
            {
                return GetStringAttributeValue("wWWHomePage");
            }
            set
            {
                SetStringAttribute("wWWHomePage", value);
            }
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return GetStringAttributeValue("sn");
            }
            set
            {
                SetStringAttribute("sn", value);
            }
        }

        /// <summary>
        /// The path to the user's logon script.
        /// </summary>
        public string LogonScript
        {
            get
            {
                return GetStringAttributeValue("scriptPath");
            }
            set
            {
                SetStringAttribute("scriptPath", value);
            }
        }

        /// <summary>
        /// The distinguished name of the user's Manager.
        /// </summary>
        public string Manager
        {
            get
            {
                return GetStringAttributeValue("manager");
            }
            set
            {
                SetStringAttribute("manager", value);
            }
        }

        /// <summary>
        /// Whether the user has to change their password at their next logon.
        /// </summary>
        public bool MustChangePasswordAtNextLogon
        {
            get
            {
                DateTime? passwordLastSetTime = GetIntervalAttributeValue("pwdLastSet");
                if (passwordLastSetTime.HasValue)
                {
                    if (passwordLastSetTime.Value == JAN_01_1601 && !ActiveDirectory.UserAccountControlContains(UserAccountControl, ActiveDirectory.UserAccountControl.DontExpirePassword))
                    {
                        // The password last set time is not set, and the don't expire password bit in the UserAccountControl attribute isn't set.
                        // The user must change their password at next logon.
                        return true;
                    }
                }
                return false;
            }
            set
            {
                if (value)
                {
                    SetAttribute("pwdLastSet", new object[] { BitConverter.GetBytes(0) });
                }
                else
                {
                    SetAttribute("pwdLastSet", new object[] { BitConverter.GetBytes(-1) });
                }
            }
        }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public bool PasswordExpired
        {
            get
            {
                return ActiveDirectory.UserAccountControlContains(UserAccountControlComputed, ActiveDirectory.UserAccountControl.PasswordExpired);
            }
        }

        /// <summary>
        /// The date and time that the user's password was last site.
        /// </summary>
        public DateTime? PasswordLastSet
        {
            get
            {
                DateTime? returnValue = GetIntervalAttributeValue("pwdLastSet");
                if (returnValue.HasValue)
                {
                    return returnValue.Value.ToLocalTime();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The user's telephone number.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return GetStringAttributeValue("telephoneNumber");
            }
            set
            {
                SetStringAttribute("telephoneNumber", value);
            }
        }

        /// <summary>
        /// The user's security identifier (SID).
        /// </summary>
        public byte[] SecurityIdentifier
        {
            get
            {
                return GetByteAttributeValue("objectSid");
            }
        }

        /// <summary>
        /// The user's street address.
        /// </summary>
        public string StreetAddress
        {
            get
            {
                return GetStringAttributeValue("streetAddress");
            }
            set
            {
                SetStringAttribute("streetAddress", value);
            }
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        public string Title
        {
            get
            {
                return GetStringAttributeValue("title");
            }
            set
            {
                SetStringAttribute("title", value);
            }
        }

        /// <summary>
        /// Gets the user's UserAccountControl property.
        /// A return of 0 means there was an error retrieving the property.
        /// </summary>
        public uint UserAccountControl
        {
            get
            {
                try
                {
                    return UInt32.Parse(GetStringAttributeValue("userAccountControl"));
                }
                catch
                {
                    // There was an error converting the string to a UInt.
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the user's computed UserAccountControl property.
        /// A return value of 0 means it wasn't computed.
        /// </summary>
        public uint UserAccountControlComputed
        {
            get
            {
                try
                {
                    return UInt32.Parse(GetStringAttributeValue("msDS-User-Account-Control-Computed"));
                }
                catch
                {
                    // There was an error converting the string to a UInt.
                    return 0;
                }
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a user object from Active Directory with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory object used to query and manipulate the user.</param>
        /// <param name="guid">The GUID of the user.</param>
        public User(ActiveDirectory ad, Guid guid)
            : base(ad, guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                // Add atttributes relevant to users to the list of base attributes
                // to retrieve from entries queried from Active Directory.
                Attributes.AddRange(AttributeNames);

                Entry = GetEntryFromAD(guid);
                if (Entry == null)
                {
                    throw new ArgumentException("The GUID provided could not be found in active directory.", "guid");
                }
            }
            else
            {
                if (ad == null)
                {
                    throw new ArgumentNullException("ad");
                }
                if (guid == Guid.Empty)
                {
                    throw new ArgumentException("guid");
                }
            }
        }

        /// <summary>
        /// Gets a user object from a supplied search result entry.
        /// </summary>
        /// <param name="ad">An Active Directory object used to manipulate the user.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public User(ActiveDirectory ad, SearchResultEntry entry)
            : base(ad, entry)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Creates a new user within Active Directory given it's proposed name, the distinguished name of the OU to place it in, and other optional attributes.
        /// </summary>
        /// <param name="ad">An Active Directory object used to create the user.</param>
        /// <param name="sAMAccountName">The proposed SAM Account name for the user.</param>
        /// <param name="ouDn">The distinguished name for the OU to place the user within.</param>
        /// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
        /// <returns>The newly created user object.</returns>
        static public User Create(ActiveDirectory ad, string sAMAccountName, string ouDn, List<DirectoryAttribute> additionalAttributes = null)
        {
            // Check that an active directory instance, SAM Account name, and distinguished name of the OU to
            // place the user within are provided.
            if (ad != null && !String.IsNullOrWhiteSpace(sAMAccountName) && !String.IsNullOrWhiteSpace(ouDn))
            {
                // Check that the OU exists in Active Directory.
                if (ad.GetEntryByDistinguishedName(ouDn) == null)
                {
                    // The OU does not exist in Active Directory.
                    throw new ArgumentException("The OU provided does not exist in Active Directory.");
                }

                // Create the user in Active Directory and return its object.
                string userDn = "CN=" + sAMAccountName + "," + ouDn;
                List<DirectoryAttribute> attributes = new List<DirectoryAttribute>
                {
                    new DirectoryAttribute("sAMAccountName", sAMAccountName)
                };
                if (additionalAttributes != null)
                {
                    // Only add non conflicting attributes.
                    foreach (DirectoryAttribute attribute in additionalAttributes)
                    {
                        if (attribute.Name != "sAMAccountName")
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
                if (ad.Add(userDn, attributes.ToArray()))
                {
                    // The user was created. Retrieve it from Active Directory.
                    return new User(ad, ad.GetGUIDByDistinguishedName(userDn));
                }
                else
                {
                    // There was a problem creating the user in Active Directory.
                    return null;
                }
            }
            else
            {
                if (ad == null)
                {
                    throw new ArgumentNullException("ad");
                }
                else
                {
                    throw new ArgumentNullException("sAMAccountName");
                }
            }
        }

        /// <summary>
        /// Disables the user's account for authentication if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public bool Disable()
        {
            return SetUserAccountControlFlag(ActiveDirectory.UserAccountControl.Accountdisable);
        }

        /// <summary>
        /// Enables the user's account for authentication if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public bool Enable()
        {
            return RemoveUserAccountControlFlag(ActiveDirectory.UserAccountControl.Accountdisable);
        }

        /// <summary>
        /// Gets all users in the Active Directory.
        /// </summary>
        /// <param name="ad">The Active Directory to retrieve users from.</param>
        /// <returns>A list of all users in the Active Directory.</returns>
        static public List<User> GetAllUsers(ActiveDirectory ad)
        {
            // The LDAP search filter to use to find all the users.
            const string FILTER = "(&(objectCategory=person)(objectClass=user))";

            if (ad != null)
            {
                // Get the GUIDs of all the users in AD.

                // Create a list of attributes that should be retrieved with the query.
                List<string> attributes = new List<string>();
                attributes.AddRange(ActiveDirectoryObject.AttributeNames);
                attributes.AddRange(SecurityPrincipal.AttributeNames);
                attributes.AddRange(AttributeNames);

                // Search for the users in AD.
                List<SearchResultEntry> entries = ad.GetEntries(FILTER, attributes);

                // Check whether the search returned results.
                if (entries != null)
                {
                    // The search returned results.
                    // Get user objects from the entries retrieved.
                    List<User> users = new List<User>();
                    foreach (SearchResultEntry entry in entries)
                    {
                        users.Add(new User(ad, entry));
                    }
                    return users;
                }
            }
            return new List<User>();
        }

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public bool SetPassword(string password)
        {
            // Create a UTF16 array of bytes from the supplied password.
            byte[] bytes = Encoding.Unicode.GetBytes("\"" + password + "\"");
            return SetAttribute("unicodePwd", new object[] { bytes });
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public bool Unlock()
        {
            // Setting a user's lockout time to 0 unlocks the account.
            return SetAttribute("lockoutTime", new object[] { BitConverter.GetBytes(0) });
        }

        /// <summary>
        /// Sets a flag in the user's User Account Control attribute.
        /// </summary>
        /// <param name="flag">A flag from the predefined UserAccountControl flags.</param>
        /// <returns>True if set, false otherwise.</returns>
        public bool SetUserAccountControlFlag(ActiveDirectory.UserAccountControl flag)
        {
            // Use the normal user account control attribute.
            uint newUserAccountControl = UserAccountControl | (uint)flag;
            if (SetStringAttribute("userAccountControl", newUserAccountControl.ToString()))
            {
                // Remove the userAccountControl attribute from the list so it is refreshed on the next request for it.
                AdditionalAttributes.Remove("userAccountControl");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a flag from the user's User Account Control attribute.
        /// </summary>
        /// <param name="flag">A flag from the predefined UserAccountControl flags.</param>
        /// <returns>True if removed, false otherwise.</returns>
        public bool RemoveUserAccountControlFlag(ActiveDirectory.UserAccountControl flag)
        {
            // Use the normal user account control attribute.
            uint newUserAccountControl = UserAccountControl & ~(uint)flag;
            if (SetStringAttribute("userAccountControl", newUserAccountControl.ToString()))
            {
                // Remove the userAccountControl attribute from the list so it is refreshed on the next request for it.
                AdditionalAttributes.Remove("userAccountControl");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first User to check.</param>
        /// <param name="y">The second User to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(User x, User y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Generates a hash code for the User supplied.
        /// </summary>
        /// <param name="obj">The User to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(User obj)
        {
            return GetHashCode((ActiveDirectoryObject)obj);
        }

        /// <summary>
        /// Compares this User to another User.
        /// </summary>
        /// <param name="other">The other User to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(User other)
        {
            return CompareTo((ActiveDirectoryObject)other);
        }
    }
}
