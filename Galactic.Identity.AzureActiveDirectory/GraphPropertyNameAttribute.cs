using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.AzureActiveDirectory
{
	/// <summary>
	/// A class representing an Attribute that can be applied to Microsoft Graph related properties.
	/// The attribute defines the name that the properity is known by within Okta.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class GraphPropertyNameAttribute : Attribute
	{
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The name of the property in Microsoft Graph.
        /// </summary>
        public string Name { get; set; }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates an attribute defining the name of the Graph property.
        /// </summary>
        /// <param name="name">The name of the property in Graph.</param>
        public GraphPropertyNameAttribute(string name)
        {
            Name = name;
        }

        // ----- METHODS -----
    }
}

