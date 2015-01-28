using Galactic.EventLog;
using Galactic.FileSystem;
using System;
using System.Collections.Generic;

namespace Galactic.Sql
{
    /// <summary>
    /// Provides various utility methods for use with SQL databases.
    /// </summary>
    public abstract class SqlUtility
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The maximum length of a NCHAR field in the database.
        /// </summary>
        public abstract int MAX_NCHAR_LENGTH
        {
            get;
        }

        /// <summary>
        /// The maximum length of a NVARCHAR field in the database.
        /// </summary>
        public abstract int MAX_NVARCHAR_LENGTH
        {
            get;
        }

        /// <summary>
        /// The maximum length of a VARCHAR field in the database.
        /// </summary>
        public abstract int MAX_VARCHAR_LENGTH
        {
            get;
        }

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

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
                log.Log(new Event(typeof(SqlUtility).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
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

        /// <summary>
        /// Runs SQL script files given their path.
        /// </summary>
        /// <param name="connectionString">The connection string to use when creating the database.</param>
        /// <param name="scriptPaths">The paths to the script files on the filesystem.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if all scripts ran, false otherwise.</returns>
        public virtual bool RunScript(string connectionString, string[] scriptPaths, EventLog.EventLog log)
        {
            // Check that the connection string and script paths were provided.
            if (!string.IsNullOrWhiteSpace(connectionString) && scriptPaths != null)
            {
                // The connection string and script paths were provided.

                // Check that the files exist.
                foreach (string path in scriptPaths)
                {
                    if (!File.Exists(path))
                    {
                        // A file does not exist.
                        return false;
                    }
                }

                // Execute the scripts.
                foreach (string path in scriptPaths)
                {
                    if (!ExecuteNonQuerySQLFile(path, connectionString, log))
                    {
                        // There was an error running the script.
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // The connection string and script paths were not provided.
                return false;
            }
        }

        /// <summary>
        /// Executes the non-query SQL command provided.
        /// </summary>
        /// <param name="command">The SQL command text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if the command ran, false otherwise.</returns>
        abstract public bool ExecuteNonQuery(String command, String connectionString, EventLog.EventLog log);

        /// <summary>
        /// Executes the SQL statements in a file given its path, and a connection string to the database.
        /// </summary>
        /// <param name="scriptFilePath">The path the the SQL script file to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if the script ran, false otherwise.</returns>
        public virtual bool ExecuteNonQuerySQLFile(String scriptFilePath, String connectionString, EventLog.EventLog log)
        {
            string script = File.ReadAllAsText(scriptFilePath);

            // Check that the script file was read.
            if (!string.IsNullOrWhiteSpace(script))
            {
                return ExecuteNonQuery(script, connectionString, log);
            }
            else
            {
                // The script file was not read.
                return false;
            }
        }

        /// <summary>
        /// Executes the SQL query provided.
        /// </summary>
        /// <param name="query">The query text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>A list of rows of data returned from the query.</returns>
        abstract public List<SqlRow> ExecuteQuery(String query, String connectionString, EventLog.EventLog log);

        /// <summary>
        /// Executes the SQL query in a file given its path, and a connection string to the database.
        /// </summary>
        /// <param name="scriptFilePath">The path the the TSQL script file to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>A list of rows of data returned from the query.</returns>
        public virtual List<SqlRow> ExecuteQuerySQLFile(String scriptFilePath, String connectionString, EventLog.EventLog log)
        {
            string script = File.ReadAllAsText(scriptFilePath);

            // Check that the script file was read.
            if (!string.IsNullOrWhiteSpace(script))
            {
                return ExecuteQuery(script, connectionString, log);
            }
            else
            {
                // The script file was not read.
                return new List<SqlRow>();
            }
        }
    }
}
