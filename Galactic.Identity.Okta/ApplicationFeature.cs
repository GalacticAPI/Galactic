using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing supported Okta Application Features.
    /// </summary>
    public record ApplicationFeature
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Creates or links a group in the app when a mapping is defined for a group in Okta.
        /// Okta is the source for group memberships and all group members and all group members
        /// in Okta who are also assigned to the app are synced as group members to the app.
        /// </summary>
        public const string GROUP_PUSH = "GROUP_PUSH";

        /// <summary>
        /// Creates or links a user in Okta to a user from the application.
        /// </summary>
        public const string IMPORT_NEW_USERS = "IMPORT_NEW_USERS";

        /// <summary>
        /// Updates a linked user's app profile during manual or scheduled imports.
        /// </summary>
        public const string IMPORT_PROFILE_UPDATES = "IMPORT_PROFILE_UPDATES";

        /// <summary>
        /// Discovers the profile schema for a user from the app automatically.
        /// </summary>
        public const string IMPORT_USER_SCHEMA = "IMPORT_USER_SCHEMA";

        /// <summary>
        /// Designates the app as the identity lifecycle and profile attribute authority for linked
        /// users. The user's profile in Okta is read-only.
        /// </summary>
        public const string PROFILE_MASTERING = "PROFILE_MASTERING";

        /// <summary>
        /// Creates or links a user account in the application when assigning the app toa a user in Okta.
        /// </summary>
        public const string PUSH_NEW_USERS = "PUSH_NEW_USERS";

        /// <summary>
        /// Updates the user's app password when their password changes in Okta.
        /// </summary>
        public const string PUSH_PASSWORD_UPDATES = "PUSH_PASSWORD_UPDATES";

        /// <summary>
        /// Updates a user's profile in the app when the user's profile changes in Okta (Profile Master).
        /// </summary>
        public const string PUSH_PROFILE_UPDATES = "PUSH_PROFILE_UPDATES";

        /// <summary>
        /// Deactivates a user's account in the app when unassigned from the app in Okta or deactivated.
        /// </summary>
        public const string PUSH_USER_DEACTIVATION = "PUSH_USER_DEACTIVATION";

        /// <summary>
        /// Reactivates an existing inactive user when provisioning a user to the app.
        /// </summary>
        public const string REACTIVATE_USERS = "REACTIVATE_USERS";

        // ----- PROPERTIES -----
    }
}
