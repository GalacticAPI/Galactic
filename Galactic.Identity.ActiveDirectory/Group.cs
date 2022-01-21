using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Versioning;

namespace Galactic.Identity.ActiveDirectory
{
    /// <summary>
    /// Group is a class that allows for the query and manipulation of
    /// Active Directory group objects.
    /// </summary>
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class Group : Identity.Group, ISecurityPrincipal
    {

        // ----- CONSTANTS -----

        /// <summary>
        /// The default location for groups to be created in Active Directory.
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
        public string OrganizationalUnit
        {
            get
            {
                string ou = DistinguishedName;
                if (!string.IsNullOrWhiteSpace(ou))
                {
                    string[] ouComponents = ou.Split(',');
                    return ou.Substring(ouComponents[0].Length + 1);
                }
                else
                {
                    return null;
                }
            }
        }

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
        public override string Description
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
        public List<string> EmailAddresses
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
                        ad.RemoveProxyAddress(, address);
                    }

                    // Add addresses that are new. Those in value but not in current addresses.
                    foreach (string address in value.Except(currentAddresses))
                    {
                        AddProxyAddress(address);
                    }

                    // Set the primary e-mail address.
                    if (value.Count > 0)
                    {
                        SetPrimaryProxyAddress(value[0]);
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
        public string PrimaryEmailAddress
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
                    AddProxyAddress(value);
                }
                // Set the address as primary.
                SetPrimaryProxyAddress(value);
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
        /// Does a recursive lookup to find all users that are a member of this
        /// group by way of subgroup membership.
        /// </summary>
        public override List<Identity.User> AllUserMembers
        {
            get
            {
                List<Identity.User> users = new();
                List<IdentityObject> members = Members;

                // Check that there are members in the group.
                if (members != null)
                {
                    // There are members in the group.
                    foreach (IdentityObject member in members)
                    {
                        // Check if the member is a user.
                        if (member is User)
                        {
                            // Add the user to the list.
                            users.Add((User)member);
                        }
                        else
                        {
                            // The member is a subgroup. Do a recursive lookup for more users.
                            List<Identity.User> subGroupUsers = ((Group)member).AllUserMembers;

                            // Add any users found in the subgroup to the list.
                            users.AddRange(subGroupUsers);
                        }
                    }
                }
                return users;
            }
        }

        /// <summary>
        /// Groups that are members of the group.
        /// </summary>
        public override List<Identity.Group> GroupMembers
        {
            get
            {
                List<Identity.Group> groups = new();
                List<IdentityObject> members = Members;

                // Check if there are members in the group.
                if (members != null)
                {
                    // There are members.
                    foreach (IdentityObject member in members)
                    {
                        // Check if the member is a group.
                        if (member is Group)
                        {
                            // The member is a group. Add it to the list.
                            groups.Add((Group)member);
                        }
                    }
                }
                return groups;
            }
        }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public override List<IdentityObject> Members
        {
            get
            {
                List<string> dns = ad.GetStringAttributeValues("member", entry);
                if (dns != null)
                {
                    // Get a list of objects by their distinguished names.
                    List<IdentityObject> objs = new();
                    foreach (string dn in dns)
                    {
                        // Get the GUID of the member.
                        Guid memberGuid = ad.GetGuidByDistinguishedName(dn);

                        // Verify that a GUID was returned.
                        if (memberGuid != Guid.Empty)
                        {
                            // Check which type of object the member is, and add it as the correct type to the list.
                            if (ad.IsGroup(memberGuid))
                            {
                                // The member is a group.
                                objs.Add(new Group(ad, memberGuid));
                            }
                            else if (ad.IsUser(memberGuid))
                            {
                                // The member is a user.
                                objs.Add(new User(ad, memberGuid));
                            }
                        }
                    }
                    return objs;
                }
                else
                {
                    // A list of distinguished names was not returned. Return an empty list.
                    return new();
                }
            }
        }

