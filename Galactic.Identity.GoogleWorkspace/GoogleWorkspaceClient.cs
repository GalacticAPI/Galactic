using Galactic.Cryptography;
using GalFile = Galactic.FileSystem.File;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
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
        public GoogleWorkspaceClient(GoogleCredential credential, string applicationName)
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all groups in the directory system.
        /// </summary>
        /// <returns>A list of all groups in the directory system.</returns>
        public List<IGroup> GetAllGroups()
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