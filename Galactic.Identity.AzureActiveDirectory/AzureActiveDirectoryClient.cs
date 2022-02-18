using System.Reflection;
using Microsoft.Graph;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using GraphUser = Microsoft.Graph.User;
using GraphGroup = Microsoft.Graph.Group;
using GraphDirectoryObject = Microsoft.Graph.DirectoryObject;

namespace Galactic.Identity.AzureActiveDirectory
{
	public class AzureActiveDirectoryClient : DirectorySystemClient
	{
		// ----- CONSTANTS -----

		static protected string[] DefaultUserAttributes = { "city", "country", "department", "displayName", "employeeId", "givenName", "accountEnabled", "surname", "userPrincipalName", "manager", "mobilePhone", "companyName", "passwordPolicies", "passwordProfile", "lastPasswordChangeDateTime", "officeLocation", "streetAddress", "postalCode", "businessPhones", "state", "jobTitle", "createdDateTime", "memberOf", "id", "mail", "proxyAddresses" };
		static protected string[] AllUserCollectionAttributes = {"accountEnabled", "ageGroup", "assignedLicenses", "assignedPlans", "businessPhones", "city", "companyName", "consentProvidedForMinor", "country", "createdDateTime", "creationType", "deletedDateTime", "department", "displayName", "employeeHireDate", "employeeId", "employeeOrgData", "employeeType", "externalUserState", "externalUserStateChangeDateTime", "faxNumber", "givenName", "id", "identities", "imAddresses", "jobTitle", "lastPasswordChangeDate", "legalAgeGroupClassification", "licenseAssignmentStates", "mail", "mailNickname", "mobilePhone", "officeLocation", "onPremisesDistinguishedName", "onPremisesDomainName", "onPremisesExtensionAttributes", "onPremisesImmutableId", "onPremisesLastSyncDateTime", "onPremisesProvisioningErrors", "onPremisesSamAccountName", "onPremisesSecurityIdentifier", "onPremisesSyncEnabled", "onPremisesUserPrincipalName", "otherMails", "passwordPolicies", "passwordProfile", "postalCode", "preferredDataLocation", "preferredLanguage", "provisionedPlans", "proxyAddresses", "refreshTokensValidFromDateTime", "showInAddressList", "signInSessionValidFromDateTime", "state", "streetAddress", "surname", "usageLocation", "userPrincipalName", "userType" };
		static protected string[] AllUserAttributes = { "aboutMe", "accountEnabled", "ageGroup", "assignedLicenses", "assignedPlans", "birthday", "businessPhones", "city", "companyName", "consentProvidedForMinor", "country", "createdDateTime", "creationType", "deletedDateTime", "department", "displayName", "employeeHireDate", "employeeId", "employeeOrgData", "employeeType", "externalUserState", "externalUserStateChangeDateTime", "faxNumber", "givenName", "hireDate", "id", "identities", "imAddresses", "interests", "jobTitle", "lastPasswordChangeDate", "legalAgeGroupClassification", "licenseAssignmentStates", "mail", "mailboxSettings", "mailNickname", "mobilePhone", "mySite", "officeLocation", "onPremisesDistinguishedName", "onPremisesDomainName", "onPremisesExtensionAttributes", "onPremisesImmutableId", "onPremisesLastSyncDateTime", "onPremisesProvisioningErrors", "onPremisesSamAccountName", "onPremisesSecurityIdentifier", "onPremisesSyncEnabled", "onPremisesUserPrincipalName", "otherMails", "passwordPolicies", "passwordProfile", "pastProjects", "postalCode", "preferredDataLocation", "preferredLanguage", "preferredName", "provisionedPlans", "proxyAddresses", "refreshTokensValidFromDateTime", "responsibilities", "schools", "showInAddressList", "skills", "signInSessionValidFromDateTime", "state", "streetAddress", "surname", "usageLocation", "userPrincipalName", "userType" };

		// ----- VARIABLES -----

		// The client used to query and manipulate Active Directory.
		protected GraphServiceClient gsc = null;

		// ----- PROPERTIES -----

		/// <summary>
        /// Default domain name for the organization.
        /// </summary>
		public string DefaultDomain
        {
            get
            {
				var org = GetOrganizationDetails();
				if(org != null)
                {
					foreach(var domain in org.VerifiedDomains)
                    {
						if((bool)domain.IsDefault)
                        {
							return domain.Name;
                        }
                    }
                }
				return "";
            }
        }

		// ----- CONSTRUCTORS -----

		public AzureActiveDirectoryClient(string tenantId, string clientId, string clientSecret)
		{
			// Validate arguments.
			if (String.IsNullOrWhiteSpace(tenantId))
			{
				throw new ArgumentException("tennantId must not be null.");
			}
			if (String.IsNullOrWhiteSpace(clientId))
			{
				throw new ArgumentException("clientId must not be null.");
			}
			if (String.IsNullOrWhiteSpace(clientSecret))
			{
				throw new ArgumentException("clientSecret must not be null.");
			}

			var scopes = new[] { "https://graph.microsoft.com/.default" };

			var options = new TokenCredentialOptions
			{
				AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
			};

			var clientServiceCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

			gsc = new GraphServiceClient(clientServiceCredential, scopes);

		}

