using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/Intangible
    /// A utility class that serves as the umbrella for a number of 'intangible'
    /// things such as quantities, structured values, etc.
    /// </summary>
    [DataContract(Name = "Intangible", Namespace = "http://schema.org/Intangible")]
    public class Intangible : Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        [DataMember]
        public override string ItemType { get { return "Intangible"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        [DataMember]
        public override string ItemTypeDescription { get { return "A utility class that serves as the umbrella for a number of 'intangible' things such as quantities, structured values, etc."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        [DataMember]
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/Intangible"); } }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