        /// <summary>
        /// The number of members in the group. 
        /// </summary>
        public override int MemberCount
        {
            get
            {
                List<string> dns = ad.GetStringAttributeValues("member", entry);
                if (dns != null)
                {
                    //Count number of DNs listed.
                    return dns.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// The type or category of the Group. Empty if unknown.
        /// </summary>
        public override string Type
        {
            get
            {
                // Get the attribute value and convert it to a usable enum.
                byte[] groupBits = ad.GetByteAttributeValue("groupType", entry);
                uint groupTypeNum = BitConverter.ToUInt32(groupBits);
                ActiveDirectoryClient.GroupType groupType = (ActiveDirectoryClient.GroupType)groupTypeNum;

                // Check if the flag is set for various types of groups.
                if (groupType.HasFlag(ActiveDirectoryClient.GroupType.DomainLocal))
                {
                    return "DomainLocal";
                }
                else if (groupType.HasFlag(ActiveDirectoryClient.GroupType.Global))
                {
                    return "Global";
                }
                else if (groupType.HasFlag(ActiveDirectoryClient.GroupType.Security))
                {
                    return "Security";
                }
                else if (groupType.HasFlag(ActiveDirectoryClient.GroupType.Universal))
                {
                    return "Universal";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Users who are members of the group.
        /// </summary>
        public override List<Identity.User> UserMembers
        {
            get
            {
                List<Identity.User> users = new();
                List<IdentityObject> members = Members;

                // Check if there are members in the group.
                if (members != null)
                {
                    // There are members.
                    foreach (IdentityObject member in members)
                    {
                        // Check if the member is a user.
                        if (member is User)
                        {
                            // The member is a user. Add them to the list.
                            users.Add((User)member);
                        }
                    }
                }
                return users;
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a group object from Active Directory with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory client used to query and manipulate the directory object.</param>
        /// <param name="guid">The GUID of the user.</param>
        public Group(ActiveDirectoryClient ad, Guid guid)
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
                else if (guid == Guid.Empty)
                {
                    throw new ArgumentException("guid");
                }
            }
        }

        /// <summary>
        /// Gets a group object from a supplied search result entry.
        /// </summary>
        /// <param name="ad">An Active Directory client used to manipulate the group.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public Group(ActiveDirectoryClient ad, SearchResultEntry entry)
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
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public override bool AddMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                List<string> dns = new List<string>();
                foreach (IdentityObject member in members)
                {
                    if (member is Group || member is User)
                    {
                        dns.Add((member as ISecurityPrincipal).DistinguishedName);
                    }
                }
                return ad.SetMultiValueAttribute(ref entry, "member", dns.ToArray());
            }
            else
            {
                // No members were supplied.
                return false;
            }
        }

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public override bool ClearMembership()
        {
            return ad.DeleteAttribute(ref entry, "member");
        }

        /// <summary>
        /// Creates a new group within Active Directory given its proposed name, the distinguished name of the OU to place it in, and other optional attributes.
        /// </summary>
        /// <param name="ad">An Active Directory client used to create the group.</param>
        /// <param name="sAMAccountName">The proposed SAM Account name for the group.</param>
        /// <param name="ouDn">The distinguished name for the OU to place the group within.</param>
        /// <param name="type">A uint from the ActiveDirectory.GroupType enum representing the type of group to create.</param>
        /// <param name="additionalAttributes">Optional: Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object.</returns>
        static public Group Create(ActiveDirectoryClient ad, string sAMAccountName, string ouDn, uint type, List<DirectoryAttribute> additionalAttributes = null)
        {
            // Check that an active directory instance, SAM Account name, and distinguished name of the OU to
            // place the group within are provided.
            if (ad != null && !String.IsNullOrWhiteSpace(sAMAccountName) && !String.IsNullOrWhiteSpace(ouDn))
            {
                // Checks whether the group name supplied is valid. Otherwise, throws an exception.
                if (!ActiveDirectoryClient.IsGroupNameValid(ouDn))
                {
                    // The group name isn't valid.
                    throw new ArgumentException("The SAM Account Name provided is not a valid group name.");
                }

                // Check that the OU exists in Active Directory.
                if (ad.GetEntryByDistinguishedName(ouDn) == null)
                {
                    // The OU does not exist in Active Directory.
                    throw new ArgumentException("The OU provided does not exist in Active Directory.");
                }

                // Create the group in Active Directory and return its object.
                string groupDn = "CN=" + sAMAccountName + "," + ouDn;
                List<DirectoryAttribute> attributes = new List<DirectoryAttribute>
                {
                    new DirectoryAttribute("objectClass", "group"),
                    new DirectoryAttribute("sAMAccountName", sAMAccountName),
                    new DirectoryAttribute("groupType", BitConverter.GetBytes(type))
                };
                if (additionalAttributes != null)
                {
                    // Only add non conflicting attributes.
                    foreach (DirectoryAttribute attribute in additionalAttributes)
                    {
                        if (attribute.Name != "objectClass" && attribute.Name != "sAMAccountName" && attribute.Name != "groupType")
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
                if (ad.Add(groupDn, attributes.ToArray()))
                {
                    // The group was created. Retrieve it from Active Directory.
                    return new Group(ad, ad.GetGuidByDistinguishedName(groupDn));
                }
                else
                {
                    // There was a problem creating the group in Active Directory.
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
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.
        /// If a returned value is null, there was an error retrieving that value.</returns>
        public override List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            return ad.GetAttributes(names, entry);
        }

        /// <summary>
        /// Removes identity objects from the group.
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public override bool RemoveMembers(List<IdentityObject> members)
        {
            if (members != null)
            {
                List<string> dns = new List<string>();
                foreach (IdentityObject member in members)
                {
                    if (member is Group || member is User)
                    {
                        dns.Add((member as ISecurityPrincipal).DistinguishedName);
                    }
                }
                return ad.DeleteAttribute(ref entry, "member", dns.ToArray());
            }
            else
            {
                // No members were supplied.
                return false;
            }
        }
    }
}
