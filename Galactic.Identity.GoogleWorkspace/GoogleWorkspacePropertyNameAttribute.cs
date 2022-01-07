using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.GoogleWorkspace
{
    /// <summary>
    /// A class representing an Attribute that can be applied to Google related properties.
    /// The attribute defines the name that the properity is known by within Google.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GoogleWorkspacePropertyNameAttribute : Attribute
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The name of the property in Google.
        /// </summary>
        public string Name { get; set; }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates an attribute defining the name of the Google property.
        /// </summary>
        /// <param name="name">The name of the property in Google.</param>
        public GoogleWorkspacePropertyNameAttribute(string name)
        {
            Name = name;
        }

        // ----- METHODS -----
    }
}
