using Galactic_LDAP = Galactic.LDAP.LDAP;
using Galactic.Configuration;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Text;
using DSAD = System.DirectoryServices.ActiveDirectory;

namespace Galactic.ActiveDirectory
{
    /// <summary>
    /// ActiveDirectory is a class that allows for the query and manipulation
    /// of Active Directory objects.
    /// </summary>
    public class ActiveDirectory : IDisposable
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The default first site name in Active Directory.
        /// </summary>
        public const string DEFAULT_FIRST_SITE_NAME = "Default-First-Site-Name";

        /// <summary>
        /// The maximum number of characters supported for a group's name in Active Directory.
        /// </summary>
        public const int GROUP_NAME_MAX_CHARS = 63;

        /// <summary>
        /// The size of page to use when searching Active Directory. This number is based upon
        /// hardcoded Microsoft limits within Active Directory's architecture.
        /// </summary>
        public const int PAGE_SIZE = 1000;

        /// <summary>
        /// The maximum number of values that can be retrieved from a multi-value attribute in a single search request.
        /// Windows 2000 DCs do not support this value and default to a maximum of 1000;
        /// </summary>
        public const int MAX_NUM_MULTIVALUE_ATTRIBUTES = 1500;

        // ----- VARIABLES -----

        /// <summary>
        /// GroupType enumerates the type of group objects in Active Directory.
        /// </summary>
        [Flags]
        public enum GroupType : uint
        {
            /// <summary>
            /// Specifies a group that can contain accounts from any domain, global
            /// groups from any domain, and other universal groups. This type of group
            /// cannot contain domain local groups.
            /// </summary>
            Universal = 0x08,

            /// <summary>
            /// Specifies a group that can contain accounts from any domain, other domain
            /// local groups from the same domain, global groups from any domain, and
            /// universal groups. This type of group should not be included in access-control
            /// lists of resouces in other domains. This type of group is intended for use
            /// with the LDAP provider.
            /// </summary>
            DomainLocal = 0x04,

            /// <summary>
            /// Specifies a group that can contain accounts from the domain and other global
            /// groups from the same domain. This type of group can be exported to a different
            /// domain.
            /// </summary>
            Global = 0x02,

            /// <summary>
            /// Specifies a group that is security enabled. This group can be used to apply an
            /// access-control list on an Active Directory object or a file system.
            /// </summary>
            Security = 0x80000000
        }

        // The object that manages the LDAP connection with the AD controller.
        private readonly Galactic_LDAP ldap = null;

        /// <summary>
        /// Flags for use with the UserAccountControl and ms-DS-User-Account-Control-Computed properties of a user.
        /// </summary>
        public enum UserAccountControl : uint
        {
            /// <summary>
            /// The logon script will be run.
            /// </summary>
            Script = 0x0001,

            /// <summary>
            /// The user account is disabled.
            /// </summary>
            Accountdisable = 0x0002,

            /// <summary>
            /// The home folder is required.
            /// </summary>
            HomedirRequired = 0x0008,

            /// <summary>
            /// Only available via ms-DS-User-Account-Control-Computed attribute.
            /// </summary>
            Lockout = 0x0010,

            /// <summary>
            /// No password is required.
            /// </summary>
            PasswdNotreqd = 0x0020,

            /// <summary>
            /// The user cannot change the password. This is a permission on the user's object.
            /// For information about how to set this permission, visit the following Web site:
            /// http://msdn2.microsoft.com/en-us/library/aa746398.aspx
            /// </summary>
            PasswdCantChange = 0x0040,

            /// <summary>
            /// The user can send an encrypted password.
            /// </summary>
            EncryptedTextPwdAllowed = 0x0080,

            /// <summary>
            /// This is an account for users whose primary account is in another domain. This
            /// account provides user access to this domain, but not to any domain that trusts
            /// this domain. This is sometimes referred to as a local user account.
            /// </summary>
            TempDuplicateAccount = 0x0100,

            /// <summary>
            /// This is a default account type that represents a typical user.
            /// </summary>
            NormalAccount = 0x0200,

            /// <summary>
            /// This is a permit to trust an account for a system domain that trusts other domains.
            /// </summary>
            InterdomainTrustAccount = 0x0800,

            /// <summary>
            /// This is a computer account for a computer that is running Microsoft Windows NT 4.0
            /// Workstation, Microsoft Windows NT 4.0 Server, Microsoft Windowser 2000 Professional,
            /// or Widnows 2000 Server and is a member of this domain.
            /// </summary>
            WorkstationTrustAccount = 0x1000,

            /// <summary>
            /// This is a computer account for a domain controller that is a member of this domain.
            /// </summary>
            ServerTrustAccount = 0x2000,

            /// <summary>
            /// Represents the password, which should never expire on the account.
            /// </summary>
            DontExpirePassword = 0x10000,

            /// <summary>
            /// This is an MNS logon account.
            /// </summary>
            MNSLogonAccount = 0x20000,

            /// <summary>
            /// When this flag is set, it forces the user to log on by using a smart card.
            /// </summary>
            SmartcardRequired = 0x40000,

            /// <summary>
            /// When this flag is set, the service account (the user or computer account) under which
            /// a service runs is trusted for Kerberos delegation. Any such service can impersonate
            /// a client requesting the service. To enable a service for Kerberos delegation, you must
            /// set this flag on the userAccountControl property of the service account.
            /// </summary>
            TrustedForDelgation = 0x80000,

