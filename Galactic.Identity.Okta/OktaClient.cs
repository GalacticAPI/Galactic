﻿using Galactic.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
            if (!string.IsNullOrWhiteSpace(oktaTenantName) && !string.IsNullOrWhiteSpace(apiKey) && rest != null)
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
                string authorizationHeader = "SSWS " + apiKey;
                rest = new(baseUri, authorizationHeader);
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
        /// Create a new group within the directory system given its proposed name, its type, and other optional attributes.
        /// </summary>
        /// <param name="name">The proposed name of the group.</param>
        /// <param name="type">(Ignored) The type of group to create. All Okta groups that are manually created are of the type OKTA_GROUP.</param>
        /// <param name="parentUniqueId">(Optional) The unique id of the object that will be the parent of the group. Defaults to the standard group create location for the system if not supplied or invalid.</param>
        /// <param name="additionalAttributes">(Optional) Additional attributes to set when creating the group.</param>
        /// <returns>The newly created group object, or null if it could not be created.</returns>
        public IGroup CreateGroup(string name, string type, string parentUniqueId = null, List<IdentityAttribute<object>> additionalAttributes = null)
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
                OktaJsonRestResponse<GroupJson> response = (OktaJsonRestResponse<GroupJson>)rest.PostAsJson<GroupJson>("/groups", profile);
                if (response != null)
                {
                    return new Group(this, response.Value);
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
        /// Deletes an object with the specified unique id from the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to delete.</param>
        /// <returns>True if the object was deleted, false otherwise.</returns>
        public bool DeleteObject(string uniqueId)
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
        /// Gets a Group from Okta given its id.
        /// </summary>
        /// <param name="id">The id of the Group to retrieve from Okta.</param>
        /// <returns>A Group object.</returns>
        public Group GetGroup(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                // Return the result.
                OktaJsonRestResponse<GroupJson> response = (OktaJsonRestResponse<GroupJson>)rest.GetFromJson<GroupJson>("/groups/" + id);
                if (response != null)
                {
                    return new Group(this, response.Value);
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
                OktaJsonRestResponse<UserJson[]> response = (OktaJsonRestResponse<UserJson[]>)rest.GetFromJson<UserJson[]>("/groups/" + uniqueId + "/users&limit=" + MAX_PAGE_SIZE);
                if (response != null)
                {
                    // Create the list of UserJson objects to return.
                    List<UserJson> jsonList = new(response.Value);

                    // Get additional pages.
                    while (response.NextPage != null)
                    {
                        // Get the next page.
                        response = (OktaJsonRestResponse<UserJson[]>)rest.GetFromJson<UserJson[]>(response.NextPage.ToString());

                        // Add the additional users to the list.
                        jsonList.AddRange(response.Value);
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
        /// Gets identity objects that match wildcarded (*) attribute value in the supplied attribute.
        /// </summary>
        /// <param name="attribute">The attribute with name and value to search against.</param>
        /// <param name="returnedAttributes">(Optional) The attributes that should be returned in the object found. If not supplied, the default list of attributes is returned.</param>
        /// <returns>A list of idenity objects that match the attribute value supplied.</returns>
        public List<IIdentityObject> GetObjectsByAttribute(IdentityAttribute<string> attribute, List<IdentityAttribute<object>> returnedAttributes = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of groups that the user is a member of.
        /// </summary>
        /// <param name="uniqueId">The unique id of the user.</param>
        /// <returns>A list of GroupJsons objects representing each group the user is a member of.</returns>
        public List<GroupJson> GetUserGroups(string uniqueId)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                // Return the result.
                GroupJson[] jsonArray = rest.GetFromJson<GroupJson[]>("/users/" + uniqueId + "/groups").Value;
                if (jsonArray != default)
                {
                    // Return the list of Groups.
                    return new(jsonArray);
                }
                else
                {
                    // Nothing was returned. Return an empty list.
                    return new ();
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }
        }

        /// <summary>
        /// Moves an object in the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to move.</param>
        /// <param name="parentUniqueId">The unique id of the object that will be the new parent of the object.</param>
        /// <returns>True if the object was moved, false otherwise.</returns>
        public bool MoveObject(string uniqueId, string parentUniqueId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renames an object in the directory system.
        /// </summary>
        /// <param name="uniqueId">The unique id of the object to rename.</param>
        /// <param name="name"The new name of the object.</param>
        /// <returns>True if the object was renamed, false otherwise.</returns>
        public bool RenameObject(string uniqueId, string name)
        {
            throw new NotImplementedException();
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
                HttpResponseMessage message = rest.Post("/users/" + uniqueId + "/lifecycle/suspend").Message;

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
                HttpResponseMessage message = rest.Post("/users/" + uniqueId + "/lifecycle/unlock").Message;

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
                HttpResponseMessage message = rest.Post("/users/" + uniqueId + "/lifecycle/unsuspend").Message;

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
                    group = rest.PostAsJson<GroupJson>("/groups/" + uniqueId, profile).Value;
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
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                throw new ArgumentNullException(nameof(uniqueId));
            }

            // TODO: Collapse these into a single request, if both aren't null.
            if (profile != null || creds != null)
            {
                // The JSON object representing the updated user.
                UserJson user = null;

                if (profile != null)
                {
                    // Update the properties.
                    user = rest.PostAsJson<UserJson>("/users/" + uniqueId, profile).Value;
                }

                if (creds != null)
                {
                    // Update the credentials.
                    user = rest.PostAsJson<UserJson>("/users/" + uniqueId, creds).Value;
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
    }
}
