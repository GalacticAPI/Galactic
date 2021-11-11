using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// IMailEnabledObject is an interface that defines functionality for identity
    /// objects that support having e-mail addresses associated with them.
    /// </summary>
    public interface IMailSupportedObject
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public List<string> EmailAddresses { get; set; }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public string PrimaryEmailAddress { get; set; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
