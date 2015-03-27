using System;
using System.Collections.Generic;
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
        [DataMember]
        public string ContentType;

        /// <summary>
        /// The supported encoding type(s) for an EntryPoint request.
        /// </summary>
        [DataMember]
        public string EncodingType;

        // Backing variable for HttpMethod.
        [DataMember]
        private string httpMethod;

        /// <summary>
        /// An url template (RFC6570) that will be used to construct the target of the execution of the action.
        /// </summary>
        [DataMember]
        public string UrlTemplate;

        // ----- PROPERTIES -----

        /// <summary>
        /// An HTTP method that specifies the appropriate HTTP method for a request to
        /// an HTTP EntryPoint. Values are capitalized strings as used in HTTP.
        /// </summary>
        [DataMember]
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
        [DataMember]
        public override string ItemType { get { return "EntryPoint"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        [DataMember]
        public override string ItemTypeDescription { get { return "An entry point, within some Web-based protocol."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        [DataMember]
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/EntryPoint"); } }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
