using Galactic.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// OktaClient is a class that allows for the query and manipulation of Okta objects.
    /// </summary>
    public class OktaClient : IDirectorySystem
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The version of the API that this client utilizes.
        /// </summary>
        private const string API_VERSION = "v1";


        // The maximum number of items to gather with each request.
        private const int MAX_PAGE_SIZE = 200;

        /// <summary>
        /// The domain portion of the URL to use when connecting to a preview organization.
        /// </summary>
        private const string PREVIEW_DOMAIN = "oktapreview.com";

        /// <summary>
        /// The domain portion of the URL to use when connecting to a production organization.
        /// </summary>
        private const string PRODUCTION_DOMAIN = "okta.com";

        // ----- VARIABLES -----

        /// <summary>
        /// Okta User properties that can be used in a filter request.
        /// </summary>
        private readonly List<string> filterableUserProperties = new()
        {
            UserJson.STATUS,
            UserJson.LAST_UPDATED,
            UserJson.ID,
            UserProfileJson.LOGIN,
            UserProfileJson.EMAIL,
            UserProfileJson.FIRST_NAME,
            UserProfileJson.LAST_NAME
        };

        /// <summary>
        /// The types of groups supported by Okta. These constrain how the Group's Profile and memberships are managed.
        /// </summary>
        public enum GroupType
        {
            /// <summary>
            /// Group Profile and memberships are directly managed in Okta via static assignments or indirectly through Group rules.
            /// </summary>
            OKTA_GROUP,

            /// <summary>
            /// Group Profile and memberships are imported and must be managed within the application that imported the Group.
            /// </summary>
            APP_GROUP,

            /// <summary>
            /// Group Profile and memberships are managed by Okta and can't be modified.
            /// </summary>
            BUILT_IN
        }

        /// <summary>
        /// The REST client to use when making API calls.
        /// </summary>
        private RestClient rest = null;

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Inititalizes a client that can make requests to Okta.
        /// </summary>
        /// <param name="oktaTenantName">The subdomain name of your Okta tenant.</param>
        /// <param name="apiKey">The API key the client should use when authenticating.</param>
        /// <param name="preview">(Optional) Whether you want to interact with the preview organization instead of production. Defaults to production.</param>
        public OktaClient(string oktaTenantName, string apiKey, bool preview = false)
        {
            if (!string.IsNullOrWhiteSpace(oktaTenantName) && !string.IsNullOrWhiteSpace(apiKey))
            {
                // Choose the domain to use.
                string organizationDomain = PRODUCTION_DOMAIN;
                if (preview)
                {
                    // Use the preview organization domain instead.
                    organizationDomain = PREVIEW_DOMAIN;
                }

                // Initialize the HTTP client.
                string baseUri = "https://" + oktaTenantName + "." + organizationDomain + "/api/" + API_VERSION;
                string authorizationHeaderScheme = "SSWS";
                rest = new(baseUri, authorizationHeaderScheme, apiKey);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(oktaTenantName))
                {
                    throw new ArgumentNullException(nameof(oktaTenantName));
                }
                else
                {
                    throw new ArgumentNullException(nameof(apiKey));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Add a user to a group.
        /// </summary>
        /// <param name="userId">The unique id of the user to add.</param>
        /// <param name="groupId">The unique id of the group to add the user to.</param>
        /// <returns>True if the user was added, false otherwise.</returns>
        public bool AddUserToGroup(string userId, string groupId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(groupId))
            {
                EmptyRestResponse response = rest.Post("/groups/" + groupId + "/users/" + userId);

                if (response != null)
                {
                    HttpResponseMessage message =response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was added.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not addded.
                        return false;
                    }
                }
                else
                {
                    // The request was not successful. The user was not added.
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            else
            {
                throw new ArgumentNullException(nameof(groupId));
            }
        }

        /// <summary>
        /// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
        /// </summary>
        /// <param name="name">The proposed name of the group.</param>
        /// <param name="type">(Ignored) The type of group to create. All Okta groups that are manually created are of the type OKTA_GROUP.</param>
        /// <param name="parentUniqueId">(Ignored) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Optional) Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object, or null if it could not be created.</returns>
        public IGroup CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    // Set the description if supplied.
                    string description = "";
                    if (additionalAttributes != null)
                    {
                        foreach (IdentityAttribute<Object> attribute in additionalAttributes)
                        {
                            if (attribute.Name == "description")
                            {
                                description = (string)attribute.Value;
                            }
                        }
                    }

                    // Create the profile for the group to submit with the request.
                    GroupProfileJson profile = new()
                    {
                        Name = name,
                        Description = description
                    };

                    // Send the POST request.
                    JsonRestResponse<GroupJson> jsonResponse = rest.PostAsJson<GroupJson>("/groups", profile);
                    if (jsonResponse != null)
                    {
                        // Convert to an OktaJsonRestResponse.
                        OktaJsonRestResponse<GroupJson> oktaResponse = OktaJsonRestResponse<GroupJson>.FromJsonRestResponse(jsonResponse);

                        return new Group(this, oktaResponse.Value);
                    }
                    else
                    {
                        // The resquest wasn't successful.
                        return null;
                    }
                }
                catch
                {
                    // There was an error and the group was not created.
                    return null;
                }
            }
            else
            {
                // A name wasn't specified.
                return null;
            }
        }

        /// <summary>
        /// Creates a user within the directory system given it's login, and other options attributes.
        /// </summary>
        /// <param name="login">The proposed login of the user.</param>
        /// <param name="parentUniqueId">(Ignored) The unique id of the object that will be the parent of the user. Defaults to the standard user create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">Optional: Additional attribute values to set when creating the user. (Malformed or incorrect attributes are skipped.)</param>
        /// <returns>The newly created user object, or null if it could not be created.</returns>
        public IUser CreateUser(string login, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
        {
            if (!string.IsNullOrWhiteSpace(login))
            {
                try
                {
                    // Set the additional attributes if supplied.

                    // Profile properties.
                    string city = null;
                    string costCenter = null;
                    string countryCode = null;
                    string department = null;
                    string displayName = null;
                    string division = null;
                    string email = null;
                    string employeeNumber = null;
                    string firstName = null;
                    string honorificPrefix = null;
                    string honorificSuffix = null;
                    string lastName = null;
                    string locale = null;
                    string manager = null;
                    string managerId = null;
                    string middleName = null;
                    string mobilePhone = null;
                    string nickName = null;
                    string organization = null;
                    string postalAddress = null;
                    string primaryPhone = null;
                    string profileUrl = null;
                    string secondEmail = null;
                    string state = null;
                    string streetAddress = null;
                    string timeZone = null;
                    string userType = null;
                    string zipCode = null;

                    // Credentials properties.
                    UserPasswordJson password = null;
                    UserRecoveryQuestionJson recoveryQuestion = null;
                    UserProviderJson provider = null;

                    if (additionalAttributes != null)
                    {
                        foreach (IdentityAttribute<Object> attribute in additionalAttributes)
                        {
                            switch (attribute.Name)
                            {
                                case UserProfileJson.CITY:
                                    city = (string)attribute.Value;
                                    break;
                                case UserProfileJson.COST_CENTER:
                                    costCenter = (string)attribute.Value;
                                    break;
                                case UserProfileJson.COUNTRY_CODE:
                                    countryCode = (string)attribute.Value;
                                    break;
                                case UserProfileJson.DEPARTMENT:
                                    department = (string)attribute.Value;
                                    break;
                                case UserProfileJson.DISPLAY_NAME:
                                    displayName = (string)attribute.Value;
                                    break;
                                case UserProfileJson.DIVISION:
                                    division = (string)attribute.Value;
                                    break;
                                case UserProfileJson.EMAIL:
                                    email = (string)attribute.Value;
                                    break;
                                case UserProfileJson.EMPLOYEE_NUMBER:
                                    employeeNumber = (string)attribute.Value;
                                    break;
                                case UserProfileJson.FIRST_NAME:
                                    firstName = (string)attribute.Value;
                                    break;
                                case UserProfileJson.HONORIFIC_PREFIX:
                                    honorificPrefix = (string)attribute.Value;
                                    break;
                                case UserProfileJson.HONORIFIC_SUFFIX:
                                    honorificSuffix = (string)attribute.Value;
                                    break;
                                case UserProfileJson.LAST_NAME:
                                    lastName = (string)attribute.Value;
                                    break;
                                case UserProfileJson.LOCALE:
                                    locale = (string)attribute.Value;
                                    break;
                                case UserProfileJson.MANAGER:
                                    manager = (string)attribute.Value;
                                    break;
                                case UserProfileJson.MANAGER_ID:
                                    managerId = (string)attribute.Value;
                                    break;
                                case UserProfileJson.MIDDLE_NAME:
                                    middleName = (string)attribute.Value;
                                    break;
                                case UserProfileJson.MOBILE_PHONE:
                                    mobilePhone = (string)attribute.Value;
                                    break;
                                case UserProfileJson.NICK_NAME:
                                    nickName = (string)attribute.Value;
                                    break;
                                case UserProfileJson.ORGANIZATION:
                                    organization = (string)attribute.Value;
                                    break;
                                case UserProfileJson.POSTAL_ADDRESS:
                                    postalAddress = (string)attribute.Value;
                                    break;
                                case UserProfileJson.PRIMARY_PHONE:
                                    primaryPhone = (string)attribute.Value;
                                    break;
                                case UserProfileJson.PROFILE_URL:
                                    profileUrl = (string)attribute.Value;
                                    break;
                                case UserProfileJson.SECOND_EMAIL:
                                    secondEmail = (string)attribute.Value;
                                    break;
                                case UserProfileJson.STATE:
                                    state = (string)attribute.Value;
                                    break;
                                case UserProfileJson.STREET_ADDRESS:
                                    streetAddress = (string)attribute.Value;
                                    break;
                                case UserProfileJson.TIME_ZONE:
                                    timeZone = (string)attribute.Value;
                                    break;
                                case UserProfileJson.USER_TYPE:
                                    userType = (string)attribute.Value;
                                    break;
                                case UserProfileJson.ZIP_CODE:
                                    zipCode = (string)attribute.Value;
                                    break;
                                case UserCredentialsJson.PASSWORD:
                                    if (attribute.Value is UserPasswordJson)
                                    {
                                        password = (UserPasswordJson)attribute.Value;
                                    }
                                    break;
                                case UserCredentialsJson.RECOVERY_QUESTION:
                                    if (attribute.Value is UserRecoveryQuestionJson)
                                    {
                                        recoveryQuestion = (UserRecoveryQuestionJson)attribute.Value;
                                    }
                                    break;
                                case UserCredentialsJson.PROVIDER:
                                    if (attribute.Value is UserProviderJson)
                                    {
                                        provider = (UserProviderJson)attribute.Value;
                                    }
                                    break;
                            }
                        }
                    }

                    // Create the profile for the user to submit with the request.
                    UserProfileJson profile = new()
                    {
                        City = city,
                        CostCenter = costCenter,
                        CountryCode = countryCode,
                        Department = department,
                        DisplayName = displayName,
                        Division = division,
                        Email = email,
                        EmployeeNumber = employeeNumber,
                        FirstName = firstName,
                        HonorificPrefix = honorificPrefix,
                        HonorificSuffix = honorificSuffix,
                        LastName = lastName,
                        Locale = locale,
                        Login = login,
                        Manager = manager,
                        ManagerId = managerId,
                        MiddleName = middleName,
                        MobilePhone = mobilePhone,
                        NickName = nickName,
                        Organization = organization,
                        PostalAddress = postalAddress,
                        PrimaryPhone = primaryPhone,
                        ProfileUrl = profileUrl,
                        SecondEmail = secondEmail,
                        State = state,
                        StreetAddress = streetAddress,
                        TimeZone = timeZone,
                        UserType = userType,
                        ZipCode = zipCode
                    };

                    // Create the credentials profile for the user to submit with the request.
                    UserCredentialsJson credentials = null;

                    // Check whether credentials properties were defined.
                    if (password != null || recoveryQuestion != null || provider != null)
                    {
                        credentials = new()
                        {
                            Password = password,
                            RecoveryQuestion = recoveryQuestion,
                            Provider = provider
                        };
                    }

                    // Send the POST request.
                    JsonRestResponse<UserJson> jsonResponse = null;

                    if (credentials != null)
                    {
                        // Create a request with both profile and credentials data.
                        UserProfileAndCredentialsJson profileAndCredentials = new()
                        {
                            Profile = profile,
                            Credentials = credentials
                        };
                        jsonResponse = rest.PostAsJson<UserJson>("/users", profileAndCredentials);

                    }
                    else
                    {
                        // Create a request with profile data only.
                        jsonResponse = rest.PostAsJson<UserJson>("/users", profile);
                    }

                    if (jsonResponse != null)
                    {
                        // Convert to an OktaJsonRestResponse.
                        OktaJsonRestResponse<UserJson> oktaResponse = OktaJsonRestResponse<UserJson>.FromJsonRestResponse(jsonResponse);

                        return new User(this, oktaResponse.Value);
                    }
                    else
                    {
                        // The resquest wasn't successful.
                        return null;
                    }
                }
                catch
                {
                    // There was an error and the user was not created.
                    return null;
                }
            }
            else
            {
                // A login wasn't specified.
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
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                EmptyRestResponse response = rest.Delete("/groups/" + uniqueId);

                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The group was deleted.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The group was not deleted.
                        return false;
                    }
                }
                else
                {
                    // The request was not successful. The group was not deleted.
                    return false;
                }
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
                EmptyRestResponse response = rest.Delete("/users/" + uniqueId);

                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was deleted.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not deleted.
                        return false;
                    }
                }
                else
                {
                    // The request was not successful. The user was not deleted.
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Gets all the JSON property names associated with the supplied type.
        /// </summary>
        public static List<string> GetAllJsonPropertyNames(Type type)
        {
            // Create a list of all the JSON property names.
            PropertyInfo[] propertyInfoList = type.GetProperties();
            List<string> names = new();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                foreach (JsonPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<JsonPropertyNameAttribute>())
                {
                    names.Add(attribute.Name);
                }
            }
            return names;
        }

        /// <summary>
        /// Get's all users in the directory system.
        /// </summary>
        /// <returns>A list of all users in the directory system.</returns>
        public List<IUser> GetAllUsers()
        {
            // Return the result.
            JsonRestResponse<UserJson[]> jsonResponse = rest.GetFromJson<UserJson[]>("/users/?limit=" + MAX_PAGE_SIZE);
            if (jsonResponse != null)
            {
                // Convert to an OktaJsonRestResponse.
                OktaJsonRestResponse<UserJson[]> oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                // Create the list of user JSON objects.
                List<UserJson> jsonList = new(oktaResponse.Value);

                // Get additional pages.
                while (oktaResponse.NextPage != null)
                {
                    // Get the next page, removing the base URI from the supplied URI.
                    jsonResponse = rest.GetFromJson<UserJson[]>(oktaResponse.NextPage.ToString().Replace(rest.BaseUri, ""));

                    if (jsonResponse != null)
                    {
                        // Convert to OktaJsonRestResponse.
                        oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                        // Add the additional users to the list.
                        jsonList.AddRange(oktaResponse.Value);
                    }
                }

                // Create the list users to return.
                List<IUser> users = new();
                foreach (UserJson userJson in jsonList)
                {
                    users.Add(new User(this, userJson));
                }

                // Return the list of users.
                return users;
            }
            else
            {
                // Nothing was returned.
                return new();
            }
        }

        /// <summary>
        /// Gets a Group from Okta given its id.
        /// </summary>
        /// <param name="id">The id of the Group to retrieve from Okta.</param>
        /// <returns>A Group object.</returns>
        public Group GetGroup(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                // Return the result.
                JsonRestResponse<GroupJson> jsonResponse = rest.GetFromJson<GroupJson>("/groups/" + id);
                if (jsonResponse != null)
                {
                    // Convert to an OktaJsonRestResponse.
                    OktaJsonRestResponse<GroupJson> oktaResponse = OktaJsonRestResponse<GroupJson>.FromJsonRestResponse(jsonResponse);

                    return new Group(this, oktaResponse.Value);
                }
                else
                {
                    // Nothing was returned.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(id));
            }
        }

        /// <summary>
        /// Gets a list of users that are a member of the supplied group.
        /// </summary>
        /// <param name="uniqueId">The unique id of the group.</param>
        /// <returns>A list of UserJsons objects representing each user that is a member of the group.</returns>
        public List<UserJson> GetGroupMembership(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // Return the result.
                JsonRestResponse<UserJson[]> jsonResponse = rest.GetFromJson<UserJson[]>("/groups/" + uniqueId + "/users?limit=" + MAX_PAGE_SIZE);
                if (jsonResponse != null)
                {
                    // Convert to an OktaJsonRestResponse.
                    OktaJsonRestResponse<UserJson[]> oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                    // Create the list of UserJson objects to return.
                    List<UserJson> jsonList = new(oktaResponse.Value);

                    // Get additional pages.
                    while (oktaResponse.NextPage != null)
                    {
                        // Get the next page, removing the base URI from the supplied URI.
                        jsonResponse = rest.GetFromJson<UserJson[]>(oktaResponse.NextPage.ToString().Replace(rest.BaseUri, ""));

                        if (jsonResponse != null)
                        {
                            // Convert to OktaJsonRestResponse.
                            oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                            // Add the additional users to the list.
                            jsonList.AddRange(oktaResponse.Value);
                        }
                    }

                    return jsonList;
                }
                else
                {
                    // Nothing was returned. Return an empty list.
                    return new();
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Gets a list of the types of groups supported by the directory system.
        /// </summary>
        /// <returns>A list of strings with the names of the types of groups supported by the system.</returns>
        public List<string> GetGroupTypes()
        {
            List<string> types = new();
            foreach(GroupType type in (GroupType[]) Enum.GetValues(typeof(GroupType)))
            {
                types.Add(type.ToString());
            }
            return types;
        }

        /// <summary>
        /// Gets IGroups that start with the attribute value in the supplied attribute.
        /// Note: Only searches Okta groups of type OKTA_GROUP.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Ignored: N/A for Okta) The attributes that should be returned in the group found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of users that match the attribute value supplied.</returns>
        public List<IGroup> GetGroupsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
        {
            return GetGroupsByAttributeAndType(attribute);
        }

        /// <summary>
        /// Gets IGroups that start with the attribute value in the supplied attribute and of the specified group type.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="groupType">(Optional) The Okta type of the group to search for. Defaults to OKTA_GROUP.</param>
        /// <returns>A list of groups that match the attribute value supplied and of the supplied type.</returns>
        public List<IGroup> GetGroupsByAttributeAndType(IdentityAttribute<string> attribute, GroupType groupType = GroupType.OKTA_GROUP)
        {
            // Check whether an attribute was supplied.
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                // An attribute was supplied.
               JsonRestResponse<GroupJson[]> jsonResponse = null;

                // Get the name of the property for use when searching.
                string searchPropertyName = Group.GetSearchPropertyName(attribute.Name, groupType);

                // Check if the name is a valid property name.
                if (!string.IsNullOrWhiteSpace(searchPropertyName))
                {
                    // The name is a valid property named.

                    // Use a search request to search for the group.
                    jsonResponse = rest.GetFromJson<GroupJson[]>("/groups?search=" + searchPropertyName + "%20sw%20%22" + attribute.Value + "%22&limit=" + MAX_PAGE_SIZE);
                }

                if (jsonResponse != null)
                {
                    // Creates an OktaJsonResponse object from the JSON one.
                    OktaJsonRestResponse<GroupJson[]> oktaResponse = OktaJsonRestResponse<GroupJson[]>.FromJsonRestResponse(jsonResponse);

                    // Create the list of group JSON objects.
                    List<GroupJson> jsonList = new(oktaResponse.Value);

                    // Get additional pages.
                    while (oktaResponse.NextPage != null)
                    {
                        // Get the next page, removing the base URI from the supplied URI.
                        jsonResponse = rest.GetFromJson<GroupJson[]>(oktaResponse.NextPage.ToString().Replace(rest.BaseUri, ""));

                        if (jsonResponse != null)
                        {
                            // Convert to OktaJsonRestResponse.
                            oktaResponse = OktaJsonRestResponse<GroupJson[]>.FromJsonRestResponse(jsonResponse);

                            // Add the additional groups to the list.
                            jsonList.AddRange(oktaResponse.Value);
                        }
                    }

                    // Create the list groups to return.
                    List<IGroup> groups = new();
                    foreach (GroupJson groupJson in jsonList)
                    {
                        groups.Add(new Group(this, groupJson));
                    }

                    // Return the list of groups.
                    return groups;
                }
                else
                {
                    // Nothing was returned.
                    return new();
                }
            }
            else
            {
                // An attribute was not supplied.
                return new();
            }
        }

        /// <summary>
        /// Gets IUsers that start with the attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Ignored: N/A for Okta) The attributes that should be returned in the user found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of users that match the attribute value supplied.</returns>
        public List<IUser> GetUsersByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<Object>> returnedAttributes = null)
        {
            // Check whether an attribute was supplied.
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                // An attribute was supplied.
                JsonRestResponse<UserJson[]> jsonResponse = null;

                // Get the name of the property for use when searching.
                string searchPropertyName = User.GetSearchPropertyName(attribute.Name);

                // Check if the name is a valid property name.
                if (!string.IsNullOrWhiteSpace(searchPropertyName))
                {
                    // The name is a valid property named.

                    // Check whether to send a filter or search request.
                    if (filterableUserProperties.Contains(attribute.Name))
                    {
                        // Use a filter request to search for the user. (This uses the most up to date information in Okta.)
                        jsonResponse = rest.GetFromJson<UserJson[]>("/users/?filter=" + attribute.Name + "%20sw%20%22" + attribute.Value + "%22&limit=" + MAX_PAGE_SIZE);
                    }
                    else
                    {
                        // Use a search request to search for the user. (This uses a search index which may not contain the most up to date information in Okta.)
                        jsonResponse = rest.GetFromJson<UserJson[]>("/users/?search=" + attribute.Name + "%20sw%20%22" + attribute.Value + "%22&limit=" + MAX_PAGE_SIZE);
                    }
                }

                if (jsonResponse != null)
                {
                    // Convert to an OktaJsonRestResponse.
                    OktaJsonRestResponse<UserJson[]> oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                    // Create the list of user JSON objects.
                    List<UserJson> jsonList = new(oktaResponse.Value);

                    // Get additional pages.
                    while (oktaResponse.NextPage != null)
                    {
                        // Get the next page, removing the base URI from the supplied URI.
                        jsonResponse = rest.GetFromJson<UserJson[]>(oktaResponse.NextPage.ToString().Replace(rest.BaseUri, ""));

                        if (jsonResponse != null)
                        {
                            // Convert to OktaJsonRestResponse.
                            oktaResponse = OktaJsonRestResponse<UserJson[]>.FromJsonRestResponse(jsonResponse);

                            // Add the additional users to the list.
                            jsonList.AddRange(oktaResponse.Value);
                        }
                    }

                    // Create the list users to return.
                    List<IUser> users = new();
                    foreach (UserJson userJson in jsonList)
                    {
                        users.Add(new User(this, userJson));
                    }

                    // Return the list of users.
                    return users;
                }
                else
                {
                    // Nothing was returned.
                    return new();
                }
            }
            else
            {
                // An attribute was not supplied.
                return new ();
            }
        }

        /// <summary>
        /// Gets a list of groups that the user is a member of.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user.</param>
        /// <returns>A list of GroupJsons objects representing each group the user is a member of, or null if there was an error retrieving the list.</returns>
        public List<GroupJson> GetUserGroups(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // Return the result.
                JsonRestResponse<GroupJson[]> jsonResponse = rest.GetFromJson<GroupJson[]>("/users/" + uniqueId + "/groups");
                if (jsonResponse != null)
                {
                    // Convert to OktaJsonRestResponse.
                    OktaJsonRestResponse<GroupJson[]> oktaResponse = OktaJsonRestResponse<GroupJson[]>.FromJsonRestResponse(jsonResponse);

                    GroupJson[] jsonArray = oktaResponse.Value;
                    if (jsonArray != default)
                    {
                        // Return the list of Groups.
                        return new(jsonArray);
                    }
                    else
                    {
                        // Nothing was returned. Return an empty list.
                        return new();
                    }
                }
                else
                {
                    // There was an error with the request.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Remove a user from a group.
        /// </summary>
        /// <param name="userId">The unique id of the user to remove.</param>
        /// <param name="groupId">The unique id of the group to remove the user from.</param>
        /// <returns>True if the user was removed, false otherwise.</returns>
        public bool RemoveUserFromGroup(string userId, string groupId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(groupId))
            {
                EmptyRestResponse response = rest.Delete("/groups/" + groupId + "/users/" + userId);

                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was removed.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not removed.
                        return false;
                    }
                }
                else
                {
                    // The request was not successful. The user was not removed.
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            else
            {
                throw new ArgumentNullException(nameof(groupId));
            }
        }

        /// <summary>
        /// Suspends a user, preventing them from logging in.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to suspend.</param>
        /// <returns>True if the user was suspended, false otherwise.</returns>
        public bool SuspendUser(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                EmptyRestResponse response = rest.Post("/users/" + uniqueId + "/lifecycle/suspend");
                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was suspended.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not suspended.
                        return false;
                    }
                }
                else
                {
                    // There was an error with the requestion.
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Unlocks a user returning them to an active state and allowing them to log in.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to unlock.</param>
        /// <returns>True if the user was unlocked, false otherwise.</returns>
        public bool UnlockUser(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                EmptyRestResponse response = rest.Post("/users/" + uniqueId + "/lifecycle/unlock");
                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was unlocked.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not unlocked.
                        return false;
                    }
                }
                else
                {
                    // There was an error with the request.
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Unsuspends a user returning them to an active state and allowing them to log in.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to unsuspend.</param>
        /// <returns>True if the user was unsuspended, false otherwise.</returns>
        public bool UnsuspendUser(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                EmptyRestResponse response = rest.Post("/users/" + uniqueId + "/lifecycle/unsuspend");
                if (response != null)
                {
                    HttpResponseMessage message = response.Message;

                    // Check that the request was a success.
                    if (message.IsSuccessStatusCode)
                    {
                        // The request was successful. The user was unsuspended.
                        return true;
                    }
                    else
                    {
                        // The request was not successful. The user was not unsuspended.
                        return false;
                    }
                }
                else
                {
                    // There was an error with the request.
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Updates a group's profile attributes.
        /// </summary>
        /// <param name="uniqueId">The unique id of the group to update.</param>
        /// <param name="profile">The json representing the all the properties of the group. Note: Include all properties, not just those that will be updated.</param>
        /// <returns>A new GroupJson object representing the new state of the group after the update, or null if the update was not completed.</returns>
        public GroupJson UpdateGroup(string uniqueId, GroupProfileJson profile)
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }

            if (profile != null)
            {
                // The JSON object representing the updated group.
                GroupJson group = null;

                if (profile != null)
                {
                    // Update the properties.
                    JsonRestResponse<GroupJson> jsonResponse = rest.PostAsJson<GroupJson>("/groups/" + uniqueId, profile);
                    if (jsonResponse != null)
                    {
                        // Convert to an OktaJsonRestResponse.
                        OktaJsonRestResponse<GroupJson> oktaResponse = OktaJsonRestResponse<GroupJson>.FromJsonRestResponse(jsonResponse);

                        group = oktaResponse.Value;
                    }
                    else
                    {
                        // There was an error with the request.
                        return null;
                    }
                }

                // Return the group's JSON object.
                return group;
            }
            else
            {
                // There was nothing to update.
                return null;
            }
        }

        /// <summary>
        /// Updates a user's profile and/or credentials. If both are supplied, they will be updated in one request.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user to update.</param>
        /// <param name="profile">(Optional) The json representing the properties that should be updated. Only the properties that need to be updated should be populated.</param>
        /// <param name="creds">(Optional) The json representing the credentials that should be updated.</param>
        /// <returns>A new UserJson object representing the new state of the user after the update, or null if the update was not completed.</returns>
        public UserJson UpdateUser(string uniqueId, UserProfileJson profile = null, UserCredentialsJson creds = null)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // TODO: Collapse these into a single request, if both aren't null.
                if (profile != null || creds != null)
                {
                    // The JSON object representing the updated user.
                    UserJson user = null;

                    if (profile != null)
                    {
                        // Update the properties.
                        JsonRestResponse<UserJson> jsonResponse = rest.PostAsJson<UserJson>("/users/" + uniqueId, profile);
                        if (jsonResponse != null)
                        {
                            // Convert to an OktaJsonRestResponse.
                            OktaJsonRestResponse<UserJson> oktaResponse = OktaJsonRestResponse<UserJson>.FromJsonRestResponse(jsonResponse);

                            user = oktaResponse.Value;
                        }
                        else
                        {
                            // There was an error with the request.
                            return null;
                        }
                    }

                    if (creds != null)
                    {
                        // Update the credentials.
                        JsonRestResponse<UserJson> jsonResponse = rest.PostAsJson<UserJson>("/users/" + uniqueId, creds);
                        if (jsonResponse != null)
                        {
                            // Convert to an OktaJsonRestResponse.
                            OktaJsonRestResponse<UserJson> oktaResponse = OktaJsonRestResponse<UserJson>.FromJsonRestResponse(jsonResponse);

                            user = oktaResponse.Value;
                        }
                        else
                        {
                            // There was an error with the request.
                            return null;
                        }
                    }

                    // Return the user's JSON object.
                    return user;
                }
                else
                {
                    // There was nothing to update.
                    return null;
                }
            }
            else
            { 
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }
    }
}
