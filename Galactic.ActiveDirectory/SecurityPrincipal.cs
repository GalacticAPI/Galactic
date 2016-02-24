using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Text.RegularExpressions;

namespace Galactic.ActiveDirectory
{
    public class SecurityPrincipal : ActiveDirectoryObject, IComparable<SecurityPrincipal>, IEqualityComparer<SecurityPrincipal>
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The list of specific attributes that should be retrieved when searching for the entry in AD. The attributes of parent objects should be included as well.
        /// </summary>
        static protected new string[] AttributeNames = { "mail", "proxyAddresses", "mailNickname", "targetAddress", "userPrincipalName", "employeeNumber", "badPwdCount", "badPasswordTime", "division", "employeeID", "givenName", "msIIS-FTPDir", "msIIS-FTPRoot", "homeDirectory", "homeDrive", "wWWHomePage", "sn", "scriptPath", "manager", "pwdLastSet", "streetAddress", "title", "msDS-User-Account-Control-Computed", "userAccountControl", "sAMAccountName", "objectSid" };

        // ----- VARIABLES -----


        // ----- PROPERTIES -----

        /// <summary>
        /// A description of the security principal.
        /// </summary>
        public string Description
        {
            get
            {
                return GetStringAttributeValue("description");
            }
            set
            {
                SetStringAttribute("description", value);
            }
        }

        /// <summary>
        /// The principal's e-mail address.
        /// </summary>
        public string EMailAddress
        {
            get
            {
                return GetStringAttributeValue("mail");
            }
            set
            {
                SetStringAttribute("mail", value);
            }
        }

