using Galactic.Cryptography;
using GalFile = Galactic.FileSystem.File;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
using GoogleMember = Google.Apis.Admin.Directory.directory_v1.Data.Member;
using GoogleUser = Google.Apis.Admin.Directory.directory_v1.Data.User;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Security.Cryptography;

namespace Galactic.Identity.GoogleWorkspace
{
    /// <summary>
    /// GoogleWorkspaceClient is a class that allows for the query and manipulation of Google Workspace objects.
    /// </summary>
    public class GoogleWorkspaceClient : IDirectorySystem
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The name of the SHA-1 algorithm when used in Google Workspace calls.
        /// </summary>
        private static string SHA1_ALGORITHM_NAME = "SHA-1";

        /// <summary>
        /// The kind of object that groups are designated as in Google Workspace.
        /// </summary>
        private static string GROUP_KIND = "directory#groups";

        // ----- VARIABLES -----

        /// <summary>
        /// The type of search operators supported by Google Workspace.
        /// </summary>
        public enum SearchOperatorType
        {
            Exact,
            Contains,
            Starts,
            Range,
            Greater,
            GreaterEqual,
            Less,
            LessEqual
        }

        // ----- PROPERTIES -----

        /// <summary>
        /// The FQDN of the Google Workspace domain to make requests against.
        /// </summary>
        private string Domain { get; set; }

