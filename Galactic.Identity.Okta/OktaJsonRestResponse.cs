using Galactic.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A class representing a response from a JSON REST API endpoint, and it's associated content.
    /// </summary>
    public class OktaJsonRestResponse<T> : JsonRestResponse<T>
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The Uri with the next page of results. Null if all results have been returned.
        /// </summary>
        public Uri NextPage
        {
            get
            {
                // Get the link headers from the message.
                IEnumerable<string> linkHeaders = responseMessage.Headers.GetValues("link");
                foreach(string linkHeader in linkHeaders)
                {
                    // Determing if the next link header is specified.
                    if (linkHeader.Contains("rel=\"next\""))
                    {
                        // Parse the link header to retrieve the URL of the next page.
                        return new Uri(linkHeader.Split(';')[0].Replace("<", "").Replace(">", ""));
                    }
                }
                // A next link header was not found.
                return null;
            }
        }

        /// <summary>
        /// The Uri of the current page of results. Null if paging is not enabled for the request.
        /// </summary>
        public Uri CurrentPage
        {
            get
            {
                // Get the link headers from the message.
                IEnumerable<string> linkHeaders = responseMessage.Headers.GetValues("link");
                foreach (string linkHeader in linkHeaders)
                {
                    // Determing if the self link header is specified.
                    if (linkHeader.Contains("rel=\"self\""))
                    {
                        // Parse the link header to retrieve the URL of the next page.
                        return new Uri(linkHeader.Split(';')[0].Replace("<", "").Replace(">", ""));
                    }
                }
                // A self link header was not found.
                return null;
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes the object with response data from the API endpoint.
        /// </summary>
        /// <param name="message">The HTTP response message obtained from the API endpoint.</param>
        /// <exception cref="ArgumentNullException">If a message is not supplied.</exception>
        public OktaJsonRestResponse(HttpResponseMessage message) : base(message)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Creates an OktaJsonRestResponse object from the supplied JsonRestResponse.
        /// </summary>
        /// <param name="response">The JsonRestResponse object to build the OktaJsonRestResponse from.</param>
        /// <returns>The newly created object.</returns>
        static public OktaJsonRestResponse<T> FromJsonRestResponse(JsonRestResponse<T> response)
        {
            if (response != null)
            {
                return new(response.Message);
            }
            else
            {
                throw new ArgumentNullException(nameof(response));
            }
        }
    }
}