        /// <summary>
        /// A list of the principal's e-mail addresses.
        /// </summary>
        public List<string> EmailAddresses
        {
            get
            {
                // The list of e-mail addresses to return.
                List<string> smtpAddresses = new List<string>();

                // Get the attribute from Active Directory.
                List<string> attribute = GetStringAttributeValues("proxyAddresses");

                // Check that proxy addresses has values in it.
                if (attribute != null)
                {
                    if (attribute.Count > 0)
                    {
                        // The attribute has values.

                        // Create a list to return with only SMTP entries.
                        foreach (string address in attribute)
                        {
                            if (address.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
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
                        string mailProperty = GetStringAttributeValue("mail");

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
        }

        /// <summary>
        /// The principal's Microsoft Exchange Alias.
        /// </summary>
        public string ExchangeAlias
        {
            get
            {
                return GetStringAttributeValue("mailNickname");
            }
        }

        /// <summary>
        /// The distinguished names of groups that this principal is a member of.
        /// </summary>
        public List<string> Groups
        {
            get
            {
                return GetStringAttributeValues("memberOf");
            }
        }

        /// <summary>
        /// Indicates if this principal is a Group.
        /// </summary>
        public bool IsGroup
        {
            get
            {
                // Check whether the class of this object in Active Directory is a group.
                if (SchemaClasses.Contains("group"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Indicates if this principal is a User.
        /// </summary>
        public bool IsUser
        {
            get
            {
                // Check whether the class of this object in Active Directory is a user.
                if (SchemaClasses.Contains("user"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The principal's primary e-mail address.
        /// </summary>
        public string PrimaryEmailAddress
        {
            get
            {
                // Get the attribute from Active Directory.
                List<string> attribute = GetStringAttributeValues("proxyAddresses");

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
                        string mailProperty = GetStringAttributeValue("mail");

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
        }

        /// <summary>
        /// The principal's SAM Account Name.
        /// </summary>
        public string SAMAccountName
        {
            get
            {
                return GetStringAttributeValue("sAMAccountName");
            }
            set
            {
                SetStringAttribute("sAMAccountName", value);
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
                return GetStringAttributeValue("targetAddress");
            }
            set
            {
                SetStringAttribute("targetAddress", value);
            }
        }

        /// <summary>
        /// The User Principal Name of the principal.
        /// </summary>
        public string UserPrincipalName
        {
            get
            {
                return GetStringAttributeValue("userPrincipalName");
            }
            set
            {
                SetStringAttribute("userPrincipalName", value);
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a security principal object from Active Directory with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory object used to query and manipulate the security principal.</param>
        /// <param name="guid">The GUID of the security principal</param>
        public SecurityPrincipal(ActiveDirectory ad, Guid guid)
            : base(ad, guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                // Add atttributes relevant to security principals to the list of base attributes
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
                else if (guid == Guid.Empty)
                {
                    throw new ArgumentException("guid");
                }
            }
        }

        /// <summary>
        /// Gets a security principal object from a supplied search result entry.
        /// </summary>
        /// <param name="ad">An Active Directory object used to manipulate the security principal.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public SecurityPrincipal(ActiveDirectory ad, SearchResultEntry entry)
            : base(ad, entry)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Adds a proxy e-mail address to the account's proxyAddresses field.
        /// </summary>
        /// <param name="emailAddress">The address in standard e-mail format (username@domain.com)</param>
        /// <param name="asPrimary">If the address should be added as the primary proxy address.</param>
        /// <returns>True if the address was added, false otherwise.</returns>
        public bool AddProxyAddress(string emailAddress, bool asPrimary = false)
        {
            // Create a regular expression to verify that a properly formated e-mail address was provided.
            Regex emailFormat = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            if (!string.IsNullOrWhiteSpace(emailAddress) && emailFormat.IsMatch(emailAddress))
            {
                // Create the address in proxy address format.
                string proxyAddressToAdd = "";
                // Check whether this address should be added as the primary address.
                if (asPrimary)
                {
                    // Check that there is not a primary address currently associated with this account.
                    if (string.IsNullOrWhiteSpace(PrimaryEmailAddress))
                    {
                        // Create the address as a primary proxy address.
                        proxyAddressToAdd = "SMTP:" + emailAddress;
                    }
                    else
                    {
                        // A primary address already exists.
                        return false;
                    }
                }
                else
                {
                    // Create the address as a normal proxy address.
                    proxyAddressToAdd = "smtp:" + emailAddress;
                }

                // Get the proxyAddresses from Active Directory.
                List<string> proxyAddresses = GetStringAttributeValues("proxyAddresses");

                // Check that proxy addresses has values in it.
                if (proxyAddresses != null)
                {
                    if (proxyAddresses.Count > 0)
                    {
                        // There are values in the proxyAddresses attribute.

                        // If the address doesn't already exist in the list. Add it.
                        if (!proxyAddresses.Contains(proxyAddressToAdd))
                        {
                            proxyAddresses.Add(proxyAddressToAdd);

                            // Set the modifications to the proxyAddress field in the account's AD object.
                            return SetAttribute("proxyAddresses", proxyAddresses.ToArray());
                        }
                        else
                        {
                            // The address already exists in the list. Return true, since it's already there.
                            return true;
                        }
                    }
                }
                // Proxy addresses has no values.
                // Create a new object array with the address in it.
                proxyAddresses = new List<string> { proxyAddressToAdd };

                // Set the modifications to the proxyAddress field in the account's AD object.
                return SetAttribute("proxyAddresses", proxyAddresses.ToArray());
            }
            else
            {
                // An e-mail address was not supplied or is inproperly formatted.
                return false;
            }
        }

        /// <summary>
        /// Adds this principal to the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to add the principal to.</param>
        /// <returns>True if the principal was added, false otherwise.</returns>
        public bool AddToGroup(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                Group group = new Group(AD, guid);
                return group.AddMembers(new List<SecurityPrincipal>() { this });
            }
            return false;
        }

        /// <summary>
        /// Checks if this principal is a member of the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all parent groups that this principal might be a member of.</param>
        /// <returns>True if the principal is a member, false otherwise.</returns>
        public bool MemberOfGroup(Guid guid, bool recursive)
        {
            if (guid != Guid.Empty)
            {
                Group group = new Group(AD, guid);
                if (!recursive)
                {
                    List<string> dns = GetStringAttributeValues("memberOf");
                    return dns.Contains(group.DistinguishedName);
                }
                else
                {
                    foreach (SecurityPrincipal member in group.Members)
                    {
                        if (member.IsGroup)
                        {
                            // If the member is a group, do a recursive lookup of that group as well.
                            if (MemberOfGroup(member.GUID, true))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            // If the member is not a group, check if it's the GUID we're looking for.
                            if (GUID == member.GUID)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Moves and / or renames this object.
        /// </summary>
        /// <param name="newParentObjectGuid">(Optional: Required only if moving) The GUID of the new parent object for the object (if moving).</param>
        /// <param name="newSAMAccountName">(Optional: Required only if renaming) The new SAM Account Name (if renaming).</param>
        /// <returns>True if the object was moved or renamed, false otherwise.</returns>
        public override bool MoveRename(Guid? newParentObjectGuid = null, string newSAMAccountName = null)
        {
            // Get old values necessary for replacement during the move / rename process.
            string oldSAMAccountName = SAMAccountName;

            // Move / rename the object.
            if (base.MoveRename(newParentObjectGuid, newSAMAccountName))
            {
                // The security principal was moved or renamed in AD.
                if (!string.IsNullOrWhiteSpace(newSAMAccountName))
                {
                    // This is a rename, update all necessary attributes with the new name.

                    // Update the SAMAccountName
                    SAMAccountName = newSAMAccountName;

                    // Update the user principal name.
                    UserPrincipalName = UserPrincipalName.Replace(oldSAMAccountName, SAMAccountName);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a proxy e-mail address from the account's proxyAddresses field.
        /// </summary>
        /// <param name="emailAddress">The address in standard e-mail format (username@domain.com)</param>
        /// <returns>True if the address was removed or not found, false otherwise.</returns>
        public bool RemoveProxyAddress(string emailAddress)
        {
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                // Create the address in proxy address format.
                string proxyAddressToRemove = "smtp:" + emailAddress;
                string primaryProxyAddressToRemove = "SMTP:" + emailAddress;

                // Get a list of existing proxy addresses.
                List<string> proxyAddresses = GetStringAttributeValues("proxyAddresses");

                // Check that proxy addresses has values in it.
                if (proxyAddresses != null)
                {
                    if (proxyAddresses.Count > 0)
                    {
                        // There are existing proxy addresses.

                        // Check whether the address to add already exists in the list.
                        if (proxyAddresses.Contains(proxyAddressToRemove))
                        {
                            // Remove the existing entry.
                            proxyAddresses.Remove(proxyAddressToRemove);
                            return SetAttribute("proxyAddresses", proxyAddresses.ToArray());
                        }
                        else
                        {
                            // Check whether the proxy address to remove is a primary.
                            if (proxyAddresses.Contains(primaryProxyAddressToRemove))
                            {
                                // Remove the existing entry.
                                proxyAddresses.Remove(primaryProxyAddressToRemove);
                                return SetAttribute("proxyAddresses", proxyAddresses.ToArray());
                            }

                            // This account did not have an e-mail address matching the one desired.
                            return true;
                        }
                    }
                    else
                    {
                        // Proxy addresses did not contain the e-mail address because it is empty.
                        return true;
                    }
                }
                else
                {
                    // There was an error retrieving the proxy addresses from the directory, or some other unspecified error.
                    return false;
                }
            }
            else
            {
                // An e-mail address was not supplied.
                return false;
            }
        }

        /// <summary>
        /// Sets the supplied e-mail address to be the primary e-mail address for receiving mail.
        /// Note: This e-mail address must already be associated with the account.
        /// If there is account currently has a primary e-mail address, it will be set as a secondary.
        /// </summary>
        /// <param name="emailAddress">The e-mail address to make primary.</param>
        /// <returns>Returns true if the e-mail address was made the primary, false if the e-mail address supplied was not already associated with the account,
        /// or the address could not be made primary for any reason.</returns>
        public bool SetPrimaryProxyAddress(string emailAddress)
        {
            // Check if an e-mail address was supplied.
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                // An e-mail address was supplied.
                // Check if the e-mail is already associated with the account.
                if (EmailAddresses.Contains(emailAddress))
                {
                    // The e-mail address is associated with the account.
                    // Get the current primary address (if there is one).
                    string currentPrimary = PrimaryEmailAddress;

                    // Check that a current primary address was retrieved.
                    if (!string.IsNullOrWhiteSpace(currentPrimary))
                    {
                        // The current primary address was retrieved.
                        // Remove the current primary address from the account.
                        if (RemoveProxyAddress(currentPrimary))
                        {
                            // The primary was removed.
                            // Add the primary back as a normal e-mail address.
                            if (!AddProxyAddress(currentPrimary))
                            {
                                // There was an error adding the primary back.
                                return false;
                            }
                        }
                    }

                    // Remove the e-mail address to promote it to primary.
                    if (RemoveProxyAddress(emailAddress))
                    {
                        // The address was removed.
                        // Add the e-mail address back as the primary.
                        return AddProxyAddress(emailAddress, true);
                    }
                }
            }
            // An e-mail address was not supplied or there was an error setting the primary address.
            return false;
        }

        /// <summary>
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first SecurityPrincipal to check.</param>
        /// <param name="y">The second SecurityPrincipal to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(SecurityPrincipal x, SecurityPrincipal y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Generates a hash code for the SecurityPrincipal supplied.
        /// </summary>
        /// <param name="obj">The SecurityPrincipal to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(SecurityPrincipal obj)
        {
            return GetHashCode((ActiveDirectoryObject)obj);
        }

        /// <summary>
        /// Compares this SecurityPrincipal to another SecurityPrincipal.
        /// </summary>
        /// <param name="other">The other SecurityPrincipal to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(SecurityPrincipal other)
        {
            return CompareTo((ActiveDirectoryObject)other);
        }
    }
}
