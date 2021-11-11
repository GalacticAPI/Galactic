using System;
using System.Collections.Generic;
namespace Galactic.Identity
{
    /// <summary>
    /// IdentityAttribute represents a named attribute and its value.
    /// </summary>
    /// <typeparam name="T">The type of the attribute's value.</typeparam>
    public class IdentityAttribute<T>
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The value of the attribute.
        /// </summary>
        protected T value;


        // ----- PROPERTIES -----

        /// <summary>
        /// The attribute's name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The attribute's value.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates a new attribute with the supplied name and value.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public IdentityAttribute(string name, T value)
        {
            Name = name;
            Value = value;
        }

        // ----- METHODS -----
    }
}
