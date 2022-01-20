using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity
{
    /// <summary>
    /// A class representing an Attribute that can be applied to directory system related properties.
    /// The attribute defines the name that the properity is known by within a directory system.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DirectorySystemPropertyNameAttribute : Attribute
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The name of the property in the directory system.
        /// </summary>
        public string Name { get; set; }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates an attribute defining the name of the directory system property.
        /// </summary>
        /// <param name="name">The name of the property in the directory system.</param>
        public DirectorySystemPropertyNameAttribute(string name)
        {
            Name = name;
        }

        // ----- METHODS -----
    }
}