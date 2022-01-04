using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
using GoogleUser = Google.Apis.Admin.Directory.directory_v1.Data.User;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Galactic.Identity.GoogleWorkspace
{
    /// <summary>
    /// GoogleWorkspaceClient is a class that allows for the query and manipulation of Google Workspace objects.
    /// </summary>
    public class GoogleWorkspaceClient : IDirectorySystem
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

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
        public GoogleWorkspaceClient(UserCredential credential, string applicationName)
        {
            if (credential != null || !string.IsNullOrWhiteSpace(applicationName))
            {
                Service = new DirectoryService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
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
        /// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
        /// Note: In Google Workspace, an e-mail address is required. The 'email' attribute must be provided as an additional
        /// attribute in order to successfully create the group.
        /// </summary>
        /// <param name="name">The proposed name of the group.</param>
        /// <param name="type">(Ignored) The type of group to create.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Required, see above.) Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object, or null if it could not be created.</returns>
        public IGroup CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a user within the directory system given it's login, and other options attributes.
        /// </summary>
        /// <param name="login">The proposed login of the user.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
        /// <returns>The newly creaTed user object, or null if it could not be created.</returns>
        public IUser CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a group with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the group to delete.</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        public bool DeleteGroup(string uniqueId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a user with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to delete.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        public bool DeleteUser(string uniqueId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get's all users in the directory system.
        /// </summary>
        /// <returns>A list of all users in the directory system.</returns>
        public List<IUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets IGroups that start with the attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Optional) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of groups that match the attribute value supplied.</returns>
        public List<IGroup> GetGroupsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<object>> returnedAttributes = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of the types of groups supported by the directory system.
        /// </summary>
        /// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
        public List<string> GetGroupTypes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets IUsers that start with the attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Optional) The attributes that should be returned in the user found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of users that match the attribute value supplied.</returns>
        public List<IUser> GetUsersByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<object>> returnedAttributes = null)
        {
            throw new NotImplementedException();
        }
    }
}