using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.ActiveDirectory
{
    /// <summary>
    /// ISecurityPrincipal is an interface that defines Microsoft specific functionality for all security principals
    /// (Groups and Users) in Active Directory.
    /// </summary>
    public interface ISecurityPrincipal : IActiveDirectoryObject, IMailSupportedObject
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The principal's e-mail address.
        /// </summary>
        public string EMailAddress { get; set; }

        /// <summary>
        /// The principal's Microsoft Exchange Alias.
        /// </summary>
        public string ExchangeAlias { get; }

        /// <summary>
        /// The principal's SAM Account Name.
        /// </summary>
        public string SAMAccountName { get; set; }

        /// <summary>
        /// The principal's target e-mail address. Used by Exchange for routing e-mail to its
        /// final destination which may lie outside of the organization. Allows for an object
        /// to appear in the GAL even though its e-mail address may be outside of Exchange.
        /// Also used when routing e-mail to the Microsoft Office365 cloud from an on-premises
        /// Exchange server.
        /// </summary>
        public string TargetAddress { get; set; }

        /// <summary>
        /// The User Principal Name of the principal.
        /// </summary>
        public string UserPrincipalName { get; set; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
