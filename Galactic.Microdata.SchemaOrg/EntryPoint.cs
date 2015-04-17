using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/EntryPoint
    /// An entry point, within some Web-based protocol.
    /// </summary>
    [DataContract(Name = "EntryPoint", Namespace = "http://schema.org/EntryPoint")]
    public class EntryPoint : Intangible
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /*
        /// <summary>
        /// An application that can complete the request.
        /// </summary>
        public SoftwareApplication Application;
        */

        /// <summary>
        /// The supported content type(s) for an EntryPoint response.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "contentType")]
        public string ContentType;

        /// <summary>
        /// The supported encoding type(s) for an EntryPoint request.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "encodingType")]
        public string EncodingType;

        // Backing variable for HttpMethod.
        [DataMember (EmitDefaultValue = false, Name = "httpMethod")]
        private string httpMethod;

        /// <summary>
        /// An url template (RFC6570) that will be used to construct the target of the execution of the action.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "urlTemplate")]
        public string UrlTemplate;

        // ----- PROPERTIES -----

        /// <summary>
        /// An HTTP method that specifies the appropriate HTTP method for a request to
        /// an HTTP EntryPoint. Values are capitalized strings as used in HTTP.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "httpMethod")]
        public string HttpMethod
        {
            get
            {
                return httpMethod;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string method = value.ToUpper();
                    switch (method)
                    {
                        case "CONNECT":
                        case "DELETE":
                        case "GET":
                        case "HEAD":
                        case "OPTIONS":
                        case "POST":
                        case "PUT":
                        case "TRACE":
                            httpMethod = method;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "EntryPoint"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "An entry point, within some Web-based protocol."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/EntryPoint"); } }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntryPoint()
        {

        }

        /// <summary>
        /// Construct a EntryPoint from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the EntryPoint.</param>
        public EntryPoint (ExpandoObject expando) : base(expando)
        {
        }
    }
}
