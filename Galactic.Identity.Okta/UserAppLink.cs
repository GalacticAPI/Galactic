using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.Okta
{
    public class UserAppLink
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the Application in Okta.
        /// </summary>
        protected UserAppLinkJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Unique ID for the Application.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.ID)]
        public string Id => json.Id;

        /// <summary>
        /// User defined display name for app.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.LABEL)]
        public string Label => json.Label;

        /// <summary>
        /// Link to sign-in URL.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.LINK_URL)]
        public string LinkUrl => json.LinkUrl;

        /// <summary>
        /// Link to logo image.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.LOGO_URL)]
        public string LogoUrl => json.LogoUrl;

        /// <summary>
        /// Application name.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.APP_NAME)]
        public string AppName => json.AppName;

        /// <summary>
        /// Application instance ID.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.APP_INSTANCE_ID)]
        public string AppInstanceId => json.AppInstanceId;

        /// <summary>
        /// Application assignment ID.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.APP_ASSIGNMENT_ID)]
        public string AppAssignmentId => json.AppAssignmentId;

        /// <summary>
        /// Credentials setup.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.CREDENTIALS_SETUP)]
        public bool CredentialsSetup => json.CredentialsSetup;

        /// <summary>
        /// Hidden.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.HIDDEN)]
        public bool Hidden => json.Hidden;

        /// <summary>
        /// Sort order.
        /// </summary>
        [DirectorySystemPropertyName(UserAppLinkJson.SORT_ORDER)]
        public int SortOrder => json.SortOrder;

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta application from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the application.</param>
        /// <param name="json">An object representing this application's JSON properties.</param>
        public UserAppLink(OktaClient okta, UserAppLinkJson json)
        {
            if (okta != null && json != null)
            {
                // Initialize the client.
                this.okta = okta;

                // Initialize the backing JSON data.
                this.json = json;
            }
            else
            {
                if (okta == null)
                {
                    throw new ArgumentNullException(nameof(okta));
                }
                else
                {
                    throw new ArgumentNullException(nameof(json));
                }
            }
        }
    }
}
