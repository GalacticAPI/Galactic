using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Galactic.Rest
{
    /// <summary>
    /// An HTTP based client for interacting with RESTful APIs.
    /// </summary>
    public class RestClient
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The default amount of time to wait in seconds if an HTTP 429 Too Many Requests response is received
        /// before querying again.
        /// </summary>
        public const int DEFAULT_STANDOFF_TIME_IN_SECS = 60;

        /// <summary>
        /// The default number of times that the client will retry a request when warranted.
        /// </summary>
        public const int DEFAULT_MAX_RETRIES = 10;

        // ----- VARIABLES -----

        /// <summary>
        /// An HttpClient instance for use when making API calls.
        /// </summary>
        private readonly HttpClient httpClient = new();

        /// <summary>
        /// The number of times that the client will retry a request when warranted.
        /// </summary>
        private int maxRetries = DEFAULT_MAX_RETRIES;

        /// <summary>
        /// The amount of time to wait in seconds if an HTTP 429 Too Many Requests response is received before
        /// querying again.
        /// </summary>
        private int standoffTimeInSecs = DEFAULT_STANDOFF_TIME_IN_SECS;

        /// <summary>
        /// Key = Guid, uniquely identifying request.
        /// Value = number of retries already taken.
        /// </summary>
        private Dictionary<Guid, int> requestRetries = new();

        // ----- PROPERTIES -----

        /// <summary>
        /// The base URI to use for requests to the API.
        /// </summary>
        public string BaseUri
        {
            get => httpClient.BaseAddress.ToString();
        }

        /// <summary>
        /// The number of times that the client will retry a request when warranted.
        /// </summary>
        public int MaxRetries
        {
            get => maxRetries;
            set
            {
                // Set the maximum number of retries.
                if (value >= 0)
                {
                    maxRetries = value;
                }
                else
                {
                    // Use the default value.
                    maxRetries = DEFAULT_MAX_RETRIES;
                }
            }
        }

        /// <summary>
        /// The amount of time to wait in seconds if an HTTP 429 Too Many Requests response is received
        /// before querying again.
        /// </summary>
        public int StandoffTimeInSecs
        {
            get => standoffTimeInSecs;
            set
            {
                // Set the standoff time.
                if (value >= 0)
                {
                    standoffTimeInSecs = value;
                }
                else
                {
                    // Use the default value.
                    standoffTimeInSecs = DEFAULT_STANDOFF_TIME_IN_SECS;
                }
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a client that can make requests to RESTful API.
        /// </summary>
        /// <param name="baseUri">The base URI to use for requests to the API.</param>
        /// <param name="authorizationHeaderScheme">(Optional)The scheme value to use for the HTTP Authorization header of requests. Note: Should be paired with authorizationHeaderCredentials.</param>
        /// <param name="authorizationHeaderCredentials">(Optional)The credential value to use for the HTTP Authorization header of requests. (Usually an API key.) Note: Should be paired with authorizationHeaderScheme.</param>
        /// <param name="standoffTimeInSecs">(Optional)The amount of time to wait in seconds if an HTTP 429 Too Many Requests response is received before querying again. Defaults to DEFAULT_STANDOFF_TIME_IN_SECS if not supplied or an invalid value is supplied.</param>
        /// <param name="maxRetries">(Optional)The number of times that the client will retry a request when warranted.. Defaults to DEFAULT_MAX_RETRIES if not supplied or an invalid value is supplied.</param>
        public RestClient(string baseUri, string authorizationHeaderScheme = "", string authorizationHeaderCredentials = "", int standoffTimeInSecs = DEFAULT_STANDOFF_TIME_IN_SECS, int maxRetries = DEFAULT_MAX_RETRIES)
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

                // Set the standoff time.
                StandoffTimeInSecs = standoffTimeInSecs;
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
            return Delete(path, Guid.Empty);
        }

        /// <summary>
        /// Sends a Delete request to a API endpoint at the supplied path.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        protected EmptyRestResponse Delete(string path, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the DELETE request.
                    Task<HttpResponseMessage> responseTask = httpClient.DeleteAsync(httpClient.BaseAddress + path);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Check whether we've submitted too many requests.
                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // Check how many retries we've already done.
                        int numRetries = 0;
                        if (requestId != Guid.Empty)
                        {
                            if (requestRetries.ContainsKey(requestId))
                            {
                                numRetries = requestRetries[requestId];
                            }
                        }
                        else
                        {
                            requestId = Guid.NewGuid();
                        }

                        // Retry if we haven't hit the limit.
                        if (numRetries < MaxRetries)
                        {
                            // Wait the standoff time.
                            Thread.Sleep(StandoffTimeInSecs * 1000);

                            // Increment the number of retries.
                            requestRetries[requestId] = ++numRetries;

                            // Retry.
                            return Delete(path, requestId);
                        }
                        else
                        {
                            // We've hit the limit.

                            // Remove any associated retries for this request.
                            if (requestRetries.ContainsKey(requestId))
                            {
                                requestRetries.Remove(requestId);
                            }

                            return null;
                        }
                    }

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

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
            return GetFromJson<T>(path, Guid.Empty);
        }

        /// <summary>
        /// Gets JSON data from an API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        protected JsonRestResponse<T> GetFromJson<T>(string path, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the GET request.
                    Task<HttpResponseMessage> responseTask = httpClient.GetAsync(httpClient.BaseAddress + path);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Check whether we've submitted too many requests.
                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // Check how many retries we've already done.
                        int numRetries = 0;
                        if (requestId != Guid.Empty)
                        {
                            if (requestRetries.ContainsKey(requestId))
                            {
                                numRetries = requestRetries[requestId];
                            }
                        }
                        else
                        {
                            requestId = Guid.NewGuid();
                        }

                        // Retry if we haven't hit the limit.
                        if (numRetries < MaxRetries)
                        {
                            // Wait the standoff time.
                            Thread.Sleep(StandoffTimeInSecs * 1000);

                            // Increment the number of retries.
                            requestRetries[requestId] = ++numRetries;

                            // Retry.
                            return GetFromJson<T>(path, requestId);
                        }
                        else
                        {
                            // We've hit the limit.

                            // Remove any associated retries for this request.
                            if (requestRetries.ContainsKey(requestId))
                            {
                                requestRetries.Remove(requestId);
                            }

                            return null;
                        }
                    }

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

                    // Return the result.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error and the endpoint couldn't be queried.

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

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
            return Post(path, Guid.Empty);
        }

        /// <summary>
        /// Posts to a API endpoint at the supplied path, where the body of the message isn't relevant.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        protected EmptyRestResponse Post(string path, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the POST request.
                    Task<HttpResponseMessage> responseTask = httpClient.PostAsync(httpClient.BaseAddress + path, null);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Check whether we've submitted too many requests.
                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // Check how many retries we've already done.
                        int numRetries = 0;
                        if (requestId != Guid.Empty)
                        {
                            if (requestRetries.ContainsKey(requestId))
                            {
                                numRetries = requestRetries[requestId];
                            }
                        }
                        else
                        {
                            requestId = Guid.NewGuid();
                        }

                        // Retry if we haven't hit the limit.
                        if (numRetries < MaxRetries)
                        {
                            // Wait the standoff time.
                            Thread.Sleep(StandoffTimeInSecs * 1000);

                            // Increment the number of retries.
                            requestRetries[requestId] = ++numRetries;

                            // Retry.
                            return Post(path, requestId);
                        }
                        else
                        {
                            // We've hit the limit.

                            // Remove any associated retries for this request.
                            if (requestRetries.ContainsKey(requestId))
                            {
                                requestRetries.Remove(requestId);
                            }

                            return null;
                        }
                    }

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

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
            return PostAsJson<T>(path, content, Guid.Empty);
        }

        /// <summary>
        /// Posts to a JSON API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="content">An object that can be serialized to JSON and used as the content of the requst.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request reponse, or null if the request could not be completed.</returns>
        protected JsonRestResponse<T> PostAsJson<T>(string path, object content, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path) && content != null)
            {
                // Send the POST request.
                Task<HttpResponseMessage> responseTask = httpClient.PostAsJsonAsync(httpClient.BaseAddress + path, content);

                // Wait for the response to complete.
                responseTask.Wait();

                // Check whether we've submitted too many requests.
                if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Check how many retries we've already done.
                    int numRetries = 0;
                    if (requestId != Guid.Empty)
                    {
                        if (requestRetries.ContainsKey(requestId))
                        {
                            numRetries = requestRetries[requestId];
                        }
                    }
                    else
                    {
                        requestId = Guid.NewGuid();
                    }

                    // Retry if we haven't hit the limit.
                    if (numRetries < MaxRetries)
                    {
                        // Wait the standoff time.
                        Thread.Sleep(StandoffTimeInSecs * 1000);

                        // Increment the number of retries.
                        requestRetries[requestId] = ++numRetries;

                        // Retry.
                        return PostAsJson<T>(path, content, requestId);
                    }
                    else
                    {
                        // We've hit the limit.

                        // Remove any associated retries for this request.
                        if (requestRetries.ContainsKey(requestId))
                        {
                            requestRetries.Remove(requestId);
                        }

                        return null;
                    }
                }

                // Remove any associated retries for this request.
                if (requestRetries.ContainsKey(requestId))
                {
                    requestRetries.Remove(requestId);
                }

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
            return Put(path, Guid.Empty);
        }

        /// <summary>
        /// Puts to a API endpoint at the supplied path, where the body of the message isn't relevant.
        /// </summary>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request response, or null if the request could not be completed.</returns>
        protected EmptyRestResponse Put(string path, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Send the POST request.
                    Task<HttpResponseMessage> responseTask = httpClient.PutAsync(httpClient.BaseAddress + path, null);

                    // Wait for the response to complete.
                    responseTask.Wait();

                    // Check whether we've submitted too many requests.
                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // Check how many retries we've already done.
                        int numRetries = 0;
                        if (requestId != Guid.Empty)
                        {
                            if (requestRetries.ContainsKey(requestId))
                            {
                                numRetries = requestRetries[requestId];
                            }
                        }
                        else
                        {
                            requestId = Guid.NewGuid();
                        }

                        // Retry if we haven't hit the limit.
                        if (numRetries < MaxRetries)
                        {
                            // Wait the standoff time.
                            Thread.Sleep(StandoffTimeInSecs * 1000);

                            // Increment the number of retries.
                            requestRetries[requestId] = ++numRetries;

                            // Retry.
                            return Put(path, requestId);
                        }
                        else
                        {
                            // We've hit the limit.

                            // Remove any associated retries for this request.
                            if (requestRetries.ContainsKey(requestId))
                            {
                                requestRetries.Remove(requestId);
                            }

                            return null;
                        }
                    }

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

                    // Return the response.
                    return new(responseTask.Result);
                }
                catch
                {
                    // There was an error sending the request.

                    // Remove any associated retries for this request.
                    if (requestRetries.ContainsKey(requestId))
                    {
                        requestRetries.Remove(requestId);
                    }

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
            return PutAsJson<T>(path, content, Guid.Empty);
        }

        /// <summary>
        /// Puts to a JSON API endpoint at the supplied path.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="path">The path to the endpoint from the baseUri.</param>
        /// <param name="content">An object that can be serialized to JSON and used as the content of the requst.</param>
        /// <param name="requestId">(Optional)A unique identifier for the request. Only necessary when making many related calls (for instance retries).</param>
        /// <returns>An object with the request reponse, or null if the request could not be completed.</returns>
        protected JsonRestResponse<T> PutAsJson<T>(string path, object content, Guid requestId)
        {
            if (!string.IsNullOrWhiteSpace(path) && content != null)
            {
                // Send the POST request.
                Task<HttpResponseMessage> responseTask = httpClient.PutAsJsonAsync(httpClient.BaseAddress + path, content);

                // Wait for the response to complete.
                responseTask.Wait();

                // Check whether we've submitted too many requests.
                if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Check how many retries we've already done.
                    int numRetries = 0;
                    if (requestId != Guid.Empty)
                    {
                        if (requestRetries.ContainsKey(requestId))
                        {
                            numRetries = requestRetries[requestId];
                        }
                    }
                    else
                    {
                        requestId = Guid.NewGuid();
                    }

                    // Retry if we haven't hit the limit.
                    if (numRetries < MaxRetries)
                    {
                        // Wait the standoff time.
                        Thread.Sleep(StandoffTimeInSecs * 1000);

                        // Increment the number of retries.
                        requestRetries[requestId] = ++numRetries;

                        // Retry.
                        return PutAsJson<T>(path, content, requestId);
                    }
                    else
                    {
                        // We've hit the limit.

                        // Remove any associated retries for this request.
                        if (requestRetries.ContainsKey(requestId))
                        {
                            requestRetries.Remove(requestId);
                        }

                        return null;
                    }
                }

                // Remove any associated retries for this request.
                if (requestRetries.ContainsKey(requestId))
                {
                    requestRetries.Remove(requestId);
                }

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
