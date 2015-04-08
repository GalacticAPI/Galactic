using Galactic.Configuration;
using Galactic.EventLog;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Galactic.NoSql.MongoDB
{
    /// <summary>
    /// Provides various utility methods for use with MongoDB servers.
    /// </summary>
    public class MongoDBUtility : NoSqlUtility
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The name of the default collection used for adding documents if a collection
        /// is not specified.
        /// </summary>
        public const string DEFAULT_COLLECTION = "default";

        // The name of the object property used to identify the document's Id.
        private const string ID_FIELD_NAME = "_id";

        // ---------- VARIABLES ----------

        // The client object that can perform operations against a MongoDB server.
        private readonly MongoClient client = null;

        // The database that the client is connected to.
        private readonly MongoDatabase database = null;

        // The server that the client is connected to.
        private MongoServer server = null;

        // The url object that contains configuration information about the MongoDB connection.
        private MongoUrl url = null;

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a utility object that can be used to perform operations against a MongoDB server.
        /// Note:
        /// Uses the authentication and database information from the supplied configuration item.
        /// </summary>
        /// <param name="configurationFolderPath">The path to the folder containing the encrypted configuration file containing information required to establish the connection to the server.</param>
        /// <param name="configurationItemName">The name of configuration item containing the information required to connect to the server. (Typically it's filename without the extension.)</param>
        public MongoDBUtility(string configurationFolderPath, string configurationItemName)
        {
            if (!string.IsNullOrWhiteSpace(configurationFolderPath) && !string.IsNullOrWhiteSpace(configurationItemName))
            {
                ConfigurationItem configItem = new ConfigurationItem(configurationFolderPath, configurationItemName, true);
                
                try
                {
                    // Read the connection string from the configuration file.
                    url = MongoUrl.Create(configItem.Value);
                    client = new MongoClient(url);
                    server = client.GetServer();
                    database = server.GetDatabase(url.DatabaseName);
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
        /// Adds or replaces a document within the default collection.
        /// </summary>
        /// <param name="id">The id of the document to add or replace. If null the document will be added with a newly generated id.</param>
        /// <param name="document">The document to add or replace. Note: These must be able to be serialized as a BsonDocument.</param>
        /// <returns>The id of the document if it was added or replaced, null otherwise.</returns>
        public override string AddOrReplace(string id, ExpandoObject document)
        {
            return AddOrReplace(id, DEFAULT_COLLECTION, document);
        }

        /// <summary>
        /// Adds or replaces a document within the specified collection.
        /// </summary>
        /// <param name="id">The id of the document to add or replace. If null the document will be added with a newly generated id.</param>
        /// <param name="collection">The name of the collection to add or replace the document within.</param>
        /// <param name="document">The document to add or replace. Note: These must be able to be serialized as a BsonDocument.</param>
        /// <returns>The id of the document if it was added or replaced, null otherwise.</returns>
        public string AddOrReplace(string id, string collection, ExpandoObject document)
        {
            if (!string.IsNullOrWhiteSpace(collection) && isCollectionNameValid(collection) && document != null)
            {
                try
                {
                    MongoCollection mongoCollection = database.GetCollection<BsonDocument>(collection);

                    // Create a BsonDocument to save from the ExpandoObject.
                    BsonDocument bsonDocument = document.ToBsonDocument();

                    // Check that the supplied document contains a ID_FIELD_NAME property.
                    if (!bsonDocument.Names.Contains(ID_FIELD_NAME))
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            // Add an ID_FIELD_NAME property to the document with a newly generated Id.
                            bsonDocument.Add(ID_FIELD_NAME, ObjectId.GenerateNewId());
                        }
                        else
                        {
                            // Add an ID_FIELD_NAME property to the document with the supplied id.
                            bsonDocument.Add(ID_FIELD_NAME, new ObjectId(id));
                        }
                    }

                    // Add or replace the document.
                    WriteConcernResult result = mongoCollection.Save(bsonDocument);
                    if (result.Ok)
                    {
                        // The document was added or replaced.
                        return ((ObjectId)bsonDocument[ID_FIELD_NAME]).ToString();
                    }
                    else
                    {
                        // There was an error adding or replacing the document.
                        return null;
                    }
                }
                catch (Exception)
                {
                    // There was an error adding or replacing the document in the collection.
                    return null;
                }
            }
            else
            {
                // A null id or collection was supplied or the collection name was invalid.
                return null;
            }
        }

        /// <summary>
        /// Deletes the specified id and it's associated document from the default colleciton.
        /// </summary>
        /// <param name="id">The name of the id to delete.</param>
        /// <returns>True if the id/document was deleted, false if there was an error or it could not otherwise be deleted.</returns>
        public override bool Delete(string id)
        {
            return Delete(id, DEFAULT_COLLECTION);
        }

        /// <summary>
        /// Deletes the specified id and it's associated document from the specified collection.
        /// </summary>
        /// <param name="id">The name of the id to delete.</param>
        /// <param name="collection">The name of the collection to delete the document in.</param>
        /// <returns>True if the id/document was deleted, false if there was an error or it could not otherwise be deleted.</returns>
        public bool Delete(string id, string collection)
        {
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(collection) && isCollectionNameValid(collection))
            {
                try
                {
                    MongoCollection mongoCollection = database.GetCollection<BsonDocument>(collection);
                    // Create a query to identify the document by id.
                    QueryDocument query = new QueryDocument("_id", new ObjectId(id));
                    // Delete the document in the collection.
                    WriteConcernResult result = mongoCollection.Remove(query);
                    if (result.Ok)
                    {
                        // The document was deleted.
                        return true;
                    }
                    else
                    {
                        // There was an error deleting the document.
                        return false;
                    }
                }
                catch (Exception)
                {
                    // There was an error deleting the document.
                    return false;
                }
                
            }
            else
            {
                // A null or empty id or collection was supplied or the collection name was invalid.
                return false;
            }
        }

        /// <summary>
        /// Gets a document from the database with the specified id within the default collection.
        /// </summary>
        /// <param name="id">The id of the document to retrieve.</param>
        /// <returns>The document with the specified id, or null if there was an error, or the id does not exist.</returns>
        public override dynamic Get(string id)
        {
            return Get(id, DEFAULT_COLLECTION);
        }

        /// <summary>
        /// Gets a document from the database with the specified id within the specified collection.
        /// </summary>
        /// <param name="id">The id of the document to retrieve.</param>
        /// <param name="collection">The name of the collection to get the document from.</param>
        /// <returns>The document with the specified id, or null if there was an error, or the id does not exist.</returns>
        public dynamic Get(string id, string collection)
        {
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(collection) && isCollectionNameValid(collection))
            {
                try
                {
                    MongoCollection mongoCollection = database.GetCollection<BsonDocument>(collection);
                    
                    // Get the document associated with the id in the collection.
                    BsonDocument document = mongoCollection.FindOneByIdAs<BsonDocument>(new ObjectId(id));

                    // Convert the document into an ExpandoObject with like properties to return.
                    ExpandoObject returnObject = new ExpandoObject();
                    foreach (string key in document.ToDictionary().Keys)
                    {
                        ((IDictionary<string, object>) returnObject)[key] = BsonTypeMapper.MapToDotNetValue(document[key]);
                    }

                    // Return the object.
                    return returnObject;
                }
                catch (Exception)
                {
                    // There was an error geting the document.
                    return null;
                }
            }
            else
            {
                // A null or empty id or collection was supplied or the collection name was invalid.
                return null;
            }
        }

        /// <summary>
        /// Gets documents from the database that correspond to the supplied query.
        /// </summary>
        /// <param name="query">The query to use when finding documents.</param>
        /// <returns>A list of documents that correspond to the query supplied, or an empty list if there was an error, or the query did not produce any results.</returns>
        public override List<dynamic> GetByQuery(object query)
        {
            return GetByQuery(query, DEFAULT_COLLECTION);
        }

        /// <summary>
        /// Gets documents from the database that correspond to the supplied query.
        /// </summary>
        /// <param name="query">The query to use when finding documents. (This implementation expects IMongoQuery objects.)</param>
        /// <param name="collection">The name of the collection to get the document from.</param>
        /// <returns>A list of documents that correspond to the query supplied, or an empty list if there was an error, or the query did not produce any results.</returns>
        public List<dynamic> GetByQuery(object query, string collection)
        {
            if (query != null && query is IMongoQuery && !string.IsNullOrWhiteSpace(collection) && isCollectionNameValid(collection))
            {
                try
                {
                    MongoCollection mongoCollection = database.GetCollection<BsonDocument>(collection);

                    // Get the documents associated with the query in the collection.
                    MongoCursor<BsonDocument> documents = mongoCollection.FindAs<BsonDocument>((IMongoQuery)query);

                    // Convert each document found into an ExpandoObject with like properties to return.
                    List<dynamic> list = new List<dynamic>();
                    foreach (BsonDocument document in documents)
                    {
                        ExpandoObject expando = new ExpandoObject();
                        foreach (string key in document.ToDictionary().Keys)
                        {
                            ((IDictionary<string, object>)expando)[key] = BsonTypeMapper.MapToDotNetValue(document[key]);
                        }
                        list.Add(expando);
                    }

                    // Return the list.
                    return list;
                }
                catch (Exception)
                {
                    // There was an error geting the document.
                    return new List<dynamic>();
                }
            }
            else
            {
                // A null or invalid query or collection was supplied.
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Checks the syntax of a supplied collection name to see if it contains invalid characters.
        /// </summary>
        /// <param name="name">The collection name to check.</param>
        /// <returns>True if the collection name is valid, false otherwise.</returns>
        public bool isCollectionNameValid(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Contains('\0'))
                {
                    // Names containing the null character are not valid.
                    return false;
                }
                if (name.StartsWith("system."))
                {
                    // Names beginnning with system. are reserved for internal use and are not valid.
                    return false;
                }
                if (name.Contains('$'))
                {
                    // $ is a reserved character and is not valid for collection names.
                    return false;
                }
                // The name is valid.
                return true;
            }
            // An empty name is not valid.
            return false;
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
                log.Log(new Event(typeof(MongoDBUtility).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
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