        /// <summary>
        /// The service that provides API access to the directory.
        /// </summary>
        private DirectoryService Service { get; init; }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Inititalizes a client that can make requests to the Google Workspace Admin SDK.
        /// </summary>
        /// <param name="credential">The credentials the client should use when authenticating.</param>
        /// <param name="applicationName">The name of the application making directory requests.</param>
        /// <param name="domain">The Google domain name to make requests against. This is usually the FQDN of the Google Workspace domain.</param>
        public GoogleWorkspaceClient(GoogleCredential credential, string applicationName, string domain)
        {
            if (credential != null && !string.IsNullOrWhiteSpace(applicationName) && !string.IsNullOrWhiteSpace(domain))
            {
                Service = new DirectoryService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
                Domain = domain;
            }
            else
            {
                if (credential == null)
                {
                    throw new ArgumentNullException(nameof(credential));
                }
                else
                {
                    throw new ArgumentNullException(nameof(applicationName));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Builds an OAuth2.0 serivce account based credential for use when authenticating the client.
        /// </summary>
        /// <param name="path">The path to the JSON credential file provided by Google for the service account.</param>
        /// <param name="user">(Optional) A user to impersonate when making client calls.</param>
        /// <param name="scopes">(Optional) A list of OAuth 2.0 scopes that should be used to request access by the client to the API.
        /// These can be a set from the properties of the DirectoryApiScopes record in this namespace, or directly via those provided by Google's DirectoryService.Scope class
        /// or as outlined at https://developers.google.com/identity/protocols/oauth2/scopes#admin-directory. </param>
        /// <returns>A GoogleCredential constructed from the supplied parameters.</returns>
        public static GoogleCredential BuildServiceAccountCredential (string path, string user = null, List<string> scopes = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                // Verify that the file exists.
                if (GalFile.Exists(path))
                {
                    // Create an object to access the file.
                    GalFile credsFile = new GalFile(path, false, true);

                    // Create and config the credential.
                    ServiceAccountCredential serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(credsFile.FileStream);
                    GoogleCredential credential = GoogleCredential.FromServiceAccountCredential(serviceAccountCredential);
                    
                    // If an impersonation user is supplied, add it to the credential.
                    if (!string.IsNullOrWhiteSpace(user))
                    {
                        credential = credential.CreateWithUser(user);
                    }

                    // If scopes are provided, add it to the credential.
                    if (scopes != null && scopes.Count > 0)
                    {
                        credential = credential.CreateScoped(scopes);
                    }

                    // Return the constructed credential.
                    return credential;
                }
                else
                {
                    // The credential file doesn't exist.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
        /// Note: In Google Workspace, an e-mail address is required. An attribute with the name of the constant Group.EMAIL must be provided as an additional
        /// attribute in order to successfully create the group.
        /// </summary>
        /// <param name="name">The proposed name of the group. (Google: The group's display name.)</param>
        /// <param name="type">(Ignored) The type of group to create.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Required, see above.) Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object, or null if it could not be created.</returns>
        /// <exception cref="ArgumentException">Thrown if an attribute with the name of the constant Group.EMAIL is not supplied.</exception>
        public IGroup CreateGroup (string name, string type, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            if (!string.IsNullOrWhiteSpace(name) && additionalAttributes != null)
            {
                // Gather the additional attributes supplied.
                string email = "";
                string description = "";
                foreach (IdentityAttribute<object> attribute in additionalAttributes)
                {
                    if (attribute.Name == Group.EMAIL)
                    {
                        email = (string)attribute.Value;
                    }
                    if (attribute.Name == Group.DESCRIPTION)
                    {
                        description = (string)attribute.Value;
                    }
                }
                
                // Verify that an email attribute was supplied.
                if (!string.IsNullOrWhiteSpace(email))
                {
                    // Create an object with the properties for the new group.
                    GoogleGroup group = new();
                    group.Name = name;
                    group.Email = email;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        group.Description = description;
                    }

                    // Perform the Google Workspace API request.
                    GroupsResource.InsertRequest request = Service.Groups.Insert(group);
                    GoogleGroup createdGroup = null;
                    try
                    {
                        createdGroup = request.Execute();
                    }
                    catch
                    {
                        // There was an error creating the group.
                    }

                    // Return the created group.
                    if (createdGroup != null)
                    {
                        return new Group(this, createdGroup);
                    }
                    else
                    {
                        // The group was not created, return null.
                        return null;
                    }
                }
                else
                {
                    // An email address attribute was not supplied.
                    throw new ArgumentException(nameof(additionalAttributes));
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                else
                {
                    throw new ArgumentNullException(nameof(additionalAttributes));
                }
            }
        }

        /// <summary>
        /// Creates a user within the directory system given its login, and other optional attributes.
        /// Note: In Google Workspace, an password is required. An attribute with the name of the constant User.PASSWORD must be provided as an additional
        /// attribute in order to successfully create the user. All password values are sent in requests via SHA-1 hashes. 
        /// </summary>
        /// <param name="login">The proposed login of the user. (Google: The user's primary e-mail address.)</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Required, see above.) Additional attributes to set when creating the user.</param>
        /// <returns>The newly creaTed user object, or null if it could not be created.</returns>
        public IUser CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            if (!string.IsNullOrWhiteSpace(login) && additionalAttributes != null)
            {
                // Gather the additional attributes supplied.
                string password = "";
                foreach (IdentityAttribute<object> attribute in additionalAttributes)
                {
                    if (attribute.Name == User.PASSWORD)
                    {
                        password = (string)attribute.Value;
                    }
                    // TODO: Add support for additional attributes here.
                }

                // Verify that a password attribute was supplied.
                if (!string.IsNullOrWhiteSpace(password))
                {
                    // Create an object with the properties for the new user.
                    GoogleUser user = new();
                    user.PrimaryEmail = login;
                    user.Password = Hash.GetHash(password, HashAlgorithmName.SHA1);
                    user.HashFunction = SHA1_ALGORITHM_NAME;

                    // Perform the Google Workspace API request.
                    UsersResource.InsertRequest request = Service.Users.Insert(user);
                    GoogleUser createdUser = null;
                    try
                    {
                        createdUser = request.Execute();
                    }
                    catch
                    {
                        // There was an error creating the user.
                    }

                    // Return the created user.
                    if (createdUser != null)
                    {
                        return new User(this, createdUser);
                    }
                    else
                    {
                        // The user was not created, return null.
                        return null;
                    }
                }
                else
                {
                    // A password attribute was not supplied.
                    throw new ArgumentException(nameof(additionalAttributes));
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(login))
                {
                    throw new ArgumentNullException(nameof(login));
                }
                else
                {
                    throw new ArgumentNullException(nameof(additionalAttributes));
                }
            }
        }

        /// <summary>
        /// Deletes a group with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the group to delete. (Google: Group's id property.)</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        public bool DeleteGroup(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // Perform the Google Workspace API request.
                GroupsResource.DeleteRequest request = Service.Groups.Delete(uniqueId);
                try
                {
                    request.Execute();
                }
                catch
                {
                    // There was an error deleting the group.
                    return false;
                }

                // Return that the group was deleted.
                return true;
                
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Deletes a user with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to delete.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        public bool DeleteUser(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // Perform the Google Workspace API request.
                UsersResource.DeleteRequest request = Service.Users.Delete(uniqueId);
                try
                {
                    request.Execute();
                }
                catch
                {
                    // There was an error deleting the user.
                    return false;
                }

                // Return that the user was deleted.
                return true;

            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Gets all groups in the directory system.
        /// </summary>
        /// <returns>A list of all groups in the directory system.</returns>
        public List<IGroup> GetAllGroups()
        {
            // Create a list of groups to return.
            List<IGroup> groups = new();

            try
            {
                // Create a request to retrieve all the groups and execute it.
                GroupsResource.ListRequest request = Service.Groups.List();
                request.Domain = Domain;
                Groups groupsRequest = request.Execute();
                IList<GoogleGroup> requestGroups = groupsRequest.GroupsValue;
                
                // Verify that groups were returned by the request.
                if (requestGroups != null)
                {
                    // Iterate over all the groups and populate the return list.
                    foreach (GoogleGroup requestGroup in requestGroups)
                    {
                        groups.Add(new Group(this, requestGroup));
                    }
                }
            }
            catch
            {
                // There was an error retrieving the groups.
            }

            // Return the list of groups.
            return groups;
        }

        /// <summary>
        /// Get's all users in the directory system.
        /// </summary>
        /// <returns>A list of all users in the directory system.</returns>
        public List<IUser> GetAllUsers()
        {
            // Create a list of users to return.
            List<IUser> users = new();

            try
            {
                // Create a request to retrieve all the users and execute it.
                UsersResource.ListRequest request = Service.Users.List();
                request.Domain = Domain;
                Users usersRequest = request.Execute();
                IList<GoogleUser> requestUsers = usersRequest.UsersValue;

                // Verify that users were returned by the request.
                if (requestUsers != null)
                {
                    // Iterate over all the users and populate the return list.
                    foreach (GoogleUser requestUser in requestUsers)
                    {
                        users.Add(new User(this, requestUser));
                    }
                }
            }
            catch
            {
                // There was an error retrieving the users.
            }

            // Return the list of users.
            return users;
        }

        /// <summary>
        /// Gets a list of identity objects that are a member of the supplied group.
        /// </summary>
        /// <param name="groupKey">The group's e-mail address or unique id.</param>
        /// <returns>A list of IIdentityObjects representing each user and group that is a member of the group.</returns>
        public List<IIdentityObject> GetGroupMembership(string groupKey)
        {
            if (!string.IsNullOrWhiteSpace(groupKey))
            {
                // Create a list of identity objects to return.
                List<IIdentityObject> identityObjects = new();

                try
                {
                    // Create a request to retrieve all the group members and execute it.
                    MembersResource.ListRequest request = Service.Members.List(groupKey);
                    Members membersRequest = request.Execute();
                    IList<GoogleMember> requestMembers = membersRequest.MembersValue;

                    // Verify that members were returned by the request.
                    if (requestMembers != null)
                    {
                        // Convert the members.
                        List<Member> members = Member.FromGoogleMembers(this, requestMembers);

                        // Create the list of IIdentityObjects.
                        foreach (Member member in members)
                        {
                            IIdentityObject obj = member.ToIIdentityObject();
                            if (obj != null)
                            {
                                identityObjects.Add(obj);
                            }
                        }
                    }
                }
                catch
                {
                    // There was an error retrieving the members.
                }

                // Return the list of identity objects.
                return identityObjects;
            }
            else
            {
                throw new ArgumentNullException(nameof(groupKey));
            }
        }

        /// <summary>
        /// Gets IGroups that start with the attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Note: Currently ignored.) (Optional) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of groups that match the attribute value supplied.</returns>
        public List<IGroup> GetGroupsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<object>> returnedAttributes = null)
        {
            // Create a list of groups to return.
            List<IGroup> groups = new();

            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name) && attribute.Value != null)
            {
                try
                {
                    // Create a request to retrieve all the groups and execute it.
                    GroupsResource.ListRequest request = Service.Groups.List();
                    request.Domain = Domain;
                    // Construct the query based upon the attribute supplied and the search operators supported.
                    string searchFieldName = "";
                    switch (attribute.Name)
                    {
                        case Group.ALIASES:
                            searchFieldName = Group.SEARCH_EMAIL;
                            break;
                        case Group.EMAIL:
                            searchFieldName = Group.SEARCH_EMAIL;
                            break;
                        case Group.NAME:
                            searchFieldName = Group.SEARCH_NAME;
                            break;
                        default:
                            break;
                    }

                    // Determine whether the attribute value requires quotes around it.
                    bool quotesRequired = false;
                    if (attribute.Value.Where(Char.IsWhiteSpace).Count() > 0)
                    {
                        // There is whitespace in the string.
                        quotesRequired = true;
                    }

                    // Build the correct query string based on the available search options for each field.
                    if (Group.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Starts))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + ":'" + attribute.Value.Trim() + "*'";
                        }
                        else
                        {
                            request.Query = searchFieldName + ":" + attribute.Value.Trim() + "*";
                        }
                    }
                    else if (Group.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Exact))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + "='" + attribute.Value.Trim() + "'";
                        }
                        else
                        {
                            request.Query = searchFieldName + "=" + attribute.Value.Trim();
                        }
                    }
                    else if (Group.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Contains))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + ":'" + attribute.Value.Trim() + "'";
                        }
                        else
                        {
                            request.Query = searchFieldName + ":" + attribute.Value.Trim();
                        }
                    }

                    // TODO: Allow for returning specified fields via returnedAttributes.
                    // request.Fields = "";
                    Groups groupsRequest = request.Execute();
                    IList<GoogleGroup> requestGroups = groupsRequest.GroupsValue;

                    // Verify that groups were returned by the request.
                    if (requestGroups != null)
                    {
                        // Iterate over all the groups and populate the return list.
                        foreach (GoogleGroup requestGroup in requestGroups)
                        {
                            groups.Add(new Group(this, requestGroup));
                        }
                    }
                }
                catch
                {
                    // There was an error retrieving the groups.
                }
            }

            // Return the list of groups.
            return groups;
        }

        /// <summary>
        /// Gets a list of the types of groups supported by the directory system.
        /// </summary>
        /// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
        public List<string> GetGroupTypes()
        {
            return new() { GROUP_KIND };
        }

        /// <summary>
        /// Gets a list of groups that the member is a member of.
        /// </summary>
        /// <param name="memberKey">A unique ID, primary e-mail address or alias of a user or group that is a member of a group.</param>
        /// <returns>A list of IGroups the member is a member of, or null if there was an error retrieving the list.</returns>
        public List<IGroup> GetMemberGroups(string memberKey)
        {
            if (!string.IsNullOrWhiteSpace(memberKey))
            {
                // Create the list of groups to return.
                List<IGroup> groups = new();

                try
                {

                    // Create a request to retrieve the groups a member belongs to and execute it.
                    GroupsResource.ListRequest request = Service.Groups.List();
                    request.UserKey = memberKey;
                    Groups groupsRequest = request.Execute();
                    IList<GoogleGroup> requestGroups = groupsRequest.GroupsValue;

                    // Verify that groups were returned by the request.
                    if (requestGroups != null)
                    {
                        // Iterate over all the groups and populate the return list.
                        foreach (GoogleGroup requestGroup in requestGroups)
                        {
                            groups.Add(new Group(this, requestGroup));
                        }
                    }
                }
                catch
                {
                    // There was an error retrieving members.
                }

                // Return the list of groups.
                return groups;
            }
            else
            {
                throw new ArgumentNullException(nameof(memberKey));
            }
        }

        /// <summary>
        /// Gets IUsers that start with the attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Note: Currently ignored.) (Optional) The attributes that should be returned in the user found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of users that match the attribute value supplied.</returns>
        public List<IUser> GetUsersByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<object>> returnedAttributes = null)
        {
            // Create a list of users to return.
            List<IUser> users = new();

            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name) && attribute.Value != null)
            {
                try
                {
                    // Create a request to retrieve all the users and execute it.
                    UsersResource.ListRequest request = Service.Users.List();
                    request.Domain = Domain;
                    // Construct the query based upon the attribute supplied and the search operators supported.
                    string searchFieldName = "";
                    switch (attribute.Name)
                    {
                        case User.ADDRESSES:
                            searchFieldName = User.SEARCH_ADDRESS;
                            break;
                        case User.ADDRESSES_COUNTRY_CODE:
                            searchFieldName = User.SEARCH_ADDRESS_COUNTRY;
                            break;
                        case User.ADDRESSES_LOCALITY:
                            searchFieldName = User.SEARCH_ADDRESS_LOCALITY;
                            break;
                        case User.ADDRESSES_POSTAL_CODE:
                            searchFieldName = User.SEARCH_ADDRESS_POSTAL_CODE;
                            break;
                        case User.ADDRESSES_REGION:
                            searchFieldName = User.SEARCH_ADDRESS_REGION;
                            break;
                        case User.ALIASES:
                            searchFieldName = User.SEARCH_EMAIL;
                            break;
                        case User.EMAILS:
                            searchFieldName = User.SEARCH_EMAIL;
                            break;
                        case User.EXTERNAL_IDS:
                            searchFieldName = User.SEARCH_EXTERNAL_ID;
                            break;
                        case User.IMS:
                            searchFieldName = User.SEARCH_IM;
                            break;
                        case User.IS_ENFORCED_IN_2_SV:
                            searchFieldName = User.SEARCH_IS_ENFORCED_IN_2_SV;
                            break;
                        case User.IS_ENROLLED_IN_2_SV:
                            searchFieldName = User.SEARCH_IS_ENROLLED_IN_2_SV;
                            break;
                        case User.IS_ADMIN:
                            searchFieldName = User.SEARCH_IS_ADMIN;
                            break;
                        case User.IS_DELEGATED_ADMIN:
                            searchFieldName = User.SEARCH_IS_DELEGATED_ADMIN;
                            break;
                        case User.NAME:
                            searchFieldName = User.SEARCH_NAME;
                            break;
                        case User.NAME_FAMILY_NAME:
                            searchFieldName = User.SEARCH_FAMILY_NAME;
                            break;
                        case User.NAME_GIVEN_NAME:
                            searchFieldName = User.SEARCH_GIVEN_NAME;
                            break;
                        case User.ORG_UNIT_PATH:
                            searchFieldName = User.SEARCH_ORG_UNIT_PATH;
                            break;
                        case User.ORGANIZATIONS:
                            searchFieldName = User.SEARCH_ORG_NAME;
                            break;
                        case User.ORGANIZATIONS_DEPARTMENT:
                            searchFieldName = User.SEARCH_ORG_DEPARTMENT;
                            break;
                        case User.ORGANIZATIONS_TITLE:
                            searchFieldName = User.SEARCH_ORG_TITLE;
                            break;
                        case User.PHONES:
                            searchFieldName = User.SEARCH_PHONE;
                            break;
                        case User.PRIMARY_EMAIL:
                            searchFieldName = User.SEARCH_EMAIL;
                            break;
                        case User.RECOVERY_EMAIL:
                            searchFieldName = User.SEARCH_EMAIL;
                            break;
                        case User.RECOVERY_PHONE:
                            searchFieldName = User.SEARCH_PHONE;
                            break;
                        case User.SUSPENDED:
                            searchFieldName = User.SEARCH_IS_SUSPENDED;
                            break;
                        default:
                            break;
                    }

                    // Determine whether the attribute value requires quotes around it.
                    bool quotesRequired = false;
                    if (!attribute.Value.All(Char.IsLetterOrDigit))
                    {
                        // There is a non-letter or digit in the string.
                        quotesRequired = true;
                    }

                    // Build the correct query string based on the available search options for each field.
                    if (User.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Starts))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + ":'" + attribute.Value.Trim() + "*'";
                        }
                        else
                        {
                            request.Query = searchFieldName + ":" + attribute.Value.Trim() + "*";
                        }
                    }
                    else if (User.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Exact))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + "='" + attribute.Value.Trim() + "'";
                        }
                        else
                        {
                            request.Query = searchFieldName + "=" + attribute.Value.Trim();
                        }
                    }
                    else if (User.SearchOperatorsSupported[searchFieldName].Contains(SearchOperatorType.Contains))
                    {
                        if (quotesRequired)
                        {
                            request.Query = searchFieldName + ":'" + attribute.Value.Trim() + "'";
                        }
                        else
                        {
                            request.Query = searchFieldName + ":" + attribute.Value.Trim();
                        }
                    }

                    // TODO: Allow for returning specified fields via returnedAttributes.
                    // request.Fields = "";
                    Users usersRequest = request.Execute();
                    IList<GoogleUser> requestUsers = usersRequest.UsersValue;

                    // Verify that users were returned by the request.
                    if (requestUsers != null)
                    {
                        // Iterate over all the users and populate the return list.
                        foreach (GoogleUser requestUser in requestUsers)
                        {
                            users.Add(new User(this, requestUser));
                        }
                    }
                }
                catch
                {
                    // There was an error retrieving the users.
                }
            }

            // Return the list of users.
            return users;
        }
    }
}