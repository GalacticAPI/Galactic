using System;

namespace Galactic.Identity
{
    /// <summary>
    /// IDescriptionSupportedObject is an interface that defines functionality for
    /// identity objects that support having descriptions associated with them.
    /// </summary>
    public interface IDescriptionSupportedObject
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// A description of the object.
        /// </summary>
        public string Description { get; set; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
