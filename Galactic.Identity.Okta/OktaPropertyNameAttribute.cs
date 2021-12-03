using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A class representing an Attribute that can be applied to Okta related properties.
    /// The attribute defines the name that the properity is known by within Okta.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OktaPropertyNameAttribute : Attribute
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----
        private string name = null;

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates an attribute defining the name of the Okta property.
        /// </summary>
        /// <param name="name">The name of the property in Okta.</param>
        public OktaPropertyNameAttribute(string name)
        {
            this.name = name;
        }

        // ----- METHODS -----
    }
}
