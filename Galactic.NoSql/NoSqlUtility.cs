using Galactic.EventLog;
using System;

namespace Galactic.NoSql
{
    /// <summary>
    /// Provides various utility methods for use with NoSQL document databases.
    /// </summary>
    public abstract class NoSqlUtility
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

        /// <summary>
        /// Adds or replaces a document in the store with the specified id.
        /// </summary>
        /// <param name="id">The id of the document to add or replace.</param>
        /// <param name="document">The document to add or replace.</param>
        /// <returns>True if the document was added or replaced, false otherwise.</returns>
        public abstract bool AddOrReplace(string id, object document);

        /// <summary>
        /// Deletes the specified id and it's associated document from the database.
        /// </summary>
        /// <param name="id">The name of the id to delete.</param>
        /// <returns>True if the id/document was deleted, false if there was an error or it could not otherwise be deleted.</returns>
        public abstract bool Delete(string id);

        /// <summary>
        /// Gets a document from the database with the specified id.
        /// </summary>
        /// <param name="id">The id of the document to retrieve.</param>
        /// <returns>The document with the specified id, or null if there was an error, or the id does not exist.</returns>
        public abstract object Get(string id);

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="log">The event log to log the execption to.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        protected virtual bool LogException(Exception e, EventLog.EventLog log)
        {
            if (log != null)
            {
                log.Log(new Event(typeof(NoSqlUtility).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
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
