using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Galactic.REST
{
    /// <summary>
    /// An HTTP based client for interacting with RESTful APIs.
    /// </summary>
    public class RESTClient
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// An HttpClient instance for use when making API calls.
        /// </summary>
        private readonly HttpClient httpClient = new();

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a client that can make requests to RESTful API.
        /// </summary>
        /// <param name="baseUri">The base URI to use for requests to the API.</param>
        /// <param name="authorizationHeader">(Optional)The value to use for the HTTP Authorization header of requests.</param>
        public RESTClient(string baseUri, string authorizationHeader = "")
        {
            if (!string.IsNullOrWhiteSpace(baseUri))
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(baseUri);

                // Setup the authorization header.
                if (!string.IsNullOrWhiteSpace(authorizationHeader))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorizationHeader);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(baseUri));
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Gets JSON data from an API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <returns>An object with the result of the request, or default(T) if the request could not be completed.</returns>
        public T GetFromJson<T>(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the GET request.
                    Task<T> responseTask = httpClient.GetFromJsonAsync<T>(path);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Return the result.
                    return responseTask.Result;
                }
                catch
                {
                    // There was an error and the endpoint couldn't be queried.
                    return default;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Posts to a JSON API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="content">An object that can be serialized to JSON and used as the content of the requst.</param>
        /// <returns>An object with the result of the request, or default(T) if the request could not be completed.</returns>
        public T PostAsJson<T>(string path, object content)
        {
            if (!string.IsNullOrWhiteSpace(path) && content != null)
            {
                // Send the POST request.
                Task<HttpResponseMessage> responseTask = httpClient.PostAsJsonAsync(path, content);

                // Wait for the response to complete.
                responseTask.Wait();

                // Check whether the response was successful.
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    // Read the content from the response.
                    Task<T> jsonReadTask = responseTask.Result.Content.ReadFromJsonAsync<T>();

                    // Wait for the read to complete.
                    jsonReadTask.Wait();

                    // Create a new group object from the JSON data.
                    return jsonReadTask.Result;
                }
                else
                {
                    // The resquest wasn't successful.
                    return default;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentNullException(nameof(path));
                }
                else
                {
                    throw new ArgumentNullException(nameof(content));
                }    
            }
        }
    }
}