		public AzureActiveDirectoryClient(string tenantId, string clientId, X509Certificate2 cert)
		{
			// Validate arguments.
			if (String.IsNullOrWhiteSpace(tenantId))
			{
				throw new ArgumentException("tennantId must not be null.");
			}
			if (String.IsNullOrWhiteSpace(clientId))
			{
				throw new ArgumentException("clientId must not be null.");
			}
			if (cert == null)
			{
				throw new ArgumentException("cert must not be null.");
			}

			var scopes = new[] { "https://graph.microsoft.com/.default" };

			var options = new TokenCredentialOptions
			{
				AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
			};

			var clientServiceCredential = new ClientCertificateCredential(tenantId, clientId, cert, options);

			gsc = new GraphServiceClient(clientServiceCredential, scopes);

		}

		// ----- METHODS -----

		// ----- Organization Operations -----

		public Organization GetOrganizationDetails()
        {
            try
            {
				Task<IGraphServiceOrganizationCollectionPage> response = gsc.Organization.Request().GetAsync();
				response.Wait();

				return response.Result.CurrentPage[0];
			}
            catch
            {
				// An error occured.
				return null;
            }
        }

		// ----- User Operations -----

		/// <summary>
		/// Creates a user within the directory system given it's login, and other options attributes.
		/// </summary>
		/// <param name="login">The proposed login of the user.</param>
		/// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
		/// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
		/// <returns>The newly created user object, or null if it could not be created.</returns>
		public override User CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null)
        {
			try
			{
				string userPrincipalName = (string)additionalAttributes.Find(x => x.Name.ToLower() == "userprincipalname").Value;
				string displayName = (string)additionalAttributes.Find(x => x.Name.ToLower() == "displayname").Value;
				string mailNickname = (string)additionalAttributes.Find(x => x.Name.ToLower() == "mailnickname").Value;
				string password = (string)additionalAttributes.Find(x => x.Name.ToLower() == "password").Value;
				bool accountEnabled = (bool)additionalAttributes.Find(x => x.Name.ToLower() == "accountenabled").Value;

				return CreateUser(userPrincipalName,displayName,mailNickname,password,accountEnabled);
			}
			catch (NullReferenceException)
			{
				// Required attribute missing from list.
				return null;
			}
		}