            /// <summary>
            /// When this flag is set, the security context of the user is not delegated to a service
            /// even if the service account is set as trusted for Kerberos delegation.
            /// </summary>
            NotDelegated = 0x100000,

            /// <summary>
            /// (Windows 2000/Windows Server 2003) Restrict this principal to use only Data Encryption 
            /// Standard (DES) encryption for keys.
            /// </summary>
            UseDESKeyOnly = 0x200000,

            /// <summary>
            /// (Windows 2000/Windows Server 2003) This account does not require Kerberos pre-authentication
            /// for logging on.
            /// </summary>
            DontReqPreauth = 0x400000,

            /// <summary>
            /// Only available via ms-DS-User-Account-Control-Computed attribute.
            /// (Windows 2000/Windows Server 2003) The user's password has expired.
            /// </summary>
            PasswordExpired = 0x800000,

            /// <summary>
            /// (Windows 2000/Windows Server 2003) The account is enabled for delegation. This is a security-sensitive
            /// setting. Accounts that have this option enabled should be tightly controlled. Thsi setting lets a service
            /// that runs under the account assume a client's identity and authenticate as that user to other remote servers
            /// on the network.
            /// </summary>
            TrustedToAuthForDelegation = 0x1000000,

            /// <summary>
            /// Only available via ms-DS-User-Account-Control-Computed attribute.
            /// (Windows Server 2008/Windows Server 2008 R2) The account is a read-only domain controller (RODC). This is a
            /// security-sensitive setting. Removing this setting from an RODC compromises security on that server.
            /// </summary>
            PartialSecretsAccount = 0x4000000,

            /// <summary>
            /// Only available via ms-DS-User-Account-Control-Computed attribute.
            /// </summary>
            UseAESKeys = 0x8000000
        }

        // ----- PROPERTIES -----

