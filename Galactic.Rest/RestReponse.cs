using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Rest
{
    /// <summary>
    /// An abstract class representing a response from a REST API endpoint, and it's associated content.
    /// Provides base functionality for all RestResponse classes.
    /// </summary>
    public abstract class RestResponse<T>
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The HTTPResponseMessage that contains the response from the endpoint.
        /// </summary>
        protected readonly HttpResponseMessage responseMessage = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// The value returned by the reponse.
        /// </summary>
        public abstract T Value
        {
            get;
        }

        /// <summary>
        /// The message returned from the endpoint. Includes, headers, status code, etc.
        /// </summary>
        public HttpResponseMessage Message
        {
            get
            {
                return responseMessage;
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes the object with response data from the API endpoint.
        /// </summary>
        /// <param name="message">The HTTP response message obtained from the API endpoint.</param>
        /// <exception cref="ArgumentNullException">If a message is not supplied.</exception>
        public RestResponse (HttpResponseMessage message)
        {
            if (message != null)
            {
                responseMessage = message;
            }
            else
            {
                throw new ArgumentNullException(nameof(message));
            }
        }

        // ----- METHODS -----
    }
}
