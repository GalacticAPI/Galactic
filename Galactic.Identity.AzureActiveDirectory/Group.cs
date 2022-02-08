﻿using GraphGroup = Microsoft.Graph.Group;
using GraphUser = Microsoft.Graph.User;

namespace Galactic.Identity.AzureActiveDirectory
{
    public class Group : Identity.Group
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        protected AzureActiveDirectoryClient aad = null;

        protected GraphGroup graphGroup = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// All users that are a member of this group or a subgroup (recursive).
        /// </summary>
        public override List<Identity.User> AllUserMembers
        {
            get
            {
                return aad.GetUserMembers(UniqueId, true);
            }
        }

        /// <summary>
        /// UPNs of all users that are members of this group or a subgroup (recursive).
        /// </summary>
        public override List<string> AllUserMemberNames
        {
            get
            {
                List<string> names = new List<string>();

                List<Microsoft.Graph.DirectoryObject> members = aad.GetMembers(UniqueId, true);

                foreach (Microsoft.Graph.DirectoryObject member in members)
                {
                    if (member.ODataType == "#microsoft.graph.user")
                    {
                        GraphUser user = (GraphUser)member;

                        names.Add(user.UserPrincipalName);
                    }
                }

                return names;
            }
        }

        /// <summary>
        /// Groups that are a member of the group.
        /// </summary>
        public override List<Identity.Group> GroupMembers
        {
            get
            {
                return aad.GetGroupMembers(UniqueId, false);
            }
        }

        /// <summary>
        /// Names of groups that are a member of the group.
        /// </summary>
        public override List<string> GroupMemberNames
        {
            get
            {
                List<string> names = new List<string>();

                foreach (Microsoft.Graph.DirectoryObject member in DirectoryObjectMembers)
                {
                    if (member.ODataType == "#microsoft.graph.group")
                    {
                        GraphGroup group = (GraphGroup)member;

                        names.Add(group.DisplayName);
                    }
                }

                return names;
            }
        }

        /// <summary>
        /// The members of the group.
        /// </summary>
        public override List<IdentityObject> Members
        {
            get
            {
                List <IdentityObject> temp = new ();
                temp.AddRange(UserMembers);
                temp.AddRange(GroupMembers);


                return temp;
            }
        }

        /// <summary>
        /// The names (group) or UPN (user) of the members of the group.
        /// </summary>
        public override List<string> MemberNames
        {
            get
            {
                List<string> names = new List<string> ();

                //List<Microsoft.Graph.DirectoryObject> results = aad.GetMembers(UniqueId, false);

                foreach(Microsoft.Graph.DirectoryObject member in DirectoryObjectMembers)
                {
                    if (member.ODataType == "#microsoft.graph.user")
                    {
                        GraphUser user = (GraphUser)member;

                        names.Add(user.UserPrincipalName);
                    }
                    else if(member.ODataType == "#microsoft.graph.group")
                    {
                        GraphGroup group = (GraphGroup)member;

                        names.Add(group.DisplayName);
                    }
                }

                return names;
            }
        }

        /// <summary>
        /// The group's name.
        /// </summary>
        public override string Name
        {
            get => DisplayName;
            set => DisplayName = value;
        }

        /// <summary>
        /// Users that are a member of the group. (Not including subgroups.)
        /// </summary>
        public override List<Identity.User> UserMembers
        {
            get
            {
                return aad.GetUserMembers(UniqueId, false);
            }
        }