        /// <summary>
        /// The base distinguished name (DN) of Active Directory.
        /// </summary>
        public string DistinguishedName
        {
            get
            {
                if (ldap != null)
                {
                    return ldap.GetStringAttributeValue("defaultNamingContext", ldap.RootDSE);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The domain name of the Active Directory.
        /// </summary>
        public string Name
        {
            get
            {
                if (ldap != null)
                {
                    SearchResultEntry domain = ldap.GetEntryByDistinguishedName(DistinguishedName, new List<string> { "canonicalName" }, DistinguishedName, SearchScope.Base);
                    string canonicalName = ldap.GetStringAttributeValue("canonicalName", domain);
                    if (!string.IsNullOrWhiteSpace(canonicalName))
                    {
                        return canonicalName.Replace("/", "");
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The NT style domain name of the Active Directory.
        /// </summary>
        public string NTName
        {
            get
            {
                if (ldap != null)
                {
                    SearchResultEntry domain = ldap.GetEntryByDistinguishedName(DistinguishedName, new List<string> { "msDS-PrincipalName" }, DistinguishedName, SearchScope.Base);
                    if (domain.Attributes.Contains("msDS-PrincipalName"))
                    {
                        string ntName = ldap.GetStringAttributeValue("msDS-PrincipalName", domain);
                        if (!string.IsNullOrWhiteSpace(ntName))
                        {
                            return ntName.Replace(@"\", "");
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The SYSTEM sid.
        /// </summary>
        public SecurityIdentifier WellKnownSid_System
        {
            get
            {
                return new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            }
        }

        /// <summary>
        /// The distinguished name of the Administrators group for this domain.
        /// </summary>
        public string AdministratorsGroupDN
        {
            get
            {
                return "CN=Administrators,CN=Builtin," + DistinguishedName;
            }
        }

        /// <summary>
        /// The distinguished name of the Domain Administrators group for this domain.
        /// </summary>
        public string DomainAdminsGroupDN
        {
            get
            {
                return "CN=Domain Admins,CN=Users," + DistinguishedName;
            }
        }

        /// <summary>
        /// The distinguished name of the Domain Users group for this domain.
        /// </summary>
        public string DomainUsersGroupDN
        {
            get
            {
                return "CN=Domain Users,CN=Users," + DistinguishedName;
            }
        }

        /// <summary>
        /// The distinguished name of the Enterprise Administrators group for this domain.
        /// </summary>
        public string EnterpriseAdminsGroupDN
        {
            get
            {
                return "CN=Enterprise Admins,CN=Users," + DistinguishedName;
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Binds to Active Directory using the configuration in the specified configuration item.
        /// </summary>
        /// <param name="configurationItemDirectoryPath">The physical path to the directory where configuration item files can be found.</param>
        /// <param name="configurationItemName">The name of the configuration item containing the Active Directory configuration.</param>
        public ActiveDirectory(string configurationItemDirectoryPath, string configurationItemName)
        {
            if (!string.IsNullOrWhiteSpace(configurationItemDirectoryPath) && !string.IsNullOrWhiteSpace(configurationItemName))
            {
                // Get the configuration item with the connection data from a file.
                ConfigurationItem configItem = new ConfigurationItem(configurationItemDirectoryPath, configurationItemName, true);

                // Get the connection data from the configuration item.
                StringReader reader = new StringReader(configItem.Value);

                string domainAndSiteLine = reader.ReadLine();

                // Get the domain name and site name (if specified).
                string[] domainAndSiteLineSections = new string[] {};
                if (!string.IsNullOrWhiteSpace(domainAndSiteLine))
                {
                    domainAndSiteLineSections = domainAndSiteLine.Split(':');
                }
                string domainName = domainAndSiteLineSections[0];
                string siteName = DEFAULT_FIRST_SITE_NAME;
                if (domainAndSiteLineSections.Length > 1)
                {
                    siteName = domainAndSiteLineSections[1];
                }

                string userName = reader.ReadLine();
                SecureString password = new SecureString();
                int intRead = reader.Read();
                while (intRead >= 0)
                {
                    char c = Convert.ToChar(intRead);
                    if (c != '\n')
                    {
                        password.AppendChar(c);
                    }
                    else
                    {
                        break;
                    }
                    intRead = reader.Read();
                }

                if (!string.IsNullOrWhiteSpace(domainName) && !string.IsNullOrWhiteSpace(userName) && password.Length > 0)
                {
                    try
                    {
                        // Get a list of domain controllers from a specific site, if one was supplied.
                        List<string> domainControllers = new List<string>();
                        if (!string.IsNullOrWhiteSpace(siteName))
                        {
                            domainControllers = GetSiteDomainControllers(domainName, siteName);
                        }

                        if (domainControllers.Count == 0)
                        {
                            // Create the connection to the domain controller serving the current computer.
                            ldap = new Galactic_LDAP(new List<string> { domainName }, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                        }
                        else
                        {
                            // Create the connection to the domain controllers serving the specified site.
                            ldap = new Galactic_LDAP(domainControllers, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                        }

                        // Set the default search base and scope.
                        ldap.SetSearchBaseAndScope(DistinguishedName);
                    }
                    catch
                    {
                        throw new ArgumentException("Unable to establish connection to Active Directory.");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(domainName))
                    {
                        throw new ArgumentException("Domain name not provided in configuration item.");
                    }
                    else if (string.IsNullOrWhiteSpace(userName))
                    {
                        throw new ArgumentException("User name not provided in configuration item.");
                    }
                    else
                    {
                        throw new ArgumentException("Password not provided in configuration item.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Unable to establish connection to Active Directory.");
            }
        }

        /// <summary>
        /// Binds to Active Directory.
        /// </summary>
        /// <param name="domainName">The DNS style domain name of the Active Directory to connect to.</param>
        /// <param name="userName">The username of the account in AD to use when making the connection.</param>
        /// <param name="password">The password of the account.</param>
        /// <param name="siteName">(Optional)The name of a site in Active Directory to use the domain controllers from. Defaults to DEFAULT_FIRST_SITE_NAME if not supplied.</param>
        public ActiveDirectory(string domainName, string userName, SecureString password, string siteName = DEFAULT_FIRST_SITE_NAME)
        {
            if (!string.IsNullOrWhiteSpace(domainName) && !string.IsNullOrWhiteSpace(userName) && password != null)
            {
                try
                {
                    // Get a list of domain controllers from a specific site, if one was supplied.
                    List<string> domainControllers = new List<string>();
                    if (!string.IsNullOrWhiteSpace(siteName))
                    {
                        domainControllers = GetSiteDomainControllers(domainName, siteName);
                    }

                    if (domainControllers.Count == 0)
                    {
                        // Create the connection to the domain controller serving the current computer.
                        ldap = new Galactic_LDAP(new List<string> { domainName }, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                    }
                    else
                    {
                        // Create the connection to the domain controllers serving the specified site.
                        ldap = new Galactic_LDAP(domainControllers, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                    }

                    // Set the default search base and scope.
                    ldap.SetSearchBaseAndScope(DistinguishedName);
                }
                catch
                {
                    throw new ArgumentException("Unable to establish connection to Active Directory.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    throw new ArgumentNullException("domainName");
                }
                if (string.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentNullException("userName");
                }
                if (password == null)
                {
                    throw new ArgumentNullException("password");
                }
            }
        }

        /// <summary>
        /// Constructs an Active Directory object with a base of the specified OU. Binds to Active Directory.
        /// </summary>
        /// <param name="domainName">The DNS style domain name of the Active Directory to connect to.</param>
        /// <param name="ouDn">The distinguished name of the OU to use as a base for operations.</param>
        /// <param name="userName">The username of the account in AD to use when making the connection.</param>
        /// <param name="password">The password of the account.</param>
        /// <param name="siteName">(Optional)The name of a site in Active Directory to use the domain controllers from. Defaults to DEFAULT_FIRST_SITE_NAME if not supplied.</param>
        public ActiveDirectory(string domainName, string ouDn, string userName, SecureString password, string siteName = DEFAULT_FIRST_SITE_NAME)
        {
            if (!string.IsNullOrWhiteSpace(domainName) && !string.IsNullOrWhiteSpace(ouDn) && !string.IsNullOrWhiteSpace(userName) && password != null)
            {
                try
                {
                    // Get a list of domain controllers from a specific site, if one was supplied.
                    List<string> domainControllers = new List<string>();
                    if (!string.IsNullOrWhiteSpace(siteName))
                    {
                        domainControllers = GetSiteDomainControllers(domainName, siteName);
                    }

                    if (domainControllers.Count == 0)
                    {
                        // Create the connection to the domain controller serving the current computer.
                        ldap = new Galactic_LDAP(new List<string> { domainName }, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                    }
                    else
                    {
                        // Create the connection to the domain controllers serving the specified site.
                        ldap = new Galactic_LDAP(domainControllers, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, userName, password, domainName);
                    }

                    // Set the search base to the specified dn.
                    ldap.SetSearchBaseAndScope(ouDn);
                }
                catch
                {
                    throw new ArgumentException("Unable to establish connection to Active Directory.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    throw new ArgumentNullException("domainName");
                }
                if (string.IsNullOrWhiteSpace(ouDn))
                {
                    throw new ArgumentNullException("ouDn");
                }
                if (string.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentNullException("userName");
                }
                if (password == null)
                {
                    throw new ArgumentNullException("password");
                }
            }
        }
        /// <summary>
        /// Binds to Active Directory. Uses the current session credentials to authenticate.
        /// </summary>
        /// <param name="domainName">(Optional) The DNS style domain name of the Active Directory to connect to. If left unspecified, the domain that the computer is currently connected to will be used.</param>
        public ActiveDirectory(string domainName = null)
        {
            string siteName = DEFAULT_FIRST_SITE_NAME;

            if (string.IsNullOrWhiteSpace(domainName))
            {
                using (DSAD.Domain domain = DSAD.Domain.GetComputerDomain())
                {
                    domainName = domain.Name;
                }
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    throw new ArgumentNullException("domainName", "Specified domain name is invalid. Unable to autodetect Active Directory domain.");
                }
            }

            try
            {
                // Get a list of domain controllers from a specific site, if one was supplied.
                List<string> domainControllers = new List<string>();
                if (!string.IsNullOrWhiteSpace(siteName))
                {
                    domainControllers = GetSiteDomainControllers(domainName, siteName);
                }

                if (domainControllers.Count == 0)
                {
                    // Create the connection to the domain controller serving the current computer.
                    ldap = new Galactic_LDAP(new List<string> { domainName }, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, null, null, domainName, true);
                }
                else
                {
                    // Create the connection to the domain controllers serving the specified site.
                    ldap = new Galactic_LDAP(domainControllers, Galactic_LDAP.LDAP_SSL_PORT, AuthType.Negotiate, null, null, domainName, true);
                }

                // Set the default search base and scope.
                ldap.SetSearchBaseAndScope(DistinguishedName);
            }
            catch
            {
                throw new ArgumentException("Unable to establish connection to Active Directory.");
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Adds an entry to the Active Directory with the specified distinguished name and attributes.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to add.</param>
        /// <param name="attributes">The attributes for the entry to add.</param>
        /// <returns>True if added, false otherwise.</returns>
        public bool Add(string dn, DirectoryAttribute[] attributes)
        {
            if (!string.IsNullOrWhiteSpace(dn))
            {
                return ldap.Add(dn, attributes);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds or replaces the attribute value(s) in the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="values">The value(s) to set the attribute to.</param>
        /// <param name="entry">The SearchResultEntry to set the attribute value in.</param>
        /// <returns>True if it was set, false otherwise.</returns>
        public bool AddOrReplaceAttributeValue(string name, object[] values, SearchResultEntry entry)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && values != null && entry != null)
                {
                    return ldap.AddOrReplaceAttribute(entry.DistinguishedName, name, values);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Appends the distinguished name of this Active Directory domain to the relative path to the root supplied.
        /// </summary>
        /// <param name="pathToRoot">The relative path to the root of this domain.</param>
        /// <returns>The absolute path including this domain's distinguished name. Null if a null string is supplied.</returns>
        public string AppendDistinguishedName(string pathToRoot)
        {
            if (!string.IsNullOrWhiteSpace(pathToRoot))
            {
                // The string is valid. Return the absolute path.
                return pathToRoot + "," + DistinguishedName;
            }
            else
            {
                // The string is null or full of whitespace.
                // Check if the string is empty.
                if (pathToRoot != null)
                {
                    return DistinguishedName;
                }
                return null;
            }
        }

        /// <summary>
        /// Deletes an entry with the specified GUID from Active Directory.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>True if the entry was deleted, false otherwise.</returns>
        public bool Delete(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                // Search for an entry with the specified GUID.
                SearchResultEntry entry = GetEntryByGUID(guid, new List<string> { "distinguishedName" });

                if (entry != null)
                {
                    // An entry was found with the specified GUID.
                    return ldap.Delete(entry.DistinguishedName);
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes an attribute's values from the specified entry in Active Directory.
        /// </summary>
        /// <param name="name">The name of the attribute that should have its value deleted.</param>
        /// <param name="entry">The SearchResultEntry containing the attribute to delete.</param>
        /// <param name="values">Optional: The specific values to delete. If null, all values will be deleted. Defaults to null.</param>
        /// <returns>True if the attribute's valuea are deleted, false otherwise.</returns>
        public bool DeleteAttribute(string name, SearchResultEntry entry, object[] values = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && entry != null)
                {
                    return ldap.DeleteAttribute(entry.DistinguishedName, name, values);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Releases underlying resources associated with the Active Directory connection.
        /// </summary>
        public void Dispose()
        {
            ldap.Dispose();
        }

        /// <summary>
        /// Gets the first byte attribute value from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A first byte array value held in the attribute, or an empty byte array if there was an error retrieving the value or the attribute was empty.</returns>
        public byte[] GetByteAttributeValue(string name, SearchResultEntry entry)
        {
            return ldap.GetByteAttributeValue(name, entry);
        }

        /// <summary>
        /// Gets all byte attribute values from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>An array of byte array values held in the attribute, or an empty array of byte arrays if there was an error retrieving the values or the attribute was empty.</returns>
        public byte[][] GetByteAttributeValues(string name, SearchResultEntry entry)
        {
            return ldap.GetByteAttributeValues(name, entry);
        }

        /// <summary>
        /// Gets the UTC DateTime from a Interval attribute of the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>The DateTime representing the interval supplied or null if attribute could not be found.</returns>
        public DateTime? GetIntervalAttributeValue(string name, SearchResultEntry entry)
        {
            // Interval attributes are stored as a Windows File Time strings with the number of 100-nanosecond intervals that have elapsed since 12:00 midnight, January 1, 1601 AD, UTC.
            string intervalStringValue = GetStringAttributeValue(name, entry);
            if (!string.IsNullOrWhiteSpace(intervalStringValue))
            {
                Int64 numIntervals;
                // Parse the file time string.
                if (Int64.TryParse(intervalStringValue, out numIntervals))
                {
                    // Convert the number of intervals to a DateTime.
                    return DateTime.FromFileTimeUtc(numIntervals);
                }
                else
                {
                    // The interval value couldn't be parsed.
                    return null;
                }
            }
            else
            {
                // Couldn't retrieve the attribute.
                return null;
            }
        }

        /// <summary>
        /// Gets the domain controllers associated with a specific Active Directory site from the Active Directory's DNS SRV records.
        /// </summary>
        /// <param name="domainName">The DNS domain name of the Active Directory to retrieve the domain controllers for.</param>
        /// <param name="siteName">The name of the site to retrieve the domain controllers for.</param>
        /// <returns>A list containg the FQDNs of the domain controllers in the specified site, or an empty list if they could not be retrieved.</returns>
        static public List<String> GetSiteDomainControllers(string domainName, string siteName)
        {
            if (!string.IsNullOrWhiteSpace(domainName) && !string.IsNullOrWhiteSpace(siteName))
            {
                try
                {
                    DnsQueryRequest request = new DnsQueryRequest();
                    DnsQueryResponse response = request.Resolve("_ldap._tcp." + siteName + "._sites.dc._msdcs." + domainName, NsType.SRV, NsClass.INET, System.Net.Sockets.ProtocolType.Tcp);
                    IDnsRecord[] records = response.Answers;
                    List<string> domainControllers = new List<string>();
                    foreach (IDnsRecord record in records)
                    {
                        domainControllers.Add((record as SrvRecord).HostName);
                    }
                    return domainControllers;
                }
                catch
                {
                    // There was an error contacting or retrieving data from the DNS server.
                    return new List<string>();
                }
            }
            else
            {
                // Invalid parameters were supplied.
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the first string attribute value from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A first string value held in the attribute, or null if there was an error retrieving the value or the attribute was empty.</returns>
        public string GetStringAttributeValue(string name, SearchResultEntry entry)
        {
            return ldap.GetStringAttributeValue(name, entry);
        }

        /// <summary>
        /// Gets all string attribute values from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A list of string values held in the attribute, or null if there was an error retrieving the values or the attribute was empty.</returns>
        public List<string> GetStringAttributeValues(string name, SearchResultEntry entry)
        {
            // Get the attribute values from active directory.
            List<string> results = ldap.GetStringAttributeValues(name, entry);

            // Check whether a range retrieval is necessary for a large results set.
            if (results != null && results.Count == 0)
            {
                // There are 0 results in the set. Check if a range retrieval attribute was supplied in the results.
                // Search using range retrieval until no results are left.
                int rangeStart = 0;

                // Search using LDAP to get a new SearchResultEntry with the desired attribute.
                string rangeAttributeName = name + ";range=" + rangeStart + "-*";
                Guid entryGuid = GetGUID(entry);
                entry = GetEntryByGUID(entryGuid, new List<string> { rangeAttributeName });

                // Get the values of the ranged attribute.
                // The name of the attribute containing the results is specific to the actual number of results in the range.
                rangeAttributeName = name + ";range=" + rangeStart + "-" + (rangeStart + MAX_NUM_MULTIVALUE_ATTRIBUTES - 1);
                List<string> rangeResults = ldap.GetStringAttributeValues(rangeAttributeName, entry);
                while (rangeResults != null && rangeResults.Count != 0)
                {
                    // Range retrieval results were found.
                    // Add the results to the values to return.
                    results.AddRange(rangeResults);

                    // Check for more results.

                    // Increment the rangeStart to the next range to check.
                    rangeStart += MAX_NUM_MULTIVALUE_ATTRIBUTES;

                    // Do a search for a new SearchResultEntry with the new range.
                    rangeAttributeName = name + ";range=" + rangeStart + "-*";
                    entry = GetEntryByGUID(entryGuid, new List<string> { rangeAttributeName });

                    // If an attribute name with the a wildcard for the end range number is returned this is the last of the result set.
                    rangeResults = ldap.GetStringAttributeValues(rangeAttributeName, entry);
                    if (rangeResults != null && rangeResults.Count != 0)
                    {
                        // This is the last of the results in the set.
                        results.AddRange(rangeResults);
                        break;
                    }
                    else
                    {
                        // There are more results in the set. Get the range specific attribute for the values.
                        rangeAttributeName = name + ";range=" + rangeStart + "-" + (rangeStart + MAX_NUM_MULTIVALUE_ATTRIBUTES - 1);
                        rangeResults = ldap.GetStringAttributeValues(rangeAttributeName, entry);
                    }
                }

                // All the results have been retrieved.
                // Return the results.
                return results;
            }
            else
            {
                // There are multiple values returned.
                // Return the results.
                return results;
            }
        }

        /// <summary>
        /// Checks whether the group name supplied conforms to the limitations imposed by Active Directory.
        /// Active Directory Group Name Limitations:
        /// 63 character length limit
        /// Can not consist solely of numbers, periods, or spaces.
        /// There must be no leading periods or spaces.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if it meets the limitations, false otherwise.</returns>
        static public bool IsGroupNameValid(string name)
        {
            // Check whether the name supplied is valid.
            if (!string.IsNullOrEmpty(name))
            {
                // Check whether the length of the name is less than or equal to 63 characters.
                if (name.Length <= GROUP_NAME_MAX_CHARS)
                {
                    // The name is of an appropriate length.

                    // Check whether the name begins with a period or space.
                    if ((name[0] != ' ') && (name[0] != '.'))
                    {
                        // The name does not begin with a period or space.

                        // Check whether the string contains letters.
                        foreach (char c in name)
                        {
                            if (char.IsLetter(c))
                            {
                                // The name contains a letter and is therefore valid.
                                return true;
                            }
                        }
                    }
                }
            }
            // The name is not valid.
            return false;
        }

        /// <summary>
        /// Gets a DateTime representation from the UTC coded time string used by some Active Directory attributes.
        /// </summary>
        /// <param name="utcCodedTime">The string that contains the UTC coded time.</param>
        /// <returns>A new DateTime with the time, or null if the string could not be parsed or was not supplied.</returns>
        public static DateTime? GetDateTimeFromUTCCodedTime(string utcCodedTime)
        {
            if (!string.IsNullOrWhiteSpace(utcCodedTime))
            {
                // Verify that the time string has the correct number of characters.
                if (utcCodedTime.Length == 17)
                {
                    int year;
                    int month;
                    int day;
                    int hour;
                    int minute;
                    int second;
                    // Parse the year.
                    if (Int32.TryParse(utcCodedTime.Substring(0, 4), out year))
                    {
                        // Continue parsing. Parse the month.
                        if (Int32.TryParse(utcCodedTime.Substring(4, 2), out month))
                        {
                            // Continue parsing. Parse the day.
                            if (Int32.TryParse(utcCodedTime.Substring(6, 2), out day))
                            {
                                // Continue parsing. Parse the hour.
                                if (Int32.TryParse(utcCodedTime.Substring(8, 2), out hour))
                                {
                                    // Continue parsing. Parse the minute.
                                    if (Int32.TryParse(utcCodedTime.Substring(10, 2), out minute))
                                    {
                                        // Continue parsing. Parse the second.
                                        if (Int32.TryParse(utcCodedTime.Substring(1, 2), out second))
                                        {
                                            // Return the local date time associated with the UTC coded time.
                                            return new DateTime(year, month, day, hour, minute, second);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    // Some component of the time string could not be parsed.
                    return null;
                }
                else
                {
                    // The time is not in the correct format.
                    return null;
                }
            }
            else
            {
                // A time string was not provided.
                return null;
            }
        }

        /// <summary>
        /// Gets an entry given an attribute name and value to search for.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to search against.</param>
        /// <param name="attributeValue">The value to search for in the attribute.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByAttribute(string attributeName, string attributeValue, List<string> attributes = null)
        {
            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
            {
                // Create an LDAP search filter string that will find the entry in the directory with
                // the specified attribute value.
                string filter = "(" + attributeName + "=" + attributeValue + ")";

                // Search the directory for the entry with the specified attribute value.
                return GetEntry(filter, attributes);
            }
            // The attribute name or value provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets entries that match a given wildcarded (*) attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to search against.</param>
        /// <param name="attributeValue">The value to search for in the attribute.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The list of SearchResultEntry(s) found, or null if not found.</returns>
        public List<SearchResultEntry> GetEntriesByAttribute(string attributeName, string attributeValue, List<string> attributes = null)
        {
            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
            {
                // Create an LDAP search filter string that will find the entry in the directory with
                // the specified attribute value.
                string filter = "(" + attributeName + "=" + attributeValue + ")";

                // Search the directory for the entry with the specified attribute value.
                return GetEntries(filter, attributes);
            }
            // The attribute name or value provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets an entry given its common name.
        /// </summary>
        /// <param name="cn">The common name of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByCommonName(string cn, List<string> attributes = null)
        {
            return GetEntryByAttribute("cn", cn, attributes);
        }

        /// <summary>
        /// Gets an entry given its distinguished name.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByDistinguishedName(string dn, List<string> attributes = null)
        {
            return GetEntryByAttribute("distinguishedName", dn, attributes);
        }

        /// <summary>
        /// Gets an entry given its Account ID.
        /// </summary>
        /// <param name="accountId">The account ID of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByAccountId(string accountId, List<string> attributes = null)
        {
            return GetEntryByAttribute("employeeNumber", accountId, attributes);
        }

        /// <summary>
        /// Gets an entry given its E-mail address.
        /// </summary>
        /// <param name="emailAddress">The e-mail address of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByEmailAddress(string emailAddress, List<string> attributes = null)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                // Create an LDAP search filter string that will find the entry in the directory with
                // the specified email address.
                // Create filters that search for their primary and secondary addresses.
                string primaryFilter = "(&(proxyAddresses=SMTP:" + emailAddress + ")(objectCategory=person)(objectClass=user))";
                string secondaryFilter = "(&(proxyAddresses=smtp:" + emailAddress + ")(objectCategory=person)(objectClass=user))";

                // Search the directory for an entry with the specified primary e-mail address.
                SearchResultEntry entryFound = GetEntry(primaryFilter, attributes);

                // If the search didn't return a value try searching for a secondary e-mail address match.
                if (entryFound == null)
                {
                    entryFound = GetEntry(secondaryFilter, attributes);
                }

                // If the search didn't return a value try searching by their 'mail' attribute.
                if (entryFound == null)
                {
                    string filter = "(&(mail=" + emailAddress + ")(objectCategory=person)(objectClass=user))";
                    entryFound = GetEntry(filter, attributes);
                }

                // Return the directory entry of the account found, or null if none was found.
                return entryFound;
            }
            // The e-mail address provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets an entry given its GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryByGUID(Guid guid, List<string> attributes = null)
        {
            if (guid != Guid.Empty)
            {
                // Create an LDAP search filter string that will find the entry in the directory with
                // the specified GUID.
                StringBuilder guidBuilder = new StringBuilder();
                foreach (byte guidByte in guid.ToByteArray())
                {
                    guidBuilder.Append('\\' + guidByte.ToString("x2"));
                }
                string filter = "(objectGUID=" + guidBuilder + ")";

                // Search the directory for an entry with the specified distinguished name.
                return GetEntry(filter, attributes);
            }
            // The distinguished name provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets an entry given its SAM account name.
        /// </summary>
        /// <param name="sAMAccountName">The SAM account name of the entry to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object found, or null if not found.</returns>
        public SearchResultEntry GetEntryBySAMAccountName(string sAMAccountName, List<string> attributes = null)
        {
            return GetEntryByAttribute("sAMAccountName", sAMAccountName, attributes);
        }

        /// <summary>
        /// Gets entries that match a given wildcarded (*) SAM account name.
        /// </summary>
        /// <param name="sAMAccountName">The SAM account name of the entries to get.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in each entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The list of SearchResultEntry(s) found, or null if not found.</returns>
        public List<SearchResultEntry> GetEntriesBySAMAccountName(string sAMAccountName, List<string> attributes = null)
        {
            return GetEntriesByAttribute("sAMAccountName", sAMAccountName, attributes);
        }

        /// <summary>
        /// Gets the first entry in a search given an LDAP search filter.
        /// </summary>
        /// <param name="filter">The LDAP search filter string that will find the entry.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in each entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>The SearchResultEntry object of the entry, or null if not found.</returns>
        public SearchResultEntry GetEntry(string filter, List<string> attributes = null)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                // Search the directory for an entry with the specified SAM account name.
                List<SearchResultEntry> entries = GetEntries(filter, attributes);

                if (entries != null && entries.Count > 0)
                {
                    // Return the first entry found.
                    return entries[0];
                }
                else
                {
                    // No entries were found.
                    return null;
                }
            }
            // The filter provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets all entries in a search given an LDAP search filter.
        /// </summary>
        /// <param name="filter">The LDAP search filter string that will find the entries.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in each entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
        /// <returns>A list of SearchResultEntry objects, or null if not found.</returns>
        public List<SearchResultEntry> GetEntries(string filter, List<string> attributes = null)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                // Search the directory for an entry with the specified SAM account name.
                return ldap.Search(filter, attributes, null, SearchScope.Subtree, PAGE_SIZE, false);
            }
            // The filter provided is not valid.
            return null;
        }

        /// <summary>
        /// Gets the GUID of the supplied entry.
        /// </summary>
        /// <param name="entry">The entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUID(SearchResultEntry entry)
        {
            if (entry != null)
            {
                try
                {
                    return new Guid(ldap.GetByteAttributeValue("objectGUID", entry));
                }
                catch
                {
                    // There was an error constructing the GUID.
                    return Guid.Empty;
                }
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the GUID of the entry with the supplied attribute value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to search.</param>
        /// <param name="attributeValue">The attribute value of the entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUIDByAttribute(string attributeName, string attributeValue)
        {
            if (!string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(attributeValue))
            {
                SearchResultEntry entry = GetEntryByAttribute(attributeName, attributeValue, new List<string>() { "objectGUID" });
                if (entry != null)
                {
                    return GetGUID(entry);
                }
                else
                {
                    return Guid.Empty;
                }
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the GUID of the entry with the supplied employee number.
        /// </summary>
        /// <param name="employeeNumber">The employee number of the entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUIDByEmployeeNumber(string employeeNumber)
        {
            return GetGUIDByAttribute("employeeNumber", employeeNumber);
        }

        /// <summary>
        /// Gets the GUID of the entry with the supplied SAM account name.
        /// </summary>
        /// <param name="sAMAccountName">The SAM account name of the entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUIDBySAMAccountName(string sAMAccountName)
        {
            return GetGUIDByAttribute("sAMAccountName", sAMAccountName);
        }

        /// <summary>
        /// Gets the GUID of the entry with the supplied distinguished name.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUIDByDistinguishedName(string dn)
        {
            return GetGUIDByAttribute("distinguishedName", dn);
        }

        /// <summary>
        /// Gets the GUID of the entry with the supplied common name.
        /// </summary>
        /// <param name="cn">The common name of the entry to get the GUID of.</param>
        /// <returns>The GUID of the entry, or an Empty GUID if it could not be found, or there was an error retrieving it.</returns>
        public Guid GetGUIDByCommonName(string cn)
        {
            return GetGUIDByAttribute("cn", cn);
        }

        /// <summary>
        /// Gets a string with the name of a User Account Control flag given its value.
        /// </summary>
        /// <param name="uac">The value of the User Account Control flag.</param>
        /// <returns>The name of the flag.</returns>
        static public string GetUserAccountControlName(UserAccountControl uac)
        {
            switch (uac)
            {
                case UserAccountControl.Accountdisable:
                    return "ACCOUNTDISABLE";
                case UserAccountControl.DontExpirePassword:
                    return "DONT_EXPIRE_PASSWORD";
                case UserAccountControl.DontReqPreauth:
                    return "DONT_REQ_PREAUTH";
                case UserAccountControl.EncryptedTextPwdAllowed:
                    return "ENCRYPTED_TEXT_PWD_ALLOWED";
                case UserAccountControl.HomedirRequired:
                    return "HOMEDIR_REQUIRED";
                case UserAccountControl.InterdomainTrustAccount:
                    return "INTERDOMAIN_TRUST_ACCOUNT";
                case UserAccountControl.Lockout:
                    return "LOCKOUT";
                case UserAccountControl.MNSLogonAccount:
                    return "MNS_LOGON_ACCOUNT";
                case UserAccountControl.NormalAccount:
                    return "NORMAL_ACCOUNT";
                case UserAccountControl.NotDelegated:
                    return "NOT_DELEGATED";
                case UserAccountControl.PartialSecretsAccount:
                    return "PARTIAL_SECRETS_ACCOUNT";
                case UserAccountControl.PasswdCantChange:
                    return "PASSWD_CANT_CHANGE";
                case UserAccountControl.PasswdNotreqd:
                    return "PASSWD_NOTREQD";
                case UserAccountControl.PasswordExpired:
                    return "PASSWORD_EXPIRED";
                case UserAccountControl.Script:
                    return "SCRIPT";
                case UserAccountControl.ServerTrustAccount:
                    return "SERVER_TRUST_ACCOUNT";
                case UserAccountControl.SmartcardRequired:
                    return "SMARTCARD_REQUIRED";
                case UserAccountControl.TempDuplicateAccount:
                    return "TEMP_DUPLICATE_ACCOUNT";
                case UserAccountControl.TrustedForDelgation:
                    return "TRUSTED_FOR_DELEGATION";
                case UserAccountControl.TrustedToAuthForDelegation:
                    return "TRUSTED_TO_AUTH_FOR_DELEGATION";
                case UserAccountControl.UseAESKeys:
                    return "USE_AES_KEYS";
                case UserAccountControl.UseDESKeyOnly:
                    return "USE_DES_KEY_ONLY";
                case UserAccountControl.WorkstationTrustAccount:
                    return "WORKSTATION_TRUST_ACCOUNT";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Moves and / or renames an object in Active Directory.
        /// </summary>
        /// <param name="objectGuid">The GUID of the object to move and / or rename.</param>
        /// <param name="newParentObjectGuid">(Optional: Required only if moving) The GUID of the new parent object for the object (if moving).</param>
        /// <param name="newCommonName">(Optional: Required only if renaming) The new common name (if renaming).</param>
        /// <returns>True if the object was moved or renamed, false otherwise.</returns>
        public bool MoveRenameObject(Guid objectGuid, Guid? newParentObjectGuid = null, string newCommonName = null)
        {
            if (objectGuid != Guid.Empty)
            {
                // Get the object that corresponds with the GUID supplied.
                try
                {
                    ActiveDirectoryObject obj = new ActiveDirectoryObject(this, objectGuid);

                    ActiveDirectoryObject parentObj;
                    if (newParentObjectGuid != null && newParentObjectGuid != Guid.Empty)
                    {
                        // We're moving.
                        parentObj = new ActiveDirectoryObject(this, newParentObjectGuid.Value);

                    }
                    else
                    {
                        // Set the parent object to the current parent of the object supplied.
                        parentObj = new ActiveDirectoryObject(this, GetGUIDByDistinguishedName(obj.OrganizationalUnit));
                    }

                    string commonName;
                    if (!string.IsNullOrWhiteSpace(newCommonName))
                    {
                        // We're renaming the object.
                        commonName = newCommonName;
                    }
                    else
                    {
                        // Set the common name to the object's existing common name.
                        commonName = obj.CommonName;
                    }

                    // Move and / or rename the object in Active Directory.
                    return ldap.MoveRenameEntry(obj.DistinguishedName, parentObj.DistinguishedName, "CN=" + commonName);
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the object that is the base for all searches within Active Directory.
        /// This only needs to be set if you need to search somewhere other than the base of the directory.
        /// </summary>
        /// <param name="distinguishedName">The distinguished name of the object where searches will begin. (Typically an OU or the base DN of the directory.)</param>
        /// <returns>True if the search base was set, false otherwise.</returns>
        public bool SetSearchBase(string distinguishedName)
        {
            return ldap.SetSearchBaseAndScope(distinguishedName);
        }

        /// <summary>
        /// Tests whether an integer contains a UserAccountControl flag.
        /// </summary>
        /// <param name="accountControlValue">The integer to test.</param>
        /// <param name="flag">The UserAccountControl flag to look for.</param>
        static public bool UserAccountControlContains(long accountControlValue, UserAccountControl flag)
        {
            if ((accountControlValue & (int)flag) == (int)flag)
            {
                // The account control value contains the flag.
                return true;
            }
            else
            {
                // The account control value does not contain the flag.
                return false;
            }
        }
    }
}
