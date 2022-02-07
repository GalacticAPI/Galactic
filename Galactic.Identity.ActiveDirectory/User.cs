using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace Galactic.Identity.ActiveDirectory
{
    /// <summary>
    /// User is a class that allows for the query and manipulation of
    /// Active Directory user objects.
    /// </summary>
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class User : Identity.User, ISecurityPrincipal
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The default location for users to be created in Active Directory.
        /// </summary>
        public const string DEFAULT_CREATE_PATH = "CN=Users";

        // ----- VARIABLES -----

        // The client used to query and manipulate Active Directory.
        protected ActiveDirectoryClient ad = null;

        // The list of attributes to retrieve when searching for the entry in AD.
        protected List<string> attributes = new List<string>(attributeNames);

        /// <summary>
        /// The list of specific attributes that should be retrieved when searching for the entry in AD. The attributes of parent objects should be included as well.
        /// </summary>
        static protected string[] attributeNames = { "department", "member", "groupType", "objectGUID", "distinguishedName", "description", "memberOf", "objectClass", "objectCategory", "displayName", "cn", "mail", "proxyAddresses", "mailNickname", "targetAddress", "userPrincipalName", "employeeNumber", "badPwdCount", "badPasswordTime", "division", "employeeID", "givenName", "msIIS-FTPDir", "msIIS-FTPRoot", "homeDirectory", "homeDrive", "wWWHomePage", "sn", "scriptPath", "manager", "pwdLastSet", "streetAddress", "title", "msDS-User-Account-Control-Computed", "userAccountControl", "sAMAccountName", "objectSid" };

        // The SearchResultEntry object that represents the Group in Active Directory.
        protected SearchResultEntry entry = null;

        // ----- PROPERTIES -----

        // ----- ActiveDirectoryObject -----
        /// <summary>
        /// The Common Name (CN) of the object in Active Directory.
        /// </summary>
        public string CommonName => ad.GetStringAttributeValue("cn", entry);

        /// <summary>
        /// The time the object was created in UTC.
        /// </summary>
        public DateTime? CreateTimeStamp
        {
            get
            {
                string timeStamp = ad.GetStringAttributeValue("createTimeStamp", entry);
                if (!string.IsNullOrWhiteSpace(timeStamp))
                {
                    return ActiveDirectoryClient.GetDateTimeFromUTCCodedTime(timeStamp);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The Distinguished Name (DN) of the object in Active Directory.
        /// </summary>
        public string DistinguishedName => ad.GetStringAttributeValue("distinguishedName", entry);

        /// <summary>
        /// The GUID of the object in Active Directory.
        /// </summary>
        public Guid Guid => ad.GetGuid(entry);

        /// <summary>
        /// The distinguished name of the organizational unit or parent object containing the object.
        /// </summary>
        public string OrganizationalUnit => ad.GetOrganizationalUnit(DistinguishedName);

        /// <summary>
        /// The schema class types that identify the type of object this is in Active Directory.
        /// Examples: group, user, etc.
        /// </summary>
        public List<string> SchemaClasses => ad.GetStringAttributeValues("objectClass", entry);

        // ----------

        // ----- SECURITY PRINCIPAL ----

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public override DateTime? CreationTime => CreateTimeStamp;

        /// <summary>
        /// A description of the security principal.
        /// </summary>
        public string Description
        {
            get
            {
                return ad.GetStringAttributeValue("description", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "description", value);
            }
        }

        /// <summary>
        /// The principal's e-mail address.
        /// </summary>
        public string EMailAddress
        {
            get
            {
                return ad.GetStringAttributeValue("mail", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "mail", value);
            }
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public override List<string> EmailAddresses
        {
            get
            {
                // The list of e-mail addresses to return.
                List<string> smtpAddresses = new List<string>();

                // Get the attribute from Active Directory.
                List<string> attribute = ad.GetStringAttributeValues("proxyAddresses", entry);

                // Check that proxy addresses has values in it.
                if (attribute != null)
                {
                    if (attribute.Count > 0)
                    {
                        // The attribute has values.

                        // Create a list to return with only SMTP entries.
                        foreach (string address in attribute)
                        {
                            if (address.StartsWith("SMTP:"))
                            {
                                // This is an primary SMTP entry.
                                // Strip the prefix and add it to the beginning of the list.
                                smtpAddresses.Insert(0, address.Substring(5));
                            }
                            else if (address.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
                            {
                                // This is an SMTP entry.
                                // Strip the prefix and add it to the list.
                                smtpAddresses.Add(address.Substring(5));
                            }
                        }
                    }
                    else
                    {
                        // Proxy addresses has no values. Check the mail property.
                        string mailProperty = ad.GetStringAttributeValue("mail", entry);

                        // Check if mail has a value.
                        if (!string.IsNullOrWhiteSpace(mailProperty))
                        {
                            // The property has a value add it to the list.
                            smtpAddresses.Add(mailProperty);
                        }
                    }
                }

                // Return the list of SMTP addresses.
                return smtpAddresses;
            }
            set
            {
                if (value != null)
                {
                    // Get the current list of email addresses associated with the principal.
                    List<string> currentAddresses = EmailAddresses;

                    // Remove addresses that should no longer be associated with the principal.
                    // Those in currentAddresses but not value.
                    foreach (string address in currentAddresses.Except(value))
                    {
                        ad.RemoveProxyAddress(ref entry, address);
                    }

                    // Add addresses that are new. Those in value but not in current addresses.
                    foreach (string address in value.Except(currentAddresses))
                    {
                        ad.AddProxyAddress(ref entry, address);
                    }

                    // Set the primary e-mail address.
                    if (value.Count > 0)
                    {
                        ad.SetPrimaryProxyAddress(ref entry, value[0]);
                        EMailAddress = value[0];
                    }
                }
            }
        }

        /// <summary>
        /// The principal's Microsoft Exchange Alias.
        /// </summary>
        public string ExchangeAlias => ad.GetStringAttributeValue("mailNickname", entry);

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public override List<Identity.Group> Groups
        {
            get
            {
                List<string> groupDns = GroupDns;
                if (groupDns != null)
                {
                    List<Identity.Group> groups = new();
                    foreach (string groupDn in groupDns)
                    {
                        try
                        {
                            groups.Add(new Group(ad, ad.GetGuidByDistinguishedName(groupDn)));
                        }
                        catch
                        {
                            // Skip adding this group. There was an error getting its GUID.
                        }
                    }
                    return groups;
                }
                else
                {
                    // The principal is not a member of any groups.
                    return new();
                }
            }
        }

        /// <summary>
        /// The distinguished names of groups that this principal is a member of.
        /// </summary>
        public List<string> GroupDns => ad.GetStringAttributeValues("memberOf", entry);

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public override string PrimaryEmailAddress
        {
            get
            {
                // Get the attribute from Active Directory.
                List<string> attribute = ad.GetStringAttributeValues("proxyAddresses", entry);

                // Check that proxy addresses has values in it.
                if (attribute != null)
                {
                    if (attribute.Count > 0)
                    {
                        // The attribute has values.

                        // Find the SMTP entry in all caps. This is the primary e-mail address.
                        foreach (string address in attribute)
                        {
                            if (address.StartsWith("SMTP:", StringComparison.Ordinal))
                            {
                                // This is the primary e-mail address.
                                // Strip the prefix and add return it.
                                return address.Substring(5);
                            }
                        }
                    }
                    else
                    {
                        // Proxy addresses has no values. Check the mail property.
                        string mailProperty = ad.GetStringAttributeValue("mail", entry);

                        // Check if mail has a value.
                        if (!string.IsNullOrWhiteSpace(mailProperty))
                        {
                            // The property has a value, return it.
                            return mailProperty;
                        }
                    }
                }
                // No e-mail address could be found for the user.
                return null;
            }
            set
            {
                if (!EmailAddresses.Contains(value))
                {
                    // The address is not associated with the principal. Add it.
                    ad.AddProxyAddress(ref entry, value);
                }
                // Set the address as primary.
                ad.SetPrimaryProxyAddress(ref entry, value);
                EMailAddress = value;
            }
        }

        /// <summary>
        /// The principal's SAM Account Name.
        /// </summary>
        public string SAMAccountName
        {
            get
            {
                return ad.GetStringAttributeValue("sAMAccountName", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "sAMAccountName", value);
            }
        }

        /// <summary>
        /// The principal's target e-mail address. Used by Exchange for routing e-mail to its
        /// final destination which may lie outside of the organization. Allows for an object
        /// to appear in the GAL even though its e-mail address may be outside of Exchange.
        /// Also used when routing e-mail to the Microsoft Office365 cloud from an on-premises
        /// Exchange server.
        /// </summary>
        public string TargetAddress
        {
            get
            {
                return ad.GetStringAttributeValue("targetAddress", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "targetAddress", value);
            }
        }

        /// <summary>
        /// The object's unique ID in the system. (GUID)
        /// </summary>
        public override string UniqueId => Guid.ToString();

        /// <summary>
        /// The User Principal Name of the principal.
        /// </summary>
        public string UserPrincipalName
        {
            get
            {
                return ad.GetStringAttributeValue("userPrincipalName", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "userPrincipalName", value);
            }
        }

        // -----

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
                    return Int32.Parse(ad.GetStringAttributeValue("badPwdCount", entry));
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
                DateTime? returnValue = ad.GetIntervalAttributeValue("badPasswordTime", entry);
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
        /// The user's city.
        /// </summary>
        public override string City
        {
            get
            {
                return ad.GetStringAttributeValue("l", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "l", value);
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        public override string CountryCode
        {
            get
            {
                return ad.GetStringAttributeValue("c", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "c", value);
            }
        }

        /// <summary>
        /// The Department the user belongs to.
        /// </summary>
        public override string Department
        {
            get
            {
                return ad.GetStringAttributeValue("department", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "department", value);
            }
        }

        /// <summary>
        /// The employee number of the user.
        /// </summary>
        public override string EmployeeNumber
        {
            get
            {
                return ad.GetStringAttributeValue("employeeNumber", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "employeeNumber", value);
            }
        }

        /// <summary>
        /// Whether the user's account is disabled in Active Directory.
        /// </summary>
        public override bool IsDisabled => ActiveDirectoryClient.UserAccountControlContains(UserAccountControl, ActiveDirectoryClient.UserAccountControl.Accountdisable);

        /// <summary>
        /// The user's display name.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return ad.GetStringAttributeValue("displayName", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "displayName", value);
            }
        }

        /// <summary>
        /// The user's division in the organization.
        /// </summary>
        public string Division
        {
            get
            {
                return ad.GetStringAttributeValue("division", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "division", value);
            }
        }

        /// <summary>
        /// The Employee Id of the user.
        /// </summary>
        public string EmployeeId
        {
            get
            {
                return ad.GetStringAttributeValue("employeeID", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "employeeID", value);
            }
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public override string FirstName
        {
            get
            {
                return ad.GetStringAttributeValue("givenName", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "givenName", value);
            }
        }

        /// <summary>
        /// The FTP Directory for the user off the FTP root.
        /// </summary>
        public string FTPDirectory
        {
            get
            {
                return ad.GetStringAttributeValue("msIIS-FTPDir", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "msIIS-FTPDir", value);
            }
        }

        /// <summary>
        /// The root directory for FTP access by the user.
        /// </summary>
        public string FTPRoot
        {
            get
            {
                return ad.GetStringAttributeValue("msIIS-FTPRoot", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "msIIS-FTPRoot", value);
            }
        }

        /// <summary>
        /// The path to the user's home directory.
        /// </summary>
        public string HomeDirectory
        {
            get
            {
                return ad.GetStringAttributeValue("homeDirectory", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "homeDirectory", value);
            }
        }

        /// <summary>
        /// The user's home drive letter.
        /// </summary>
        public string HomeDrive
        {
            get
            {
                return ad.GetStringAttributeValue("homeDrive", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "homeDrive", value);
            }
        }

        /// <summary>
        /// The user's home page URL.
        /// </summary>
        public string HomePage
        {
            get
            {
                return ad.GetStringAttributeValue("wWWHomePage", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "wWWHomePage", value);
            }
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public override string LastName
        {
            get
            {
                return ad.GetStringAttributeValue("sn", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "sn", value);
            }
        }

        /// <summary>
        /// The login name (SAMAccountName) for the user in the system.
        /// </summary>
        public override string Login
        {
            get => SAMAccountName;
            set
            {
                SAMAccountName = value;
            }
        }

        /// <summary>
        /// The path to the user's logon script.
        /// </summary>
        public string LogonScript
        {
            get
            {
                return ad.GetStringAttributeValue("scriptPath", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "scriptPath", value);
            }
        }

        /// <summary>
        /// The distinguished name of the user's Manager.
        /// </summary>
        public string Manager
        {
            get
            {
                return ad.GetStringAttributeValue("manager", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "manager", value);
            }
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public override string ManagerId
        {
            get
            {
                return ad.GetGuidByDistinguishedName(Manager).ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    try
                    {
                        User manager = new(ad, Guid.Parse(value));
                        Manager = manager.DistinguishedName;
                    }
                    catch(FormatException)
                    {
                        throw new FormatException("The unique ID (GUID) supplied was invalid.");
                    }

                }
                else
                {
                    throw new ArgumentNullException(nameof(ManagerId));
                }
            }
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public override string ManagerName => new User(ad, ad.GetGuidByDistinguishedName(Manager)).DisplayName;

        /// <summary>
        /// The user's middle name.
        /// </summary>
        public override string MiddleName
        {
            get
            {
                return ad.GetStringAttributeValue("middleName", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "middleName", value);
            }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public override string MobilePhone
        {
            get
            {
                return ad.GetStringAttributeValue("mobile", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "mobile", value);
            }
        }

        /// <summary>
        /// Whether the user has to change their password at their next logon.
        /// </summary>
        public bool MustChangePasswordAtNextLogon
        {
            get
            {
                DateTime? passwordLastSetTime = ad.GetIntervalAttributeValue("pwdLastSet", entry);
                if (passwordLastSetTime.HasValue)
                {
                    if (passwordLastSetTime.Value == ad.JAN_01_1601 && !ActiveDirectoryClient.UserAccountControlContains(UserAccountControl, ActiveDirectoryClient.UserAccountControl.DontExpirePassword))
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
                    ad.SetAttribute(ref entry, "pwdLastSet", new object[] { ActiveDirectoryClient.ToInterval(0) });
                }
                else
                {
                    ad.SetAttribute(ref entry, "pwdLastSet", new object[] { "-1" });
                }
            }
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        public override string Organization
        {
            get
            {
                return ad.GetStringAttributeValue("company", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "company", value);
            }
        }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public override bool PasswordChangeRequiredAtNextLogin => MustChangePasswordAtNextLogon;
        

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public override bool PasswordExpired => ActiveDirectoryClient.UserAccountControlContains(UserAccountControlComputed, ActiveDirectoryClient.UserAccountControl.PasswordExpired);

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public override DateTime? PasswordLastSet
        {
            get
            {
                DateTime? returnValue = ad.GetIntervalAttributeValue("pwdLastSet", entry);
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
                return ad.GetStringAttributeValue("telephoneNumber", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "telephoneNumber", value);
            }
        }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public override string PhysicalAddress
        {
            get => StreetAddress;
            set
            {
                StreetAddress = value;
            }
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        public override string PostalAddress
        {
            get
            {
                return ad.GetStringAttributeValue("postalAddress", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "postalAddress", value);
            }
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public override string PostalCode
        {
            get
            {
                return ad.GetStringAttributeValue("postalCode", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "postalCode", value);
            }
        }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        public override string PrimaryPhone
        {
            get => PhoneNumber;
            set
            {
                PhoneNumber = value;
            }
        }

        /// <summary>
        /// The user's security identifier (SID).
        /// </summary>
        public byte[] SecurityIdentifier => ad.GetByteAttributeValue("objectSid", entry);

        /// <summary>
        /// The user's state.
        /// </summary>
        public override string State
        {
            get
            {
                return ad.GetStringAttributeValue("st", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "st", value);
            }
        }

        /// <summary>
        /// The user's street address.
        /// </summary>
        public string StreetAddress
        {
            get
            {
                return ad.GetStringAttributeValue("streetAddress", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "streetAddress", value);
            }
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        public override string Title
        {
            get
            {
                return ad.GetStringAttributeValue("title", entry);
            }
            set
            {
                ad.SetStringAttribute(ref entry, "title", value);
            }
        }

        /// <summary>
        /// The type or category of the User. Empty if unknown.
        /// </summary>
        public override string Type => ad.GetStringAttributeValue("employeeType", entry);

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
                    return UInt32.Parse(ad.GetStringAttributeValue("userAccountControl", entry));
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
                    return UInt32.Parse(ad.GetStringAttributeValue("msDS-User-Account-Control-Computed", entry));
                }
                catch
                {
                    // There was an error converting the string to a UInt.
                    return 0;
                }
            }
        }

        /// <summary>
        /// A hashed version of the user's UPN to allow for faster compare operations.
        /// </summary>
        public override int HashedIdentifier
        {
            get
            {
                return UserPrincipalName.GetHashCode();
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a user object from Active Directory with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory client used to query and manipulate the user.</param>
        /// <param name="guid">The GUID of the user.</param>
        public User(ActiveDirectoryClient ad, Guid guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                // Set this object's Active Directory object.
                this.ad = ad;

                entry = ad.GetEntryByGUID(guid);
                if (entry == null)
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
        /// <param name="ad">An Active Directory client used to manipulate the user.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public User(ActiveDirectoryClient ad, SearchResultEntry entry)
        {
            // Check that an Active Directory object and search result entry have been provided.
            if (ad != null && entry != null)
            {
                // Set this object's Active Directory object.
                this.ad = ad;
                this.entry = entry;
            }
            else
            {
                if (ad == null)
                {
                    throw new ArgumentNullException(nameof(ad));
                }
                else
                {
                    throw new ArgumentNullException(nameof(entry));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Creates a new user within Active Directory given it's proposed name, the distinguished name of the OU to place it in, and other optional attributes.
        /// </summary>
        /// <param name="ad">An Active Directory client used to create the user.</param>
        /// <param name="sAMAccountName">The proposed SAM Account name for the user.</param>
        /// <param name="ouDn">The distinguished name for the OU to place the user within.</param>
        /// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
        /// <returns>The newly created user object.</returns>
        static public User Create(ActiveDirectoryClient ad, string sAMAccountName, string ouDn, List<DirectoryAttribute> additionalAttributes = null)
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
                    new DirectoryAttribute("sAMAccountName", sAMAccountName),
                    new DirectoryAttribute("objectClass", "user"),
                    new DirectoryAttribute("userPrincipalName", sAMAccountName + "@" + ad.Name)
                };
                if (additionalAttributes != null)
                {
                    // Only add non conflicting attributes.
                    if (additionalAttributes.Exists(x => String.Equals(x.Name, "userPrincipalName", StringComparison.OrdinalIgnoreCase)))
                    {
                        //Removes default UPN from attribute list.
                        attributes.RemoveAll(x => String.Equals(x.Name, "userPrincipalName", StringComparison.OrdinalIgnoreCase));
                    }
                    foreach (DirectoryAttribute attribute in additionalAttributes)
                    {
                        if (attribute.Name != "sAMAccountName" && attribute.Name != "objectClass")
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
                if (ad.Add(userDn, attributes.ToArray()))
                {
                    // The user was created. Retrieve it from Active Directory.
                    return new User(ad, ad.GetGuidByDistinguishedName(userDn));
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
        public override bool Disable()
        {
            return SetUserAccountControlFlag(ActiveDirectoryClient.UserAccountControl.Accountdisable);
        }

        /// <summary>
        /// Enables the user's account for authentication if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public override bool Enable()
        {
            return RemoveUserAccountControlFlag(ActiveDirectoryClient.UserAccountControl.Accountdisable);
        }

        /// <summary>
        /// Gets all users in the Active Directory.
        /// </summary>
        /// <param name="ad">The Active Directory to retrieve users from.</param>
        /// <returns>A list of all users in the Active Directory.</returns>
        static public List<Identity.User> GetAllUsers(ActiveDirectoryClient ad)
        {
            // The LDAP search filter to use to find all the users.
            const string FILTER = "(&(objectCategory=person)(objectClass=user))";

            if (ad != null)
            {
                // Get the GUIDs of all the users in AD.

                // Create a list of attributes that should be retrieved with the query.
                List<string> attributes = new List<string>();
                attributes.AddRange(attributeNames);

                // Search for the users in AD.
                List<SearchResultEntry> entries = ad.GetEntries(FILTER, attributes);

                // Check whether the search returned results.
                if (entries != null)
                {
                    // The search returned results.
                    // Get user objects from the entries retrieved.
                    List<Identity.User> users = new();
                    foreach (SearchResultEntry entry in entries)
                    {
                        users.Add(new User(ad, entry));
                    }
                    return users;
                }
            }
            return new();
        }

        /// <summary>
        /// Gets of all user accounts that were modified within the specified time frame.
        /// </summary>
        /// <param name="ad">The Active Directory client to retrieve users with.</param>
        /// <param name="startDate">The lower boundary of the time frame.</param>
        /// <param name="endDate">The upper boundary of the time frame.</param>
        /// <returns>Returns a list of all users that were during the specified period of time.</returns>
        static public List<User> GetModifiedUsers(ActiveDirectoryClient ad, DateTime startDate, DateTime endDate)
        {
            // The LDAP search filter to use to find all the users.
            string FILTER = String.Format("(&(objectCategory=person)(objectClass=user)(whenChanged>={0})(whenChanged<={1}))", startDate.ToUniversalTime().ToString("yyyyMMddHHmmss.s") + "Z", endDate.ToUniversalTime().ToString("yyyyMMddHHmmss.s") + "Z");

            if (ad != null)
            {
                // Get the GUIDs of all the users in AD.

                // Create a list of attributes that should be retrieved with the query.
                List<string> attributes = new List<string>();
                attributes.AddRange(attributeNames);

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
        public override bool SetPassword(string password)
        {
            // Create a UTF16 array of bytes from the supplied password.
            byte[] bytes = Encoding.Unicode.GetBytes("\"" + password + "\"");
            return ad.SetAttribute(ref entry, "unicodePwd", new object[] { bytes });
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public override bool Unlock()
        {
            // Setting a user's lockout time to 0 unlocks the account.
            return ad.SetAttribute(ref entry, "lockoutTime", new object[] { BitConverter.GetBytes(0) });
        }

        /// <summary>
        /// Sets a flag in the user's User Account Control attribute.
        /// </summary>
        /// <param name="flag">A flag from the predefined UserAccountControl flags.</param>
        /// <returns>True if set, false otherwise.</returns>
        public bool SetUserAccountControlFlag(ActiveDirectoryClient.UserAccountControl flag)
        {
            // Use the normal user account control attribute.
            uint newUserAccountControl = UserAccountControl | (uint)flag;
            return ad.SetStringAttribute(ref entry, "userAccountControl", newUserAccountControl.ToString());
        }

        /// <summary>
        /// Removes a flag from the user's User Account Control attribute.
        /// </summary>
        /// <param name="flag">A flag from the predefined UserAccountControl flags.</param>
        /// <returns>True if removed, false otherwise.</returns>
        public bool RemoveUserAccountControlFlag(ActiveDirectoryClient.UserAccountControl flag)
        {
            // Use the normal user account control attribute.
            uint newUserAccountControl = UserAccountControl & ~(uint)flag;
            return ad.SetStringAttribute(ref entry, "userAccountControl", newUserAccountControl.ToString());
        }
    }
}