        /// <summary>
        /// UPNs of users that are members of the group. (Not including subgroups.)
        /// </summary>
        public override List<string> UserMemberNames
        {
            get
            {
                List<string> names = new List<string>();

                foreach (Microsoft.Graph.DirectoryObject member in DirectoryObjectMembers)
                {
                    if (member.ODataType == "#microsoft.graph.user")
                    {
                        GraphUser user = (GraphUser)member;

                        names.Add(user.UserPrincipalName);
                    }
                }

                return names;
            }
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        [DirectorySystemPropertyName("createdDateTime")]
        public override DateTime? CreationTime
        {
            get
            {
                return graphGroup.CreatedDateTime.Value.DateTime;
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public override List<Identity.Group> Groups
        {
            get
            {
                return aad.GetGroupMembership(UniqueId, true);
            }
        }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public override string Type
        {
            get
            {
                return "Group";
            }
        }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        [DirectorySystemPropertyName("id")]
        public override string UniqueId
        {
            get
            {
                return graphGroup.Id;
            }
        }

        /// <summary>
        /// A description of the object.
        /// </summary>
        [DirectorySystemPropertyName("description")]
        public override string Description
        {
            get
            {
                return graphGroup.Description;
            }
            set
            {
                GraphGroup group = new()
                {
                    Description = value
                };

                aad.UpdateGroup(UniqueId, group);
            }
        }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [DirectorySystemPropertyName("displayName")]
        public string DisplayName
        {
            get
            {
                return graphGroup.DisplayName;
            }
            set
            {
                GraphGroup group = new()
                {
                    DisplayName = value
                };

                aad.UpdateGroup(UniqueId, group);
            }
        }

        /// <summary>
        /// Primary email alias of group.
        /// </summary>
        [DirectorySystemPropertyName("mail")]
        public string PrimaryEmailAddress
        {
            get
            {
                return graphGroup.Mail;
            }
            set
            {

            }
        }

        /// <summary>
        /// All proxy addresses on the group.
        /// </summary>
        [DirectorySystemPropertyName("proxyAddresses")]
        public List<string> EmailAddresses
        {
            get
            {
                return (List<string>)graphGroup.ProxyAddresses;
            }
            set
            {

            }
        }

        /// <summary>
        /// True if group is mail enabled, otherwise false.
        /// </summary>
        [DirectorySystemPropertyName("mailEnabled")]
        public bool MailEnabled
        {
            get
            {
                return (bool)graphGroup.MailEnabled;
            }
        }

        /// <summary>
        /// The mail nickname of the group.
        /// </summary>
        [DirectorySystemPropertyName("mailNickname")]
        public string MailNickname
        {
            get
            {
                return graphGroup.MailNickname;
            }
            set
            {
                GraphGroup group = new()
                {
                    MailNickname = value
                };

                aad.UpdateGroup(UniqueId, group);
            }
        }

        /// <summary>
        /// True if group is security enabled, otherwise false.
        /// </summary>
        [DirectorySystemPropertyName("securityEnabled")]
        public bool SecurityEnabled
        {
            get
            {
                return (bool)graphGroup.SecurityEnabled;
            }
        }

        /// <summary>
        /// Visibility of the group.
        /// </summary>
        [DirectorySystemPropertyName("visibility")]
        public string Visability
        {
            get
            {
                return graphGroup.Visibility;
            }
        }

        /// <summary>
        /// A hashed version of the group's Name to allow for faster compare operations.
        /// </summary>
        public override int HashedIdentifier
        {
            get
            {
                return Name.GetHashCode();
            }
        }

        // ----- Private Properties -----
        private List<Microsoft.Graph.DirectoryObject> DirectoryObjectMembers
        {
            get
            {
                return aad.GetMembers(UniqueId, false);
            }
        }

        // ----- CONSTRUCTORS -----

        public Group(AzureActiveDirectoryClient aad, GraphGroup graphGroup)
        {
            if (aad != null && graphGroup != null)
            {
                // Initialize the client.
                this.aad = aad;

                // Initialize the source GraphUser data.
                this.graphGroup = graphGroup;
            }
            else
            {
                if (aad == null)
                {
                    throw new ArgumentNullException(nameof(aad));
                }
                else
                {
                    throw new ArgumentNullException(nameof(graphGroup));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Refreshes group properties with new data.
        /// </summary>
        public void Refresh()
        {
            graphGroup = aad.GetGraphGroup(UniqueId);
        }

        /// <summary>
        /// Adds members to the group.
        /// </summary>
        /// <param name="members">The members to add.</param>
        /// <returns>True if the members were added, false otherwise.</returns>
        public override bool AddMembers(List<IdentityObject> members)
        {
            if(members != null)
            {
                foreach(IdentityObject member in members)
                {
                    // Skip non-AAD IdentityObjects.
                    if (member is Group || member is User)
                    {
                        if (!aad.AddObjectToGroup(member.UniqueId, UniqueId))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public override List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes)
        {
            List<IdentityAttribute<bool>> results = new List<IdentityAttribute<bool>>();

            foreach(var attribute in attributes)
            {
                results.Add(new IdentityAttribute<bool>(attribute.Name, aad.UpdateGroup(UniqueId, new List<IdentityAttribute<object>> { attribute })));
            }

            return results;
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
                foreach (IdentityObject member in members)
                {
                    // Skip non-AAD members.
                    if (member is Group || member is User)
                    {
                        if (!aad.DeleteObjectFromGroup(member.UniqueId, UniqueId))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}

