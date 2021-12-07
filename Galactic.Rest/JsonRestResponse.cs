using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Rest
{
    /// <summary>
    /// A class representing a response from a JSON REST API endpoint, and it's associated content.
    /// </summary>
    public class JsonRestResponse<T> : RestResponse<T>
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The JSON parsed value returned by the reponse.
        /// </summary>
        public override T Value
        {
            get
            {
                // Read the content from the response.
                Task<T> jsonReadTask = responseMessage.Content.ReadFromJsonAsync<T>();

                // Wait for the read to complete.
                jsonReadTask.Wait();

                // Create a new group object from the JSON data.
                return jsonReadTask.Result;
            }
        }


        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes the object with response data from the API endpoint.
        /// </summary>
        /// <param name="message">The HTTP response message obtained from the API endpoint.</param>
        /// <exception cref="ArgumentNullException">If a message is not supplied.</exception>
        public JsonRestResponse(HttpResponseMessage message) : base(message)
        {
        }

        // ----- METHODS -----
    }
}
