using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing available Okta Application Authentication Schemes.
    /// </summary>
    public record ApplicationAuthenticationScheme
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Administrator sets username and password.
        /// </summary>
        public const string ADMIN_SETS_CREDENTIALS = "ADMIN_SETS_CREDENTIALS";

        /// <summary>
        /// Administrator sets username, user sets password.
        /// </summary>
        public const string EDIT_PASSWORD_ONLY = "EDIT_PASSWORD_ONLY";

        /// <summary>
        /// User sets username and password.
        /// </summary>
        public const string EDIT_USERNAME_AND_PASSWORD = "EDIT_USERNAME_AND_PASSWORD";

        /// <summary>
        /// Administrator sets username, password is the same as user's Okta password.
        /// </summary>
        public const string EXTERNAL_PASSWORD_SYNC = "EXTERNAL_PASSWORD_SYNC";

        /// <summary>
        /// Users share a single username and password set by administrator.
        /// </summary>
        public const string SHARED_USERNAME_AND_PASSWORD = "SHARED_USERNAME_AND_PASSWORD";

        // ----- PROPERTIES -----
    }
}
