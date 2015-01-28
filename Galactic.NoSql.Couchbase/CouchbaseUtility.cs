using Galactic.Configuration;
using Galactic.EventLog;
using Couchbase;
using Couchbase.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.IO;

namespace Galactic.NoSql.Couchbase
{
    /// <summary>
    /// Provides various utility methods for use with Couchbase servers.
    /// </summary>
    public class CouchbaseUtility : NoSqlUtility, IDisposable
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The client object that can perform operations against a Couchbase server.
        private readonly CouchbaseClient client = null;

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a utility object that can be used to perform operations against a Couchbase server.
        /// </summary>
        /// <param name="config">A Couchbase configuration object initialized with information on how to connect to the server.</param>
        public CouchbaseUtility (CouchbaseClientConfiguration config)
        {
            if (config != null)
            {
                // Create a connection with the Couchbase bucket.
                client = new CouchbaseClient(config);
            }
            else
            {
                throw new ArgumentNullException("config", "A Couchbase Configuration object must be supplied.");
            }
        }

        /// <summary>
        /// Creates a utility object that can be used to perform operations against a Couchbase server.
        /// </summary>
        /// <param name="configurationFolderPath">The path to the folder containing the encrypted configuration file containing information required to establish the connection to the server.</param>
        /// <param name="configurationItemName">The name of configuration item containing the information required to connect to the server. (Typically it's filename without the extension.)</param>
        public CouchbaseUtility(string configurationFolderPath, string configurationItemName)
        {
            if (!string.IsNullOrWhiteSpace(configurationFolderPath) && !string.IsNullOrWhiteSpace(configurationItemName))
            {
                ConfigurationItem configItem = new ConfigurationItem(configurationFolderPath, configurationItemName, true);
                
                try
                {
                    // Read the values required from the configuration file.
                    StringReader reader = new StringReader(configItem.Value);
                    string urlsLine = reader.ReadLine();
                    string[] urls = new string[] { };
                    if (!string.IsNullOrWhiteSpace(urlsLine))
                    {
                        urls = urlsLine.Split(',');
                    }
                    string bucket = reader.ReadLine();
                    string bucketPassword = reader.ReadLine();

                    if (urls.Length > 0 && !string.IsNullOrWhiteSpace(bucket) && !string.IsNullOrWhiteSpace(bucketPassword))
                    {
                        // Configure the client.
                        CouchbaseClientConfiguration config = new CouchbaseClientConfiguration();
                        foreach (string url in urls)
                        {
                            config.Urls.Add(new Uri(url));
                        }
                        config.Bucket = bucket;
                        config.BucketPassword = bucketPassword;

                        // Create a connection with the Couchbase bucket.
                        client = new CouchbaseClient(config);
                    }
                    else
                    {
                        throw new FormatException("Could not load configuration data from file. File is not of the correct format.");
                    }
                }
                catch
                {
                    throw new FormatException("Could not load configuration data from file. File is not of the correct format.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(configurationFolderPath))
                {
                    throw new ArgumentNullException("configurationFolderPath", "A path to a configuration items folder must be supplied.");
                }
                else
                {
                    throw new ArgumentNullException("configurationItemName", "The name of the configuration item to load must be supplied.");
                }
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Adds or replaces a document in the store with the specified id.
        /// </summary>
        /// <param name="id">The id of the document to add or replace.</param>
        /// <param name="document">The document to add or replace.</param>
        /// <returns>True if the document was added or replaced, false otherwise.</returns>
        public override bool AddOrReplace(string id, object document)
        {
            if (!string.IsNullOrWhiteSpace(id) && document != null)
            {
                var result = client.ExecuteStore(StoreMode.Set, id, document);
                return result.Success;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified id and it's associated document from the database.
        /// </summary>
        /// <param name="id">The name of the id to delete.</param>
        /// <returns>True if the id/document was deleted, false if there was an error or it could not otherwise be deleted.</returns>
        public override bool Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var result = client.ExecuteRemove(id);
                return result.Success;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Releases all resources associated with the client's connection to the Couchbase server.
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Gets a document from the database with the specified id.
        /// </summary>
        /// <param name="id">The id of the document to retrieve.</param>
        /// <returns>The document with the specified id, or null if there was an error, or the id does not exist.</returns>
        public override object Get(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                //return client.Get(id);
                var result = client.ExecuteGet(id);
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="log">The event log to log the execption to.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        protected override bool LogException(Exception e, EventLog.EventLog log)
        {
            if (log != null)
            {
                log.Log(new Event(typeof(CouchbaseUtility).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
                    "Description:\n" +
                   e.Message + "\n" +
                   "Stack Trace:\n" +
                   e.StackTrace));
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
