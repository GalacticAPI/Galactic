using System;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    /// <summary>
    /// A class representing a user on a SharePoint system.
    /// </summary>
    public class SharePointUser
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The User object that underlies this document.
        private readonly User user = null;

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string UserName
        {
            get
            {
                return user.LoginName;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointUser object from the underlying User object.
        /// </summary>
        /// <param name="user">The base User object representing this user.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a user is not supplied.</exception>
        public SharePointUser(User user)
        {
            if (user != null)
            {
                this.user = user;
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        // ---------- METHODS ----------
    }
}
