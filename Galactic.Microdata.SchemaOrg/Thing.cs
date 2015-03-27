using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization;


namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/Thing
    /// The most generic type of item.
    /// </summary>
    [DataContract(Name = "Thing", Namespace = "http://schema.org/Thing")]
    public class Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// An additional type for the item, typically used for adding more specific
        /// types from external vocabularies in microdata syntax. This is a relationship
        /// between something and a class that the thing is in. In RDFa syntax, it is
        /// better to use the native RDFa syntax - the 'typeof' attribute - for multiple
        /// types. Schema.org tools may have only weaker understanding of extra types,
        /// in particular those defined externally
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "additionalType")]
        public Uri AdditionalType;

        /// <summary>
        /// An alias for the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "alternateName")]
        public string AlternateName;

        /// <summary>
        /// A short description of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "description")]
        public string Description;

        /// <summary>
        /// The name of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name= "name")]
        public string Name;

        /// <summary>
        /// Indicates a potential Action, which describes an idealized action in which
        /// this thing would play an 'object' role.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "potentialAction")]
        public Action PotentialAction;

        /// <summary>
        /// URL of a reference Web page that unambiguously indicates the item's identity.
        /// E.g. the URL of the item's Wikipedia page, Freebase page, or official website.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "sameAs")]
        public Uri SameAs;

        /// <summary>
        /// URL of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "url")]
        public Uri Url;

        // ----- PROPERTIES -----

        /// <summary>
        /// An image of the item.
        /// Expected Types: URI or ImageObject.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "image")]
        public dynamic Image
        {
            get; 
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public virtual string ItemType { get { return "Thing"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public virtual string ItemTypeDescription { get { return "The most generic type of item."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public virtual Uri ItemTypeUrl { get { return new Uri("http://schema.org/Thing"); } }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
