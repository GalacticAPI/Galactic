using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

// Microsoft SQL Server Management Objects (SMO) SDK
// Microsoft.SqlServer.Smo.dll
// Microsoft.SqlServer.ConnectionInfo.dll
// Microsoft.SqlServer.Management.Sdk.Sfc.dll

namespace Galactic.Sql.MSSql
{
    /// <summary>
    /// Provides various utility methods for use with Microsoft SQL databases.
    /// </summary>
    public class MSSqlUtility : SqlUtility
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The maximum length of a NCHAR field in the database.
        /// </summary>
        public override int MAX_NCHAR_LENGTH
        {
            get
            {
                return 4000;
            }
        }

        /// <summary>
        /// The maximum length of a NVARCHAR field in the database.
        /// </summary>
        public override int MAX_NVARCHAR_LENGTH
        {
            get
            {
                return 4000;
            }
        }

        /// <summary>
        /// The maximum length of a VARCHAR field in the database.
        /// </summary>
        public override int MAX_VARCHAR_LENGTH
        {
            get
            {
                return 8000;
            }
        }

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

        /// <summary>
        /// Builds a connection string.
        /// </summary>
        /// <param name="serverName">The name of the database server.</param>
        /// <param name="instanceName">The name of the instance on the database server, if needed.</param>
        /// <param name="databaseName">The name of the database on the server.</param>
        /// <param name="accountName">The account for the database server.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="trustedConnection">Whether to use integrated security to create a trusted connection with the
        /// database server. An accountName and password are not required for a trusted connection, the Windows account
        /// running the application will be used.</param>
        /// <param name="management">Whether this connection string will be used for creating or deleting the database.</param>
        /// <returns>The built connection string, or null if the string could not be built.</returns>
        static public string BuildConnectionString(string serverName, string instanceName, string databaseName,
            string accountName, string password, bool trustedConnection, bool management)
        {
            // Check that a server name was supplied.
            if (!string.IsNullOrWhiteSpace(serverName))
            {
                // A server name was supplied.

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    // Create the connection string.
                    DataSource = serverName + instanceName
                };

                // Only add the initial catalog value if this is not a management connection string.
                if (!management)
                {
                    // Check that a database name was supplied.
                    if (!string.IsNullOrWhiteSpace(databaseName))
                    {
                        // A database name was supplied.
                        builder.InitialCatalog = databaseName;
                    }
                    else
                    {
                        // A database name was not supplied.
                        return null;
                    }
                }

                // Check whether a trusted connection should be created.
                if (trustedConnection)
                {
                    // Use the application's Windows account and integrated security to create a trusted connection.
                    builder.IntegratedSecurity = true;
                }
                else
                {
                    // A trusted connection is not required. Use standard security.
                    // Check that an account name and password were supplied.
                    if (!string.IsNullOrWhiteSpace(accountName) && !string.IsNullOrWhiteSpace(password))
                    {
                        // An account name and password were supplied.
                        builder.UserID = accountName;
                        builder.Password = password;
                    }
                    else
                    {
                        // An account name or password was not supplied.
                        // Throw the appropriate exception.
                        return null;
                    }
                }

                return builder.ToString();
            }
            else
            {
                // A server name was not supplied.
                return null;
            }
        }

        /// <summary>
        /// Executes the non-query TSQL command provided.
        /// </summary>
        /// <param name="command">The TSQL command text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if the command ran, false otherwise.</returns>
        override public bool ExecuteNonQuery(String command, String connectionString, EventLog.EventLog log)
        {
            // Check that the command and connection string are supplied.
            if (!string.IsNullOrWhiteSpace(command) && !string.IsNullOrWhiteSpace(connectionString))
            {
                SqlConnection connection = new SqlConnection(connectionString);
                Server server = new Server(new ServerConnection(connection));
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(command);
                    return true;
                }
                catch (ExecutionFailureException e)
                {
                    // A non-SQL statement was included in the script.
                    LogException(e, log);
                    return false;
                }
            }
            else
            {
                // The script file was not read.
                return false;
            }
        }

        /// <summary>
        /// Executes the TSQL query provided.
        /// </summary>
        /// <param name="query">The TSQL query text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>A list of rows of data returned from the query.</returns>
        override public List<SqlRow> ExecuteQuery(String query, String connectionString, EventLog.EventLog log)
        {
            // Check that the query and connection string are supplied.
            if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(connectionString))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Server server = new Server(new ServerConnection(connection));
                    try
                    {
                        // Execute the query.
                        List<SqlRow> results = new List<SqlRow>();
                        using (SqlDataReader reader = server.ConnectionContext.ExecuteReader(query))
                        {
                            while (reader.Read())
                            {
                                // Add each row's data to the results list.
                                SqlRow row = new SqlRow();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(i, reader.GetName(i), reader.GetValue(i));
                                }
                                results.Add(row);
                            }
                        }
                        return results;
                    }
                    catch (ExecutionFailureException e)
                    {
                        // A non-SQL statement was included in the script.
                        LogException(e, log);
                        return new List<SqlRow>();
                    }
                }
            }
            else
            {
                // The script file was not read.
                return new List<SqlRow>();
            }
        }
    }
}
