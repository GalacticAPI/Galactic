using Galactic.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Versioning;

namespace Galactic.ActiveDirectory
{
    /// <summary>
    /// Group is a class that allows for the query and manipulation of
    /// Active Directory group objects.
    /// </summary>
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class Group : SecurityPrincipal, IEnumerable<SecurityPrincipal>, IGroup
    {

        // ----- CONSTANTS -----

        /// <summary>
        /// The list of specific attributes that should be retrieved when searching for the entry in AD. The attributes of parent objects should be included as well.
        /// </summary>
        static protected new string[] AttributeNames = { "department", "member", "groupType" };

        /// <summary>
        /// The default location for groups to be created in Active Directory.
        /// </summary>
        public const string DEFAULT_CREATE_PATH = "CN=Users";

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup.
        /// </summary>
        List<IUser> IGroup.AllUserMembers
        {
            get
            {
                return AllUserMembers.ConvertAll<IUser>(user => user);
            }
        }

        /// <summary>
        /// Does a recursive lookup to find all users that are a member of this
        /// group by way of subgroup membership.
        /// </summary>
        public List<User> AllUserMembers
        {
            get
            {
                List<User> users = new List<User>();
                List<SecurityPrincipal> members = Members;

                // Check that there are members in the group.
                if (members != null)
                {
                    // There are members in the group.
                    foreach (SecurityPrincipal member in members)
                    {
                        // Check if the member is a user.
                        if (member.IsUser)
                        {
                            // Get the user object from the principal's GUID.
                            User user = new User(AD, member.GUID);

                            // Add the user to the list.
                            users.Add(user);
                        }
                        else
                        {
                            // The members is a subgroup. Do a recursive lookup for more users.
                            Group subGroup = new Group(AD, member.GUID);
                            List<User> subGroupUsers = subGroup.AllUserMembers;

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
        public List<Group> GroupMembers
        {
            get
            {
                List<Group> groups = new List<Group>();
                List<SecurityPrincipal> members = Members;

                // Check if there are security principals in the group.
                if (members != null)
                {
                    // There are members.
                    foreach (SecurityPrincipal member in members)
                    {
                        // Check if the security principal is a group.
                        if (member.IsGroup)
                        {
                            // The principal is a group. Add it to the list.
                            groups.Add(new Group(AD, member.GUID));
                        }
                    }
                }
                return groups;
            }
        }

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        List<IGroup> IGroup.GroupMembers
        {
            get
            {
                return GroupMembers.ConvertAll<IGroup>(group => group);
            }
        }

        /// <summary>
        /// The members of the group.
        /// </summary>
        List<IIdentityObject> IGroup.Members
        {
            get
            {
                return Members.ConvertAll<IIdentityObject>(principal => principal);
            }
        }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public List<SecurityPrincipal> Members
        {
            get
            {
                List<string> dns = GetStringAttributeValues("member");
                if (dns != null)
                {
                    // Get a list of security principals by their distinguished names.
                    List<SecurityPrincipal> securityPrincipals = new List<SecurityPrincipal>();
                    foreach (string dn in dns)
                    {
                        securityPrincipals.Add(new SecurityPrincipal(AD, AD.GetGUIDByDistinguishedName(dn)));
                    }
                    return securityPrincipals;
                }
                else
                {
                    return new List<SecurityPrincipal>();
                }
            }
        }

        /// <summary>
        /// The number of members in the group. 
        /// </summary>
        public int MemberCount
        {
            get
            {
                List<string> dns = GetStringAttributeValues("member");
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
                byte[] groupBits = GetByteAttributeValue("groupType");
                uint groupTypeNum = BitConverter.ToUInt32(groupBits);
                ActiveDirectory.GroupType groupType = (ActiveDirectory.GroupType)groupTypeNum;

                // Check if the flag is set for various types of groups.
                if (groupType.HasFlag(ActiveDirectory.GroupType.DomainLocal))
                {
                    return "DomainLocal";
                }
                else if (groupType.HasFlag(ActiveDirectory.GroupType.Global))
                {
                    return "Global";
                }
                else if (groupType.HasFlag(ActiveDirectory.GroupType.Security))
                {
                    return "Security";
                }
                else if (groupType.HasFlag(ActiveDirectory.GroupType.Universal))
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
        public List<User> UserMembers
        {
            get
            {
                List<User> users = new List<User>();
                List<SecurityPrincipal> members = Members;

                // Check if there are members in the group.
                if (members != null)
                {
                    // There are members.
                    foreach (SecurityPrincipal member in members)
                    {
                        // Check if the principal is a user.
                        if (member.IsUser)
                        {
                            // The principal is a user. Add them to the list.
                            users.Add(new User(AD, member.GUID));
                        }
                    }
                }
                return users;
            }
        }

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        List<IUser> IGroup.UserMembers
        {
            get
            {
                return UserMembers.ConvertAll<IUser>(user => user);
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a group object from Active Directory with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory object used to query and manipulate the directory object.</param>
        /// <param name="guid">The GUID of the user.</param>
        public Group(ActiveDirectory ad, Guid guid)
            : base(ad, guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                // Add atttributes relevant to groups to the list of base attributes
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
        /// Gets a group object from a supplied search result entry.
        /// </summary>
        /// <param name="ad">An Active Directory object used to manipulate the group.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public Group(ActiveDirectory ad, SearchResultEntry entry)
            : base(ad, entry)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public bool AddMembers(List<IIdentityObject> members)
        {
            if (members != null)
            {
                List<SecurityPrincipal> securityPrincipals = new();
                foreach (IIdentityObject member in members)
                {
                    securityPrincipals.Add(member as SecurityPrincipal);
                }
                return AddMembers(securityPrincipals);
            }
            else
            {
                // No members were supplied.
                return false;
            }
        }

        /// <summary>
        /// Adds security principals to the group.
        /// </summary>
        /// <param name="principals">The principals to add.</param>
        /// <returns>True if the principals were added, false otherwise.</returns>
        public bool AddMembers(List<SecurityPrincipal> principals)
        {
            if (principals != null)
            {
                List<string> dns = new List<string>();
                foreach (SecurityPrincipal principal in principals)
                {
                    dns.Add(principal.DistinguishedName);
                }
                return SetAttribute("member", dns.ToArray());
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clears all members from this group.
        /// </summary>
        /// <returns>True if all members were cleared, false otherwise.</returns>
        public bool ClearMembership()
        {
            return DeleteAttribute("member");
        }


        /// <summary>
        /// Compares this Group to another Group.
        /// </summary>
        /// <param name="other">The other Group to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(Group other)
        {
            return CompareTo((ActiveDirectoryObject)other);
        }

        /// <summary>
        /// Creates a new group within Active Directory given its proposed name, the distinguished name of the OU to place it in, and other optional attributes.
        /// </summary>
        /// <param name="ad">An Active Directory object used to create the group.</param>
        /// <param name="sAMAccountName">The proposed SAM Account name for the group.</param>
        /// <param name="ouDn">The distinguished name for the OU to place the group within.</param>
        /// <param name="type">A uint from the ActiveDirectory.GroupType enum representing the type of group to create.</param>
        /// <param name="additionalAttributes">Optional: Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object.</returns>
        static public Group Create(ActiveDirectory ad, string sAMAccountName, string ouDn, uint type, List<DirectoryAttribute> additionalAttributes = null)
        {
            // Check that an active directory instance, SAM Account name, and distinguished name of the OU to
            // place the group within are provided.
            if (ad != null && !String.IsNullOrWhiteSpace(sAMAccountName) && !String.IsNullOrWhiteSpace(ouDn))
            {
                // Checks whether the group name supplied is valid. Otherwise, throws an exception.
                if (!ActiveDirectory.IsGroupNameValid(ouDn))
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
                    return new Group(ad, ad.GetGUIDByDistinguishedName(groupDn));
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
        /// Deletes a group from Active Directory.
        /// </summary>
        /// <param name="ad">An Active Directory object used to delete the group.</param>
        /// <param name="guid">The GUID of the group.</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        static public bool Delete(ActiveDirectory ad, Guid guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                return ad.Delete(guid);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first Group to check.</param>
        /// <param name="y">The second Group to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(Group x, Group y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator<IIdentityObject> IEnumerable<IIdentityObject>.GetEnumerator()
        {
            return ((IGroup)this).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<SecurityPrincipal> GetEnumerator()
        {
            foreach (SecurityPrincipal member in Members)
            {
                yield return member;
            }
        }

        /// <summary>
        /// Generates a hash code for the Group supplied.
        /// </summary>
        /// <param name="obj">The Group to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(Group obj)
        {
            return GetHashCode((ActiveDirectoryObject)obj);
        }

        /// <summary>
        /// Removes a security principal from the group.
        /// </summary>
        /// <param name="principal">The principal to remove.</param>
        /// <returns>True if the principal was removed, false otherwise.</returns>
        public bool RemoveMember(SecurityPrincipal principal)
        {
            return RemoveMembers(new List<SecurityPrincipal> { principal });
        }

        /// <summary>
        /// Removes identity objects from the group.
        /// </summary>
        /// <param name="members">The objects to remove.</param>
        /// <returns>True if the objects were removed, false otherwise.</returns>
        public bool RemoveMembers(List<IIdentityObject> members)
        {
            if (members != null)
            {
                List<SecurityPrincipal> securityPrincipals = new();
                foreach (IIdentityObject member in members)
                {
                    securityPrincipals.Add(member as SecurityPrincipal);
                }
                return RemoveMembers(securityPrincipals);
            }
            else
            {
                // No members were supplied.
                return false;
            }
        }

        /// <summary>
        /// Removes security principals from the group.
        /// </summary>
        /// <param name="principals">The principals to remove.</param>
        /// <returns>True if the principals were removed, false otherwise.</returns>
        public bool RemoveMembers(List<SecurityPrincipal> principals)
        {
            if (principals != null)
            {
                List<string> dns = new List<string>();
                foreach (SecurityPrincipal principal in principals)
                {
                    dns.Add(principal.DistinguishedName);
                }
                return DeleteAttribute("member", dns.ToArray());
            }
            return false;
        }
    }
}
