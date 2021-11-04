using Galactic.EventLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace Galactic.Sql.Oracle
{
    /// <summary>
    /// Provides various utility methods for use with Oracle databases.
    /// </summary>
    public class OracleUtility : SqlUtility
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// A list of valid DBAPriviliege Oracle system level privilege constants.
        /// </summary>
        public struct DBAPrivilege
        {
            static public string NONE = "NONE";
            static public string SYSDBA = "SYSDBA";
            static public string SYSOPER = "SYSOPER";
        }

        /// <summary>
        /// The default port number for Oracle database client connections.
        /// </summary>
        static public int DefaultPortNumber = 1521;

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The maximum length of a NCHAR field in the database.
        /// </summary>
        public override int MAX_NCHAR_LENGTH
        {
            get
            {
                return 2000;
            }
        }

        /// <summary>
        /// The maximum length of a NVARCHAR field in the database.
        /// </summary>
        public override int MAX_NVARCHAR_LENGTH
        {
            get
            {
                return 2000;
            }
        }

        /// <summary>
        /// The maximum length of a VARCHAR field in the database.
        /// </summary>
        public override int MAX_VARCHAR_LENGTH
        {
            get
            {
                return 4000;
            }
        }

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

        /// <summary>
        /// Builds a connection string using TNS.
        /// </summary>
        /// <param name="addressName">The address name of the database to connect to, as defined in the tnsnames.ora file.</param>
        /// <param name="accountName">The name of the account to use when connecting to the database server.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="trustedConnection">Whether to use integrated security to create a trusted connection with the
        /// database server. An accountName and password are not required for a trusted connection, the Windows account
        /// running the application will be used.</param>
        /// <param name="loadBalancing">Whether connection should be optimized for RAC databases.</param>
        /// <param name="dbaPrivilege">The database system level privilege to make the connection with. DBAPrivilege defines all valid values.
        /// Use NONE for non-system connections</param>
        /// <returns>The built connection string, or null if the string could not be built.</returns>
        static public string BuildConnectionString(string addressName, string accountName, string password, bool trustedConnection, bool loadBalancing, string dbaPrivilege)
        {
            // Check that a host name was supplied.
            if (!string.IsNullOrWhiteSpace(addressName))
            {
                // An address name was supplied.

                // Create the data source field for the connection string.
                string dataSource = addressName;

                // Return the finished connection string.
                return BuildConnectionStringFromDataSource(dataSource, accountName, password, trustedConnection, loadBalancing, dbaPrivilege);
            }
            else
            {
                // An address name was not supplied.
                return null;
            }
        }

        /// <summary>
        /// Builds a connection string.
        /// </summary>
        /// <param name="hostName">The name of the database server.</param>
        /// <param name="portNumber">The port number to use when connecting to the database server.</param>
        /// <param name="serviceName">The name of the service on the database server.</param>
        /// <param name="accountName">The name of the account to use when connecting to the database server.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="trustedConnection">Whether to use integrated security to create a trusted connection with the
        /// database server. An accountName and password are not required for a trusted connection, the Windows account
        /// running the application will be used.</param>
        /// <param name="loadBalancing">Whether connection should be optimized for RAC databases.</param>
        /// <param name="dbaPrivilege">The database system level privilege to make the connection with. DBAPrivilege defines all valid values.
        /// Use NONE for non-system connections</param>
        /// <returns>The built connection string, or null if the string could not be built.</returns>
        static public string BuildConnectionString(string hostName, int portNumber, string serviceName,
            string accountName, string password, bool trustedConnection, bool loadBalancing, string dbaPrivilege)
        {
            // Check that a host name was supplied.
            if (!string.IsNullOrWhiteSpace(hostName) && !string.IsNullOrWhiteSpace(serviceName))
            {
                // A host name and service name was supplied.

                // Create the data source field for the connection string.
                string dataSource = "(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = " + hostName + ") (PORT = " + portNumber + ") (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = " + serviceName + ")))";

                // Return the finished connection string.
                return BuildConnectionStringFromDataSource(dataSource, accountName, password, trustedConnection, loadBalancing, dbaPrivilege);
            }
            else
            {
                // A host or service name was not supplied.
                return null;
            }
        }

        /// <summary>
        /// Builds a connection string.
        /// </summary>
        /// <param name="dataSource">The data source field of the connection string to build.</param>
        /// <param name="accountName">The name of the account to use when connecting to the database server.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="trustedConnection">Whether to use integrated security to create a trusted connection with the
        /// database server. An accountName and password are not required for a trusted connection, the Windows account
        /// running the application will be used.</param>
        /// <param name="loadBalancing">Whether connection should be optimized for RAC databases.</param>
        /// <param name="dbaPrivilege">The database system level privilege to make the connection with. DBAPrivilege defines all valid values.
        /// Use NONE for non-system connections</param>
        /// <returns>The built connection string, or null if the string could not be built.</returns>
        static private string BuildConnectionStringFromDataSource(string dataSource, string accountName, string password, bool trustedConnection, bool loadBalancing, string dbaPrivilege)
        {
            if (!string.IsNullOrWhiteSpace(dataSource) && !string.IsNullOrWhiteSpace(accountName) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(dbaPrivilege))
            {
                // A host name was supplied.

                OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder
                {
                    // Create the connection string.
                    DataSource = dataSource,
                    // Optimize the connection for RAC load balancing if necessary.
                    LoadBalancing = loadBalancing
                };

                // Set the system privilege level of the connection, if specified.
                if (dbaPrivilege != DBAPrivilege.NONE)
                {
                    builder.DBAPrivilege = dbaPrivilege;
                }

                // Check whether a trusted connection should be created.
                if (trustedConnection)
                {
                    // Use the application's Windows account and integrated security to create a trusted connection.
                    builder.ConnectionString += "Integrated Security=SSPI;";
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
                // All of the required values to build the string were not provided.
                return null;
            }
        }

        /// <summary>
        /// Logs an Oracle exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="log">The event log to log the execption to.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        protected bool LogException(OracleException e, EventLog.EventLog log)
        {
            if (log != null)
            {
                log.Log(new Event(typeof(OracleUtility).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
                    "Number:\n" +
                    e.Number + "\n" +
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
        /// Executes the non-query PL/SQL command provided.
        /// </summary>
        /// <param name="command">The PL/SQL command text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if the command ran, false otherwise.</returns>
        override public bool ExecuteNonQuery(String command, String connectionString, EventLog.EventLog log)
        {
            // Check that the command and connection string are supplied.
            if (!string.IsNullOrWhiteSpace(command) && !string.IsNullOrWhiteSpace(connectionString))
            {
                // Get the connection to the database.
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    try
                    {
                        // Open the connection to the server.
                        connection.Open();

                        // Create a command that contains the text of the script.
                        using (OracleCommand oracleCommand = new OracleCommand())
                        {
                            oracleCommand.Connection = connection;
                            oracleCommand.CommandText = command;
                            oracleCommand.CommandType = CommandType.Text;

                            // Execute the command.
                            oracleCommand.ExecuteNonQuery();
                            return true;
                        }
                    }
                    catch (OracleException e)
                    {
                        // There was an error executing the script.
                        LogException(e, log);
                        return false;
                    }
                    catch (Exception e)
                    {
                        // There was an error executing the script.
                        LogException(e, log);
                        return false;
                    }
                }
            }
            else
            {
                // The script file was not read.
                return false;
            }
        }

        /// <summary>
        /// Executes the PL/SQL query provided.
        /// </summary>
        /// <param name="query">The PL/SQL query text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <param name="exception">Outputs an exception if there was one during the processing of the query. Null otherwise.</param>
        /// <returns>A list of rows of data returned from the query.</returns>
        public List<SqlRow> ExecuteQuery(String query, String connectionString, EventLog.EventLog log, out Exception exception)
        {
            // Check that the query and connection string are supplied.
            if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(connectionString))
            {
                // Get the connection to the database.
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    try
                    {
                        // Open the connection to the server.
                        connection.Open();

                        // Create a command that contains the text of the script.
                        using (OracleCommand queryCommand = new OracleCommand())
                        {
                            queryCommand.Connection = connection;
                            queryCommand.CommandText = query;
                            queryCommand.CommandType = CommandType.Text;

                            // Execute the query.
                            List<SqlRow> results = new List<SqlRow>();
                            using (OracleDataReader reader = queryCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    // Add each row's data to the results list.
                                    while (reader.Read())
                                    {
                                        SqlRow row = new SqlRow();
                                        for (int columnNum = 0; columnNum < reader.FieldCount; columnNum++)
                                        {
                                            row.Add(columnNum, reader.GetName(columnNum), reader.GetValue(columnNum));
                                        }
                                        results.Add(row);
                                    }
                                }
                            }
                            exception = null;
                            return results;
                        }
                    }
                    catch (OracleException e)
                    {
                        // There was an error executing the script.
                        LogException(e, log);
                        exception = e;
                        return new List<SqlRow>();
                    }
                    catch (Exception e)
                    {
                        // There was an error executing the script.
                        LogException(e, log);
                        exception = e;
                        return new List<SqlRow>();
                    }
                }
            }
            else
            {
                // The script file was not read.
                exception = null;
                return new List<SqlRow>();
            }
        }

        /// <summary>
        /// Executes the PL/SQL query provided.
        /// </summary>
        /// <param name="query">The PL/SQL query text to execute.</param>
        /// <param name="connectionString">The connection string of the database to execute the script against.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>A list of rows of data returned from the query.</returns>
        override public List<SqlRow> ExecuteQuery(String query, String connectionString, EventLog.EventLog log)
        {
            Exception e = null;
            return ExecuteQuery(query, connectionString, log, out e);
        }
    }
}