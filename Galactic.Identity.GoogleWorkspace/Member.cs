using GoogleMember = Google.Apis.Admin.Directory.directory_v1.Data.Member;

namespace Galactic.Identity.GoogleWorkspace
{
    /// <summary>
    /// An object that represents a member of a Google Workspace group.
    /// </summary>
    public class Member
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Email property name.
        /// </summary>
        public const string EMAIL = "email";

        /// <summary>
        /// Id property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Kind property name.
        /// </summary>
        public const string KIND = "kind";

        /// <summary>
        /// Role property name.
        /// </summary>
        public const string ROLE = "role";

        /// <summary>
        /// Type property name.
        /// </summary>
        public const string TYPE = "type";

        // Roles.

        /// <summary>
        /// This role is only available if the Google Workspace is enabled using the Admin console. A MANAGER role can do everything
        /// done by an OWNER role except make a member an OWNER or delete the group. A group can have multiple OWNER and MANAGER members.
        /// </summary>
        public const string ROLE_MANAGER = "MANAGER";

        /// <summary>
        /// This role can subscribe to a group, view discussion archives, and view the group's membership list.
        /// </summary>
        public const string ROLE_MEMBER = "MEMBER";

        /// <summary>
        /// This role can change send message to the group, add or remove members, change member roles, change group's settings,
        /// and delete the group. An OWNER must be a group member.
        /// </summary>
        public const string ROLE_OWNER = "OWNER";

        // Types.

        /// <summary>
        /// The member is another group.
        /// </summary>
        public const string TYPE_GROUP = "GROUP";

        /// <summary>
        /// The member is a user.
        /// </summary>
        public const string TYPE_USER = "USER";

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Google.
        /// </summary>
        protected GoogleWorkspaceClient gws = null;

        /// <summary>
        /// The backing native data representing the Member in Google Workspace.
        /// </summary>
        protected GoogleMember member = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// The group member's e-mail address.
        /// </summary>
        [DirectorySystemPropertyName(EMAIL)]
        public string Email
        {
            get => member.Email;
            set => member.Email = value;
        }

        /// <summary>
        /// The group member's unique ID.
        /// </summary>
        [DirectorySystemPropertyName(ID)]
        public string Id
        {
            get => member.Id;
            set => member.Id = value;
        }

        /// <summary>
        /// The kind of object the member is in Google Workspace.
        /// </summary>
        [DirectorySystemPropertyName(KIND)]
        public string Kind => member.Kind;

        /// <summary>
        /// The native GoogleMember object representing the Member in Google Workspace.
        /// </summary>
        public GoogleMember MemberObject => member;

        /// <summary>
        /// The role of the member within the group.
        /// </summary>
        [DirectorySystemPropertyName(ROLE)]
        public string Role
        {
            get => member.Role;
            set => member.Role = value;
        }

        /// <summary>
        /// The type of object the member is. (Group or User)
        /// </summary>
        [DirectorySystemPropertyName(TYPE)]
        public string MemberType
        {
            get => member.Type;
            set => member.Type = value;
        }


        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a Google Workspace member.
        /// </summary>
        /// <param name="gws">A Google Workspace client object used to query and manipulate the member.</param>
        /// <param name="member">(Optional) A Google Workspace native object representing this member's properties.
        /// A new native object is created if not provided.</param>
        public Member(GoogleWorkspaceClient gws, GoogleMember member = null)
        {
            if (gws != null)
            {
                // Initialize the client.
                this.gws = gws;

                // Initialize the member data from the native object supplied.
                if (member != null)
                {
                    // Use the supplied member.
                    this.member = member;
                }
                else
                {
                    // Create a new member object.
                    this.member = new();
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(gws));
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Converts a list of native backing member objects, to Member objects.
        /// </summary>
        /// <param name="gws">A Google Workspace client object used to query and manipulate the members.</param>
        /// <param name="members">The list of native backing member objects.</param>
        /// <returns>A list of Member objects backed by the supplied native objects. Null if the list couldn't be created.</returns>
        public static List<Member> FromGoogleMembers(GoogleWorkspaceClient gws, IList<GoogleMember> googleMembers)
        {
            List<Member> members = new();
            if (gws != null && googleMembers != null)
            {
                foreach (GoogleMember googleMember in googleMembers)
                {
                    members.Add(new Member(gws, googleMember));
                }
            }
            else
            {
                // The Google Workspace client, or list of members were not defined.
                members = null;
            }

            return members;
        }

        /// <summary>
        /// Create an IIdentityObject for this Member.
        /// </summary>
        /// <returns></returns>
        public IdentityObject ToIdentityObject()
        {
            if (MemberType == TYPE_USER)
            {
                List<Identity.User> users = gws.GetUsersByAttribute(new(User.PRIMARY_EMAIL, Email));
                if (users != null && users.Count == 1)
                {
                    return users[0];
                }
                else
                {
                    // The member couldn't be converted.
                    return null;
                }
            }
            else if (MemberType == TYPE_GROUP)
            {
                List<Identity.Group> groups = gws.GetGroupsByAttribute(new(Group.EMAIL, Email));
                if (groups != null && groups.Count == 1)
                {
                    return groups[0];
                }
                else
                {
                    // The member couldn't be converted.
                    return null;
                }
            }
            else
            {
                // The member couldn't be converted.
                return null;
            }
        }
    }
}
