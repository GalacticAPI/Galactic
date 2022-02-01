using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Galactic.Rest
{
    /// <summary>
    /// An HTTP based client for interacting with RESTful APIs.
    /// </summary>
    public class RestClient
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// An HttpClient instance for use when making API calls.
        /// </summary>
        private readonly HttpClient httpClient = new();

        // ----- PROPERTIES -----

        /// <summary>
        /// The base URI to use for requests to the API.
        /// </summary>
        public string BaseUri
        {
            get => httpClient.BaseAddress.ToString();
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a client that can make requests to RESTful API.
        /// </summary>
        /// <param name="baseUri">The base URI to use for requests to the API.</param>
        /// <param name="authorizationHeaderScheme">(Optional)The scheme value to use for the HTTP Authorization header of requests. Note: Should be paired with authorizationHeaderCredentials.</param>
        /// <param name="authorizationHeaderCredentials">(Optional)The credential value to use for the HTTP Authorization header of requests. (Usually an API key.) Note: Should be paired with authorizationHeaderScheme.</param>
        public RestClient(string baseUri, string authorizationHeaderScheme = "", string authorizationHeaderCredentials = "")
        {
            if (!string.IsNullOrWhiteSpace(baseUri))
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(baseUri);

                // Setup the authorization header.
                if (!string.IsNullOrWhiteSpace(authorizationHeaderScheme) && !string.IsNullOrWhiteSpace(authorizationHeaderCredentials))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorizationHeaderScheme, authorizationHeaderCredentials);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(baseUri));
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Sends a Delete request to a API endpoint at the supplied path.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        public EmptyRestResponse Delete(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the DELETE request.
                    Task<HttpResponseMessage> responseTask = httpClient.DeleteAsync(httpClient.BaseAddress + path);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Gets JSON data from an API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        public JsonRestResponse<T> GetFromJson<T>(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the GET request.
                    Task<HttpResponseMessage> responseTask = httpClient.GetAsync(httpClient.BaseAddress + path);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Return the result.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error and the endpoint couldn't be queried.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Posts to a API endpoint at the supplied path, where the body of the message isn't relevant.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        public EmptyRestResponse Post(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the POST request.
                    Task<HttpResponseMessage> responseTask = httpClient.PostAsync(httpClient.BaseAddress + path, null);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.
                    return null;
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
        /// <returns>An object with the request reponse, or null if the request could not be completed.</returns>
        public JsonRestResponse<T> PostAsJson<T>(string path, object content)
        {
            if (!string.IsNullOrWhiteSpace(path) && content != null)
            {
                // Send the POST request.
                Task<HttpResponseMessage> responseTask = httpClient.PostAsJsonAsync(httpClient.BaseAddress + path, content);

                // Wait for the response to complete.
                responseTask.Wait();

                // Check whether the response was successful.
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    // Return the response object.
                    return new(responseTask.Result);
                }
                else
                {
                    // The resquest wasn't successful.
                    return null;
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

        /// <summary>
        /// Puts to a API endpoint at the supplied path, where the body of the message isn't relevant.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        public EmptyRestResponse Put(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the POST request.
                    Task<HttpResponseMessage> responseTask = httpClient.PutAsync(httpClient.BaseAddress + path, null);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.
                    return null;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Puts to a JSON API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="content">An object that can be serialized to JSON and used as the content of the requst.</param>
        /// <returns>An object with the request reponse, or null if the request could not be completed.</returns>
        public JsonRestResponse<T> PutAsJson<T>(string path, object content)
        {
            if (!string.IsNullOrWhiteSpace(path) && content != null)
            {
                // Send the POST request.
                Task<HttpResponseMessage> responseTask = httpClient.PutAsJsonAsync(httpClient.BaseAddress + path, content);

                // Wait for the response to complete.
                responseTask.Wait();

                // Check whether the response was successful.
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    // Return the response object.
                    return new(responseTask.Result);
                }
                else
                {
                    // The resquest wasn't successful.
                    return null;
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
