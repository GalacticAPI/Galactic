using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Galactic.Identity.ActiveDirectory
{
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class SecurityPrincipal : ActiveDirectoryObject, IComparable<SecurityPrincipal>, IDescriptionSupportedObject, IEqualityComparer<SecurityPrincipal>, IIdentityObject, IMailSupportedObject
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The list of specific attributes that should be retrieved when searching for the entry in AD. The attributes of parent objects should be included as well.
        /// </summary>
        static protected new string[] AttributeNames = { "mail", "proxyAddresses", "mailNickname", "targetAddress", "userPrincipalName", "employeeNumber", "badPwdCount", "badPasswordTime", "division", "employeeID", "givenName", "msIIS-FTPDir", "msIIS-FTPRoot", "homeDirectory", "homeDrive", "wWWHomePage", "sn", "scriptPath", "manager", "pwdLastSet", "streetAddress", "title", "msDS-User-Account-Control-Computed", "userAccountControl", "sAMAccountName", "objectSid" };

        // ----- VARIABLES -----


        // ----- PROPERTIES -----

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => CreateTimeStamp;

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
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        List<string> IMailSupportedObject.EmailAddresses
        {
            get
            {
                // Create a list of e-mail addresses to return.
                List<string> emailAddresses = new();

                // Get the principal's primary e-mail address.
                string primaryEmailAddress = PrimaryEmailAddress;

                // Get other e-mail addresses associated with the principal.
                List<string> otherEmailAddresses = EmailAddresses;

                // Remove the primary e-mail address from the list of other addresses.
                otherEmailAddresses.Remove(primaryEmailAddress);

                // Create the list with the primary first.
                emailAddresses.Add(primaryEmailAddress);
                emailAddresses.AddRange(otherEmailAddresses);

                // Return the list.
                return emailAddresses;
                
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
                        RemoveProxyAddress(address);
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
        public string ExchangeAlias
        {
            get
            {
                return GetStringAttributeValue("mailNickname");
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        List<IGroup> IIdentityObject.Groups
        {
            get
            {
                List<string> groupDns = Groups;
                if (groupDns != null)
                {
                    List<IGroup> groups = new();
                    foreach (string groupDn in groupDns)
                    {
                        try
                        {
                            groups.Add(new Group(AD, AD.GetGUIDByDistinguishedName(groupDn)));
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
        /// The object's primary e-mail address.
        /// </summary>
        string IMailSupportedObject.PrimaryEmailAddress
        {
            get
            {
                return PrimaryEmailAddress;
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
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        virtual public string Type
        {
            get
            {
                if (IsGroup)
                {
                    return "group";
                }
                else if (IsUser)
                {
                    return "user";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// The object's unique ID in the system. (GUID)
        /// </summary>
        public string UniqueId
        {
            get
            {
                return GUID.ToString();
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
        /// <param name="ad">An Active Directory client used to query and manipulate the security principal.</param>
        /// <param name="guid">The GUID of the security principal</param>
        public SecurityPrincipal(ActiveDirectoryClient ad, Guid guid)
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
        public SecurityPrincipal(ActiveDirectoryClient ad, SearchResultEntry entry)
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
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 iif the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public int CompareTo(IIdentityObject other)
        {
            return ((IIdentityObject)this).CompareTo(other);
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

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            return ((IIdentityObject)this).Equals(x, y);
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
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.
        /// If a returned value is null, there was an error retrieving that value.</returns>
        public List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            List<IdentityAttribute<object>> attributes = new();
            if (names != null)
            {
                foreach (string name in names)
                {
                    // TODO: See if its possible to determine type via the AD Schema.
                    // Try getting the value by each possible type it could be.
                    object value = GetStringAttributeValue(name);
                    if (value == null)
                    {
                        // It wasn't a string.
                        value = GetStringAttributeValues(name);
                        if (value == null)
                        {
                            // It wasn't a list of strings.
                            value = GetByteAttributeValue(name);
                            if (value == null)
                            {
                                // It wasn't a byte.
                                value = GetByteAttributeValues(name);
                                if (value == null)
                                {
                                    // It wasn't a byte array.
                                    value = GetIntervalAttributeValue(name);
                                }
                            }
                        }
                    }

                    // Add the attribute and it's value.
                    attributes.Add(new(name, value));
                }
            }

            // Return the list of attributes.
            return attributes;
        }

        /// <summary>
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public int GetHashCode(IIdentityObject obj) => IIdentityObject.GetHashCode(obj);

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
        /// Checks if this principal is a member of the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this principal might be a member of.</param>
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
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            return ((IIdentityObject)this).MemberOfGroup(group, recursive);
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
        /// Removes this principal from the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to add the principal to.</param>
        /// <returns>True if the principal was removed, false otherwise.</returns>
        public bool RemoveFromGroup(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                Group group = new Group(AD, guid);
                return group.RemoveMembers(new List<SecurityPrincipal>() { this });
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
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<object>> attributes)
        {
            if (attributes != null)
            {
                // Create the list of results attributes to return.
                List<IdentityAttribute<bool>> resultAttributes = new();

                // Iterate over all the attributes and set their values.
                foreach (IdentityAttribute<object> attribute in attributes)
                {
                    if (attribute.Value is string)
                    {
                        // The value is a string, set it using the string version of the method.
                        bool result = SetStringAttribute(attribute.Name, attribute.Value as string);
                        resultAttributes.Add(new(attribute.Name, result));
                    }
                    else if (attribute.Value is IEnumerable<object>)
                    {
                        // The value is a collection of objects. Use the default method.
                        bool result = SetAttribute(attribute.Name, attribute.Value as object[]);
                        resultAttributes.Add(new(attribute.Name, result));
                    }
                }

                // Return the results.
                return resultAttributes;
            }
            else
            {
                // The list of attributes was null.
                throw new ArgumentNullException(nameof(attributes));
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
    }
}