		/// <summary>
		/// Creates a user within the directory system given it's login, and other options attributes.
		/// </summary>
		/// <param name="userPrincipalName"></param>
		/// <param name="displayName"></param>
		/// <param name="mailNickname"></param>
		/// <param name="password"></param>
		/// <param name="accountEnabled">Optional: Specifies if the account object should be created in an enabled or disabled state. Set to true by default.</param>
		/// <returns>The newly created user object, or null if it could not be created.</returns>
		public User CreateUser(string userPrincipalName, string displayName, string mailNickname, string password, bool accountEnabled = true)
        {
            try
            {
				var graphUser = new GraphUser
				{
					AccountEnabled = accountEnabled,
					DisplayName = displayName,
					MailNickname = mailNickname,
					UserPrincipalName = userPrincipalName,
					PasswordProfile = new PasswordProfile
					{
						ForceChangePasswordNextSignIn = false,
						Password = password
					}
				};

				Task<GraphUser> response = gsc.Users.Request().AddAsync(graphUser);
				response.Wait();

				return new User(this, response.Result);
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
		}

		/// <summary>
		/// Deletes a user with the specified unique id from the directory system.
		/// </summary>
		/// <param name="uniqueId">The unique id of the user to delete.</param>
		/// <returns>True if the user was deleted, false otherwise.</returns>
		public override bool DeleteUser(string uniqueId)
		{
            try
            {
				Task<GraphResponse> response = gsc.Users[uniqueId].Request().DeleteResponseAsync();

				response.Wait();

				if (response.Result.StatusCode == System.Net.HttpStatusCode.NoContent)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return false;
            }
		}

		/// <summary>
		/// Updates the supplied attributes on the specified user.
		/// </summary>
		/// <param name="id">ID of user to update.</param>
		/// <param name="attributes">Attributes to update.</param>
		/// <returns>True if updated, false otherwise.</returns>
		public bool UpdateUser(string id, List<IdentityAttribute<Object>> attributes)
        {
			GraphUser user = new GraphUser();

			var type = user.GetType();


			foreach(var attr in attributes)
            {
				PropertyInfo prop = type.GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				if(prop != null)
                {
					prop.SetValue(user, attr.Value);
                }
                else
                {
					// Specified attribute does not match a property on Microsoft.Graph.User object.
					return false;
                }
			}

			return UpdateUser(id,user);
        }

		/// <summary>
		/// Updates the supplied attributes on the specified user.
		/// </summary>
		/// <param name="id">ID of user to update.</param>
		/// <param name="user">User object containing updated attributes.</param>
		/// <returns>True if updated, false otherwise.</returns>
		public bool UpdateUser(string id, GraphUser user)
		{
            try
            {
				Task<GraphUser> response = gsc.Users[id].Request().UpdateAsync(user);
				response.Wait();

				return true;
			}
            catch(AggregateException e)
            {
				// An error occured.
				return false;
            }
		}

		/// <summary>
		/// Get's all users in the directory system.
		/// </summary>
		/// <returns>A list of all users in the directory system.</returns>
		public override List<Identity.User> GetAllUsers()
		{
			List<GraphUser> graphUsers = GetAllGraphUsers();
			List<Identity.User> users = new();

			if(graphUsers != null)
            {
				foreach (GraphUser graphUser in graphUsers)
				{
					users.Add(new User(this, graphUser));
				}
			}

			return users;
		}

		/// <summary>
		/// Get's all users in the directory system.
		/// </summary>
		/// <returns>A list of all users in the directory system.</returns>
		public List<GraphUser> GetAllGraphUsers()
		{
            try
            {
				Task<IGraphServiceUsersCollectionPage> response = gsc.Users.Request().Select(string.Join(',', DefaultUserAttributes)).GetAsync();

				response.Wait();

				List<GraphUser> users = new();

				users.AddRange(response.Result.CurrentPage);

				while (response.Result.NextPageRequest != null)
				{
					response = response.Result.NextPageRequest.GetAsync();
					response.Wait();
					users.AddRange(response.Result.CurrentPage);
				}

				return users;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
		}

		/// <summary>
		/// Gets IUsers that start with the attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attribute">The attribute with name and value to search against.</param>
		/// <param name="returnedAttributes">(Optional) The attributes that should be returned in the user found. If not supplied, the default list of attributes is returned.</param>
		/// <returns>A list of users that match the attribute value supplied.</returns>
		public override List<Identity.User> GetUsersByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
		{
			if (attribute != null && !String.IsNullOrWhiteSpace(attribute.Name) && attribute.Value != null)
			{
				// Get the names of any attributes to return.
				List<string> attributeNames = new();
				if (returnedAttributes != null)
				{
					foreach (IdentityAttribute<object> returnedAttribute in returnedAttributes)
					{
						attributeNames.Add(returnedAttribute.Name);
					}
				}

				// Search for entries that match the wildcarded attribute value supplied.
				List<GraphUser> searchResults = GetGraphUsersByAttribute(attribute.Name, attribute.Value, attributeNames);

				// Filter the list of entries returned so that only Users are returned.
				List<Identity.User> users = new();
				if (searchResults != null)
				{
					foreach (GraphUser graphUser in searchResults)
					{
						User user = new(this, graphUser);
						users.Add(user);
					}
				}

				return users;
			}
			else
			{
				throw new ArgumentNullException(nameof(attribute));
			}
		}

		/// <summary>
		/// Gets a user matching the supplied login.
		/// </summary>
		/// <param name="login">The login of the user.</param>
		/// <returns>A User matching the supplied login.</returns>
		public Identity.User GetUserByLogin(string login)
		{
			// Validate that parameter is supplied.
			if (!string.IsNullOrWhiteSpace(login))
			{
				// Create IdentityAttribute for group name.
				IdentityAttribute<string> attribute = new IdentityAttribute<string>("userprincipalname", login);

				try
				{
					List<Identity.User> result = GetUsersByAttribute(attribute);

					if (result.Count == 1)
					{
						return result[0];
					}
					else if (result.Count > 1)
					{
						// Multiple results found.
						return result.FirstOrDefault(x => x.Login == login);
					}
					else
					{
						// No results found.
						return null;
					}
				}
				catch
				{
					// There was an error retrieving the group.
					return null;
				}
			}

			// Bad parameter. 
			return null;
		}

		/// <summary>
		/// Gets Microsoft Graph Users that match a given attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search against.</param>
		/// <param name="attributeValue">The value to search for in the attribute.</param>
		/// <param name="attributeNames">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
		/// <returns></returns>
		public List<GraphUser> GetGraphUsersByAttribute(string attributeName, string attributeValue, List<string> attributeNames = null)
		{
            try
            {
				if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
				{
					//Check if custom list of attributes exists.
					if (attributeNames == null | attributeNames.Count == 0)
					{
						attributeNames = AllUserCollectionAttributes.ToList();
					}

					// Create attribute string.
					string selectedAttributes = string.Join(',', attributeNames);

					// Create the search filter string that will find the entry in the directory with
					// the specified attribute value.
					string filter;

					// Check if search is wildcard.
					if (attributeValue[attributeValue.Length - 1] == '*')
					{
						// Perform search using "startswith" filter.
						filter = "startswith(" + attributeName + ", \'" + attributeValue.TrimEnd('*') + "\')";
					}
					else
					{
						// Perform exact match search.
						filter = attributeName + " eq \'" + attributeValue + "\'";
					}

					//Create list to hold search results.
					List<GraphUser> users = new();

					//Query Graph Client for users matching search filter.
					Task<IGraphServiceUsersCollectionPage> response = gsc.Users.Request().Header("ConsistencyLevel", "eventual").Filter(filter).Select(selectedAttributes).GetAsync();
					response.Wait();
					users.AddRange(response.Result.CurrentPage);

					//Check for additional pages of data.
					while (response.Result.NextPageRequest != null)
					{
						response = response.Result.NextPageRequest.GetAsync();
						response.Wait();
						users.AddRange(response.Result.CurrentPage);
					}

					return users;
				}
				// The attribute name or value provided is not valid.
				return null;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
		}

		/// <summary>
		/// Gets User object associated with specified ID.
		/// </summary>
		/// <param name="id">ID of the user to get.</param>
		/// <param name="returnedAttributes"><Optional: List of attributes to get. If not specified, a default set will be used./param>
		/// <returns>User object associated with supplied ID.</returns>
		public Identity.User GetUser(string id, List<IdentityAttribute<Object>> returnedAttributes = null)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				// Get the names of any attributes to return.
				List<string> attributeNames = new();
				if (returnedAttributes != null)
				{
					foreach (IdentityAttribute<object> returnedAttribute in returnedAttributes)
					{
						attributeNames.Add(returnedAttribute.Name);
					}
				}

				GraphUser result = GetGraphUser(id, attributeNames);

				if(result != null)
                {
					return new User(this, result);
                }
				else
                {
					// No user was found.
					return null;
                }
			}
			else
			{
				throw new ArgumentException("id cannot be null or empty");
			}
		}

		/// <summary>
        /// Gets Microsoft Graph user object representing specified user.
        /// </summary>
        /// <param name="id">ID of the user to get.</param>
        /// <param name="attributeNames">Optional: List of attributes to get. If not specified, a default set will be used.</param>
        /// <returns>Microsoft Graph User object associated with supplied ID.</returns>
		public GraphUser GetGraphUser(string id, List<string> attributeNames = null)
        {
            try
            {
				if (!string.IsNullOrWhiteSpace(id))
				{
					// Check if custom list of attributes exists.
					if (attributeNames == null | attributeNames.Count == 0)
					{
						attributeNames = AllUserCollectionAttributes.ToList();
					}

					// Create attribute string.
					string selectedAttributes = string.Join(',', attributeNames);

					// Query Graph Client for user data.
					Task<GraphUser> response = gsc.Users[id].Request().Select(selectedAttributes).GetAsync();
					response.Wait();

					return response.Result;
				}
				else
				{
					throw new ArgumentException("id cannot be null or empty");
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
		}

		/// <summary>
        /// Gets the manager of the specified user.
        /// </summary>
        /// <param name="id">ID of the user to perform operation on.</param>
        /// <returns>Microsoft.Graph.User object of the users manager. Null if no manager.</returns>
		public GraphUser GetUserManager(string id)
        {
            try
            {
				Task<GraphDirectoryObject> response = gsc.Users[id].Manager.Request().GetAsync();
				response.Wait();

				return (GraphUser)response.Result;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
        }

		/// <summary>
        /// Sets the manager of the specified user.
        /// </summary>
        /// <param name="userId">ID of the user to perform set operation on.</param>
        /// <param name="managerId">ID of the manager.</param>
        /// <returns>True if manager was set, false otherwise.</returns>
		public bool SetUserManager(string userId, string managerId)
        {
            try
            {
				Task<GraphResponse> response = gsc.Users[userId].Manager.Reference.Request().PutResponseAsync(managerId);
				response.Wait();

				return true;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return false;
            }
		}



		// ----- Group Operations -----

		/// <summary>
		/// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
		/// </summary>
		/// <param name="name">The proposed name of the group.</param>
		/// <param name="type">The type of group to create.</param>
		/// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
		/// <param name="additionalAttributes">(Optional) Additional attributes to set when creating the group.</param>
		/// <returns>The newly created group object, or null if it could not be created.</returns>
		public override Group CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null)
		{
            try
            {
				string description = (string)additionalAttributes.Find(x => x.Name.ToLower() == "description").Value;
				string displayName = (string)additionalAttributes.Find(x => x.Name.ToLower() == "displayname").Value;
				string mailNickname = (string)additionalAttributes.Find(x => x.Name.ToLower() == "mailnickname").Value;
				bool mailEnabled = (bool)additionalAttributes.Find(x => x.Name.ToLower() == "mailenabled").Value;
				bool securityEnabled = (bool)additionalAttributes.Find(x => x.Name.ToLower() == "securityenabled").Value;

				return CreateGroup(description, displayName, mailNickname, mailEnabled, securityEnabled);
			}
            catch(NullReferenceException)
            {
				// Required attribute missing from list.
				return null;
            }
		}

		/// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="displayName"></param>
        /// <param name="mailNickname"></param>
        /// <param name="mailEnabled"></param>
        /// <param name="securityEnabled"></param>
        /// <returns></returns>
		public Group CreateGroup(string description, string displayName, string mailNickname, bool mailEnabled, bool securityEnabled)
        {
            try
            {
				if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(mailNickname))
				{
					var group = new GraphGroup
					{
						Description = description,
						DisplayName = displayName,
						MailNickname = mailNickname,
						MailEnabled = mailEnabled,
						SecurityEnabled = securityEnabled,
						GroupTypes = new List<string>
					{
					"Unified"
					}
					};

					Task<GraphGroup> response = gsc.Groups.Request().AddAsync(group);
					response.Wait();
					return new Group(this, response.Result);
				}
				else
				{
					// One of the supplied arguments was invalid.
					return null;
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
        }

		/// <summary>
		/// Deletes a group with the specified unique id from the directory system.
		/// </summary>
		/// <param name="uniqueId">The unique id of the group to delete.</param>
		/// <returns>True if the group was deleted, false otherwise.</returns>
		public override bool DeleteGroup(string uniqueId)
        {
            try
            {
				Task<GraphResponse> response = gsc.Groups[uniqueId].Request().DeleteResponseAsync();

				response.Wait();

				if (response.Result.StatusCode == System.Net.HttpStatusCode.NoContent)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return false;
            }
		}

		/// <summary>
		/// Gets a list of the types of groups supported by the directory system.
		/// </summary>
		/// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
		public override List<string> GetGroupTypes()
        {
			return new List<string>();
        }

		/// <summary>
        /// Updates group attributes.
        /// </summary>
        /// <param name="id">ID of group to update.</param>
        /// <param name="attributes">Attributes to update.</param>
        /// <returns>True if success, false otherwise.</returns>
		public bool UpdateGroup(string id, List<IdentityAttribute<Object>> attributes)
		{
			GraphGroup group = new ();

			var type = group.GetType();


			foreach (var attr in attributes)
			{
				PropertyInfo prop = type.GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				if (prop != null)
				{
					prop.SetValue(group, attr.Value);
				}
				else
				{
					// Specified attribute does not match a property on Microsoft.Graph.Group object.
					return false;
				}
			}

			return UpdateGroup(id, group);
		}

		/// <summary>
        /// Updates group attributes.
        /// </summary>
        /// <param name="id">ID of group to update.</param>
        /// <param name="group">Microsoft Graph Group object containing updated attributes.</param>
        /// <returns>True if success, false otherwise.</returns>
		public bool UpdateGroup(string id, GraphGroup group)
		{
			try
			{
				Task<GraphGroup> response = gsc.Groups[id].Request().UpdateAsync(group);
				response.Wait();

				return true;
			}
			catch(AggregateException e)
			{
				// An error occured.
				return false;
			}
		}

		/// <summary>
		/// Get's all groups in the directory system.
		/// </summary>
		/// <returns>A list of all groups in the directory system.</returns>
		public override List<Identity.Group> GetAllGroups()
		{
			List<GraphGroup> graphGroups = GetAllGraphGroups();
			List<Identity.Group> groups = new();

			if (graphGroups != null)
			{
				foreach (GraphGroup graphGroup in graphGroups)
				{
					groups.Add(new Group(this, graphGroup));
				}
			}

			return groups;
		}

		/// <summary>
		/// Get's all groups in the directory system.
		/// </summary>
		/// <returns>A list of all groups in the directory system.</returns>
		public List<GraphGroup> GetAllGraphGroups()
		{
			try
			{
				Task<IGraphServiceGroupsCollectionPage> response = gsc.Groups.Request().GetAsync();

				response.Wait();

				List<GraphGroup> groups = new();

				groups.AddRange(response.Result.CurrentPage);

				while (response.Result.NextPageRequest != null)
				{
					response = response.Result.NextPageRequest.GetAsync();
					response.Wait();
					groups.AddRange(response.Result.CurrentPage);
				}

				return groups;
			}
			catch (AggregateException)
			{
				// An error occurred.
				return null;
			}
		}

		/// <summary>
		/// Gets IGroups that start with the attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attribute">The attribute with name and value to search against.</param>
		/// <param name="returnedAttributes">(Optional) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
		/// <returns>A list of groups that match the attribute value supplied.</returns>
		public override List<Identity.Group> GetGroupsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
        {
			if (attribute != null && !String.IsNullOrWhiteSpace(attribute.Name) && attribute.Value != null)
			{
				// Get the names of any attributes to return.
				List<string> attributeNames = new();
				if (returnedAttributes != null)
				{
					foreach (IdentityAttribute<object> returnedAttribute in returnedAttributes)
					{
						attributeNames.Add(returnedAttribute.Name);
					}
				}

				// Search for entries that match the wildcarded attribute value supplied.
				List<GraphGroup> searchResults = GetGraphGroupsByAttribute(attribute.Name, attribute.Value, attributeNames);

				// Filter the list of entries returned so that only Users are returned.
				List<Identity.Group> groups = new();
				if (searchResults != null)
				{
					foreach (GraphGroup graphGroup in searchResults)
					{
						Group group = new(this, graphGroup);
						groups.Add(group);
					}
				}

				return groups;
			}
			else
			{
				throw new ArgumentNullException(nameof(attribute));
			}
		}

		/// <summary>
		/// Gets a group matching the supplied name.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		/// <returns>A Group matching the supplied name.</returns>
		public Identity.Group GetGroupByName(string name)
		{
			// Validate that parameter is supplied.
			if (!string.IsNullOrWhiteSpace(name))
			{
				// Create IdentityAttribute for group name.
				IdentityAttribute<string> attribute = new IdentityAttribute<string>("displayname", name);

				try
				{
					List<Identity.Group> result = GetGroupsByAttribute(attribute);

					if (result.Count == 1)
					{
						return result[0];
					}
					else if (result.Count > 1)
					{
						// Multiple results found.
						return result.FirstOrDefault(x => x.Name == name);
					}
					else
					{
						// No results found.
						return null;
					}
				}
				catch
				{
					// There was an error retrieving the group.
					return null;
				}
			}

			// Bad parameter. 
			return null;
		}

		/// <summary>
		/// Gets Microsoft Graph Groups that match a given attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search against.</param>
		/// <param name="attributeValue">The value to search for in the attribute.</param>
		/// <param name="attributeNames">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
		/// <returns>List of GraphGroup objects matching search criteria.</returns>
		public List<GraphGroup> GetGraphGroupsByAttribute(string attributeName, string attributeValue, List<string> attributeNames = null)
		{
            try
            {
				if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
				{
					//Check if custom list of attributes exists.
					if (attributeNames == null)
					{
						attributeNames = DefaultUserAttributes.ToList();
					}

					// Create attribute string.
					string selectedAttributes = string.Join(',', attributeNames);

					// Create the search filter string that will find the entry in the directory with
					// the specified attribute value.
					string filter;

					// Check if search is wildcard.
					if (attributeValue[attributeValue.Length - 1] == '*')
					{
						// Perform search using "startswith" filter.
						filter = "startswith(" + attributeName + ", \'" + attributeValue.TrimEnd('*') + "\')";
					}
					else
					{
						// Perform exact match search.
						filter = attributeName + " eq \'" + attributeValue + "\'";
					}

					// Create list to hold search results.
					List<GraphGroup> groups = new();

					// Query Graph Client for users matching search filter.
					Task<IGraphServiceGroupsCollectionPage> response = gsc.Groups.Request().Header("ConsistencyLevel", "eventual").Filter(filter).Select(selectedAttributes).GetAsync();
					response.Wait();

					groups.AddRange(response.Result.CurrentPage);

					// Check for additional pages of data.
					while (response.Result.NextPageRequest != null)
					{
						response = response.Result.NextPageRequest.GetAsync();
						response.Wait();
						groups.AddRange(response.Result.CurrentPage);
					}

					return groups;
				}
				// The attribute name or value provided is not valid.
				return null;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
		}

		/// <summary>
		/// Gets Groups that match the supplied search filter.
		/// </summary>
		/// <param name="filter">The search filter.</param>
		/// <param name="returnedAttributes">(Optional) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
		/// <returns>A list of groups that match the search filter.</returns>
		public List<Identity.Group> GetGroupsByFilter(string filter, List<IdentityAttribute<Object>> returnedAttributes = null)
		{
			if (!String.IsNullOrWhiteSpace(filter))
			{
				// Get the names of any attributes to return.
				List<string> attributeNames = new();
				if (returnedAttributes != null)
				{
					foreach (IdentityAttribute<object> returnedAttribute in returnedAttributes)
					{
						attributeNames.Add(returnedAttribute.Name);
					}
				}

				// Search for entries that match the wildcarded attribute value supplied.
				List<GraphGroup> searchResults = GetGraphGroupsByFilter(filter, attributeNames);

				// Filter the list of entries returned so that only Users are returned.
				List<Identity.Group> groups = new();
				if (searchResults != null)
				{
					foreach (GraphGroup graphGroup in searchResults)
					{
						Group group = new(this, graphGroup);
						groups.Add(group);
					}
				}

				return groups;
			}
			else
			{
				throw new ArgumentNullException(nameof(filter));
			}
		}

		/// <summary>
		/// Gets Microsoft Graph Groups that match a given search filter.
		/// </summary>
		/// <param name="filter">The search filter.</param>
		/// <param name="attributeNames">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
		/// <returns>List of GraphGroup objects matching search filter.</returns>
		public List<GraphGroup> GetGraphGroupsByFilter(string filter, List<string> attributeNames = null)
		{
			try
			{
				if (!string.IsNullOrEmpty(filter))
				{
					//Check if custom list of attributes exists.
					if (attributeNames == null || attributeNames.Count == 0)
					{
						attributeNames = DefaultUserAttributes.ToList();
					}

					// Create attribute string.
					string selectedAttributes = string.Join(',', attributeNames);

					// Create list to hold search results.
					List<GraphGroup> groups = new();

					// Query Graph Client for users matching search filter.
					var req = gsc.Groups.Request();
					req.QueryOptions.Add(new QueryOption("$count", "true"));
					Task<IGraphServiceGroupsCollectionPage> response = req.Header("ConsistencyLevel", "eventual").Filter(filter).Select(selectedAttributes).GetAsync();
					response.Wait();

					groups.AddRange(response.Result.CurrentPage);

					// Check for additional pages of data.
					while (response.Result.NextPageRequest != null)
					{
						response = response.Result.NextPageRequest.GetAsync();
						response.Wait();
						groups.AddRange(response.Result.CurrentPage);
					}

					return groups;
				}
				// The attribute name or value provided is not valid.
				return null;
			}
			catch (AggregateException e)
			{
				// An error occurred.
				return null;
			}
		}

		/// <summary>
		/// Gets the group membership of the specified object.
		/// </summary>
		/// <param name="id">Id of the object.</param>
		/// <param name="recursive">If true, performs a transitive search of membership.</param>
		/// <returns>List of Group objects that this object is a member of.</returns>
		public List<Identity.Group> GetGroupMembership(string id, bool recursive)
		{
            try
            {
				if (!String.IsNullOrWhiteSpace(id))
				{
					List<DirectoryObject> groups = new();
					List<GraphGroup> gg = new();

					if (recursive)
					{
						// Perform recursive membership lookup.
						Task<IUserTransitiveMemberOfCollectionWithReferencesPage> response = gsc.Users[id].TransitiveMemberOf.Request().GetAsync();
						try
						{
							response.Wait();
						}
						catch
						{
							return null;
						}

						groups.AddRange(response.Result.CurrentPage);

						//Check for additional pages of data.
						while (response.Result.NextPageRequest != null)
						{
							response = response.Result.NextPageRequest.GetAsync();
							response.Wait();
							groups.AddRange(response.Result.CurrentPage);
						}
					}
					else
					{
						// Only list explicit membership.
						Task<IUserMemberOfCollectionWithReferencesPage> response = gsc.Users[id].MemberOf.Request().GetAsync();
						response.Wait();
						groups.AddRange(response.Result.CurrentPage);

						//Check for additional pages of data.
						while (response.Result.NextPageRequest != null)
						{
							response = response.Result.NextPageRequest.GetAsync();
							response.Wait();
							groups.AddRange(response.Result.CurrentPage);
						}
					}

					foreach (var obj in groups)
					{
						// Fix later when directory roles are implemented.
						if (obj.ODataType == "#microsoft.graph.group")
						{
							gg.Add((GraphGroup)obj);
						}
					}

					return gg.ConvertAll(graphGroup => (Identity.Group)new Group(this, graphGroup));
				}

				return null;
			}
            catch(AggregateException)
            {
				// An error occurred.
				return null;
            }
			
		}

		/// <summary>
        /// Gets all users that are members of the specified group.
        /// </summary>
        /// <param name="id">The unique identifier of the group.</param>
        /// <param name="recursive">If true, performs a transitive search of membership.</param>
        /// <returns>List of User objects that are members of this group.</returns>
		public List<Identity.User> GetUserMembers(string id, bool recursive)
        {
			List<Identity.User> users = new ();

			var objects = GetMembers(id, recursive);

			if(objects != null)
            {
				foreach (GraphDirectoryObject obj in objects)
				{
					if (obj.ODataType == "#microsoft.graph.user")
					{
						users.Add(new User(this, (GraphUser)obj));
					}
				}
			}

			return users;
        }

		/// <summary>
		/// Gets all users that are members of the specified group.
		/// </summary>
		/// <param name="id">The unique identifier of the group.</param>
		/// <param name="recursive">If true, performs a transitive search of membership.</param>
		/// <returns>List of Group objects that are members of this group.</returns>
		public List<Identity.Group> GetGroupMembers(string id, bool recursive)
        {
			List<Identity.Group> groups = new();

			var objects = GetMembers(id, recursive);

			if(objects != null)
            {
				foreach (GraphDirectoryObject obj in objects)
				{
					if (obj.ODataType == "#microsoft.graph.group")
					{
						groups.Add(new Group(this, (GraphGroup)obj));
					}
				}
			}

			return groups;
		}

		/// <summary>
		/// Gets all objects that are members of the specified group.
		/// </summary>
		/// <param name="id">The unique identifier of the group.</param>
		/// <param name="recursive">If true, performs a transitive search of membership.</param>
		/// <returns>List of Microsoft Graph Directory Objects that are members of this group.</returns>
		public List<GraphDirectoryObject> GetMembers(string id, bool recursive)
		{
            try
            {
				if (!String.IsNullOrWhiteSpace(id))
				{
					List<GraphDirectoryObject> objects = new();

					if (recursive)
					{
						// Perform recursive membership lookup.
						Task<IGroupTransitiveMembersCollectionWithReferencesPage> response = gsc.Groups[id].TransitiveMembers.Request().GetAsync();
						response.Wait();
						objects.AddRange(response.Result.CurrentPage);

						//Check for additional pages of data.
						while (response.Result.NextPageRequest != null)
						{
							response = response.Result.NextPageRequest.GetAsync();
							response.Wait();
							objects.AddRange(response.Result.CurrentPage);
						}
					}
					else
					{
						// Only list explicit membership.
						Task<IGroupMembersCollectionWithReferencesPage> response = gsc.Groups[id].Members.Request().GetAsync();
						response.Wait();
						objects.AddRange(response.Result.CurrentPage);

						//Check for additional pages of data.
						while (response.Result.NextPageRequest != null)
						{
							response = response.Result.NextPageRequest.GetAsync();
							response.Wait();
							objects.AddRange(response.Result.CurrentPage);
						}
					}

					return objects;
				}

				return null;
			}
            catch(AggregateException e)
            {
				// An error occured.
				return null;
            }
		}

		/// <summary>
        /// Checks if a Directory Object is a member of the groups in the supplied list.
        /// </summary>
        /// <param name="objectId">ID of the directory object.</param>
        /// <param name="groupIds">IDs of the groups to check for membership in.</param>
        /// <returns>List of group IDs. If a match exists, the ID wil be returned. If no IDs are returned, then the Directory Object is not a member of any of the groups supplied.</returns>
		public IList<string> CheckGroupMembership(string objectId, List<string> groupIds)
        {
            try
            {
				if (!string.IsNullOrEmpty(objectId) && groupIds != null && groupIds.Count > 0)
				{
					Task<IDirectoryObjectCheckMemberGroupsCollectionPage> response = gsc.DirectoryObjects[objectId].CheckMemberGroups(groupIds).Request().PostAsync();
					response.Wait();

					return response.Result.CurrentPage;
				}

				// Bad arguments
				return null;
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
        }

		/// <summary>
        /// Removes the specified Directory Object from the specified Group.
        /// </summary>
        /// <param name="objectId">ID of the directory object to remove from group.</param>
        /// <param name="groupId">ID of the group to remove object from.</param>
        /// <returns>True if object was removed, false if not.</returns>
        /// <exception cref="ArgumentNullException"></exception>
		public bool DeleteObjectFromGroup(string objectId, string groupId)
        {
            try
            {
				if (!String.IsNullOrEmpty(objectId) && !String.IsNullOrEmpty(groupId))
				{
					Task<GraphResponse> response = gsc.Groups[groupId].Members[objectId].Reference.Request().DeleteResponseAsync();
					response.Wait();

					if (response.Result.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					throw new ArgumentNullException();
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return false;
            }
        }

		/// <summary>
		/// Adds specified Directory Object to the specified Group.
		/// </summary>
		/// <param name="objectId">ID of the directory object to add to the group.</param>
		/// <param name="groupId">ID of the group to add object to.</param>
		/// <returns>True if object was added, false if not.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool AddObjectToGroup(string objectId, string groupId)
        {
            try
            {
				if (!String.IsNullOrEmpty(objectId) && !String.IsNullOrEmpty(groupId))
				{
					GraphDirectoryObject obj = new DirectoryObject
					{
						Id = objectId
					};

					Task<GraphResponse> response = gsc.Groups[groupId].Members.References.Request().AddResponseAsync(obj);
					response.Wait();

					if (response.Result.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					throw new ArgumentNullException();
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return false;
            }
		}

		/// <summary>
        /// Gets a Group matching the provided ID.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <param name="returnedAttributes">Optional: Additonal attributes to be returned. If null, a default set will be used.</param>
        /// <returns>Group matching the supplied ID.</returns>
        /// <exception cref="ArgumentException"></exception>
		public Identity.Group GetGroup(string id, List<IdentityAttribute<Object>> returnedAttributes = null)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				// Get the names of any attributes to return.
				List<string> attributeNames = new();
				if (returnedAttributes != null)
				{
					foreach (IdentityAttribute<object> returnedAttribute in returnedAttributes)
					{
						attributeNames.Add(returnedAttribute.Name);
					}
				}

				GraphGroup result = GetGraphGroup(id, attributeNames);

				if (result != null)
				{
					return new Group(this, result);
				}
				else
				{
					// No user was found.
					return null;
				}
			}
			else
			{
				throw new ArgumentException("id cannot be null or empty");
			}
		}

		/// <summary>
		/// Get a Microsoft Graph Group matching the supplied ID.
		/// </summary>
		/// <param name="id">The ID of the group.</param>
		/// <param name="attributeNames">Optional: Additonal attributes to be returned. If null, a default set will be used.</param>
		/// <returns>Microsoft Graph Group matching the ID.</returns>
		/// <exception cref="ArgumentException"></exception>
		public GraphGroup GetGraphGroup(string id, List<string> attributeNames = null)
		{
            try
            {
				if (!string.IsNullOrWhiteSpace(id))
				{
					// Check if custom list of attributes exists.
					if (attributeNames == null | attributeNames.Count == 0)
					{
						//attributeNames = AllUserCollectionAttributes.ToList();
					}

					// Create attribute string.
					string selectedAttributes = string.Join(',', attributeNames);

					// Query Graph Client for user data.
					Task<GraphGroup> response = gsc.Groups[id].Request().Select(selectedAttributes).GetAsync();
					response.Wait();

					return response.Result;
				}
				else
				{
					throw new ArgumentException("id cannot be null or empty");
				}
			}
            catch(AggregateException e)
            {
				// An error occurred.
				return null;
            }
			
		}
	}
}

