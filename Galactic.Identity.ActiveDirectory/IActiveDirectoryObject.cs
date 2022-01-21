using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.ActiveDirectory
{
    /// <summary>
    /// IActiveDirectoryObject is an interface that defines Microsoft specific functionality for all objects
    /// in Active Directory.
    /// </summary>
    public interface IActiveDirectoryObject
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The Common Name (CN) of the object in Active Directory.
        /// </summary>
        public string CommonName { get; }

        /// <summary>
        /// The time the object was created in UTC.
        /// </summary>
        public DateTime? CreateTimeStamp { get; }

        /// <summary>
        /// The Distinguished Name (DN) of the object in Active Directory.
        /// </summary>
        public string DistinguishedName { get; }

        /// <summary>
        /// The GUID of the object in Active Directory.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// The distinguished name of the organizational unit or parent object containing the object.
        /// </summary>
        public string OrganizationalUnit { get; }

        /// <summary>
        /// The schema class types that identify the type of object this is in Active Directory.
        /// Examples: group, user, etc.
        /// </summary>
        public List<string> SchemaClasses { get; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
