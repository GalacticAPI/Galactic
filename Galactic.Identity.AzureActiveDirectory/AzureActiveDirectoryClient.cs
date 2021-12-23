﻿using System;
using Microsoft.Graph;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using GraphUser = Microsoft.Graph.User;
using GraphGroup = Microsoft.Graph.Group;
using GraphDirectoryObject = Microsoft.Graph.DirectoryObject;

namespace Galactic.Identity.AzureActiveDirectory
{
	public class AzureActiveDirectoryClient //: IDirectorySystem, IDisposable
	{
		// ----- CONSTANTS -----

		static protected string[] DefaultUserAttributes = { "city", "country", "department", "displayName", "employeeId", "givenName", "accountEnabled", "surname", "userPrincipalName", "manager", "mobilePhone", "companyName", "passwordPolicies", "passwordProfile", "lastPasswordChangeDateTime", "officeLocation", "streetAddress", "postalCode", "businessPhones", "state", "jobTitle", "createdDateTime", "memberOf", "id", "mail", "proxyAddresses" };

		// ----- VARIABLES -----

		// The client used to query and manipulate Active Directory.
		protected GraphServiceClient gsc = null;

		// ----- PROPERTIES -----

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

		// ----- User Operations -----

		/// <summary>
		/// Creates a user within the directory system given it's login, and other options attributes.
		/// </summary>
		/// <param name="login">The proposed login of the user.</param>
		/// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
		/// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user.</param>
		/// <returns>The newly created user object, or null if it could not be created.</returns>
		public IUser CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null)
        {
			return null;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userPrincipalName"></param>
		/// <param name="displayName"></param>
		/// <param name="mailNickname"></param>
		/// <param name="password"></param>
		/// <param name="accountEnabled">Optional: Specifies if the account object should be created in an enabled or disabled state. Set to true by default.</param>
		/// <returns>The newly created user object, or null if it could not be created.</returns>
		public User CreateUser(string userPrincipalName, string displayName, string mailNickname, string password, bool accountEnabled = true)
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

		/// <summary>
		/// Deletes a user with the specified unique id from the directory system.
		/// </summary>
		/// <param name="uniqueId">The unique id of the user to delete.</param>
		/// <returns>True if the user was deleted, false otherwise.</returns>
		public bool DeleteUser(string uniqueId)
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

		/// <summary>
		/// Get's all users in the directory system.
		/// </summary>
		/// <returns>A list of all users in the directory system.</returns>
		public List<IUser> GetAllUsers()
		{
			List<GraphUser> graphUsers = GetAllGraphUsers();
			List<IUser> users = new();

			foreach (GraphUser graphUser in graphUsers)
			{
				users.Add(new User(this, graphUser));
			}

			return users;
		}

		/// <summary>
		/// Get's all users in the directory system.
		/// </summary>
		/// <returns>A list of all users in the directory system.</returns>
		public List<GraphUser> GetAllGraphUsers()
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

		/// <summary>
		/// Gets IUsers that start with the attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attribute">The attribute with name and value to search against.</param>
		/// <param name="returnedAttributes">(Optional) The attributes that should be returned in the user found. If not supplied, the default list of attributes is returned.</param>
		/// <returns>A list of users that match the attribute value supplied.</returns>
		public List<IUser> GetUsersByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
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
				List<GraphUser> searchResults = GetUsersByAttribute(attribute.Name, attribute.Value, attributeNames);

				// Filter the list of entries returned so that only Users are returned.
				List<IUser> users = new();
				if (searchResults != null)
				{
					foreach (GraphUser graphUser in searchResults)
					{
						User user = new(this, graphUser);
						users.Add((User)user);
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
		/// Gets Microsoft Graph Users that match a given attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search against.</param>
		/// <param name="attributeValue">The value to search for in the attribute.</param>
		/// <param name="attributeNames">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
		/// <returns></returns>
		public List<GraphUser> GetUsersByAttribute(string attributeName, string attributeValue, List<string> attributeNames = null)
		{
			if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
			{
				//Check if custom list of attributes exists.
				if (attributeNames == null)
				{
					attributeNames = DefaultUserAttributes.ToList();
				}

				// Create the search filter string that will find the entry in the directory with
				// the specified attribute value.
				string filter = attributeName + " eq \'" + attributeValue + "\'";

				//Create list to hold search results.
				List<GraphUser> users = new();

				//Query Graph Client for users matching search filter.
				Task<IGraphServiceUsersCollectionPage> response = gsc.Users.Request().Header("ConsistencyLevel", "eventual").Filter(filter).Select(string.Join(',', attributeNames)).GetAsync();
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

		// ----- Group Operations -----

		/// <summary>
		/// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
		/// </summary>
		/// <param name="name">The proposed name of the group.</param>
		/// <param name="type">The type of group to create.</param>
		/// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
		/// <param name="additionalAttributes">(Optional) Additional attributes to set when creating the group.</param>
		/// <returns>The newly created group object, or null if it could not be created.</returns>
		public IGroup CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<Object>> additionalAttributes = null)
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

		public Group CreateGroup(string description, string displayName, string mailNickname, bool mailEnabled, bool securityEnabled)
        {
			if(!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(mailNickname))
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

		/// <summary>
		/// Deletes a group with the specified unique id from the directory system.
		/// </summary>
		/// <param name="uniqueId">The unique id of the group to delete.</param>
		/// <returns>True if the group was deleted, false otherwise.</returns>
		public bool DeleteGroup(string uniqueId)
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

		/// <summary>
		/// Gets a list of the types of groups supported by the directory system.
		/// </summary>
		/// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
		public List<string> GetGroupTypes()
        {
			return new List<string>();
        }

		/// <summary>
		/// Gets IGroups that start with the attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attribute">The attribute with name and value to search against.</param>
		/// <param name="returnedAttributes">(Optional) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
		/// <returns>A list of groups that match the attribute value supplied.</returns>
		public List<IGroup> GetGroupsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
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
				List<GraphGroup> searchResults = GetGroupsByAttribute(attribute.Name, attribute.Value, attributeNames);

				// Filter the list of entries returned so that only Users are returned.
				List<IGroup> groups = new();
				if (searchResults != null)
				{
					foreach (GraphGroup graphGroup in searchResults)
					{
						Group group = new(this, graphGroup);
						groups.Add((Group)group);
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
		/// Gets Microsoft Graph Groups that match a given attribute value in the supplied attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search against.</param>
		/// <param name="attributeValue">The value to search for in the attribute.</param>
		/// <param name="attributeNames">(Optional) The attributes that should be returned in the entry found. If not provided, all non-constructed attributes are returned. Constructed attributes must be explicitly defined.</param>
		/// <returns></returns>
		public List<GraphGroup> GetGroupsByAttribute(string attributeName, string attributeValue, List<string> attributeNames = null)
		{
			if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
			{
				//Check if custom list of attributes exists.
				if (attributeNames == null)
				{
					attributeNames = DefaultUserAttributes.ToList();
				}

				// Create the search filter string that will find the entry in the directory with
				// the specified attribute value.
				string filter = attributeName + " eq \'" + attributeValue + "\'";

				//Create list to hold search results.
				List<GraphGroup> groups = new();

				//Query Graph Client for users matching search filter.
				Task<IGraphServiceGroupsCollectionPage> response = gsc.Groups.Request().Header("ConsistencyLevel", "eventual").Filter(filter).Select(string.Join(',', attributeNames)).GetAsync();
				response.Wait();
				groups.AddRange(response.Result.CurrentPage);

				//Check for additional pages of data.
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

		/// <summary>
		/// Gets the group membership of the specified object.
		/// </summary>
		/// <param name="id">Id of the object.</param>
		/// <param name="recursive">If true, performs a transitive search of membership.</param>
		/// <returns>List of Group objects that this object is a member of.</returns>
		public List<IGroup> GetGroupMembership(string id, bool recursive)
		{
			if(!String.IsNullOrWhiteSpace(id))
            {
				List<DirectoryObject> groups = new();
				List<GraphGroup> gg = new();

				if(recursive)
                {
					// Perform recursive membership lookup.
					Task<IUserTransitiveMemberOfCollectionWithReferencesPage> response = gsc.Users[id].TransitiveMemberOf.Request().GetAsync();
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

				foreach(var obj in groups)
                {
					// Fix later when directory roles are implemented.
					if(obj.ODataType == "#microsoft.graph.group")
                    {
						gg.Add((GraphGroup)obj);
					}
                }

				return gg.ConvertAll(graphGroup => new Group(this, graphGroup)).ConvertAll<IGroup>(x => x);
			}

			return null;
		}

		/// <summary>
        /// Gets all users that are members of the specified group.
        /// </summary>
        /// <param name="id">The unique identifier of the group.</param>
        /// <param name="recursive">If true, performs a transitive search of membership.</param>
        /// <returns>List of User objects that are members of this group.</returns>
		public List<IUser> GetUserMembers(string id, bool recursive)
        {
			List<IUser> users = new ();

			var objects = GetMembers(id, recursive);

			foreach(GraphDirectoryObject obj in objects)
            {
				if(obj.ODataType == "#microsoft.graph.user")
                {
					users.Add(new User(this,(GraphUser)obj));
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
		public List<IGroup> GetGroupMembers(string id, bool recursive)
        {
			List<IGroup> groups = new();

			var objects = GetMembers(id, recursive);

			foreach (GraphDirectoryObject obj in objects)
			{
				if (obj.ODataType == "#microsoft.graph.group")
				{
					groups.Add(new Group(this, (GraphGroup)obj));
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
	}
}
