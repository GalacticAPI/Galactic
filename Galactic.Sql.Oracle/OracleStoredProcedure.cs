using Oracle.DataAccess.Client;
using System;
using System.Data;

namespace Galactic.Sql.Oracle
{
    /// <summary>
    /// An SQL Stored Procedure.
    /// </summary>
    public class OracleStoredProcedure : StoredProcedure
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The value for MAX sized nvarchar parameters.
        /// </summary>
        public const int MAX_NVARCHAR_SIZE = -1;

        /// <summary>
        /// The value for MAX sized varchar parameters.
        /// </summary>
        public const int MAX_VARCHAR_SIZE = -1;

        // ---------- VARIABLES ----------

        // The OracleConnection exposed by Connection.
        private OracleConnection connection = null;

        // The OracleCommand exposed by Command.
        private OracleCommand command = null;

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The connection to the database.
        /// </summary>
        public sealed override IDbConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = (OracleConnection)value;
            }
        }

        /// <summary>
        /// The command to be sent to the database.
        /// </summary>
        public sealed override IDbCommand Command
        {
            get
            {
                return command;
            }
            set
            {
                command = (OracleCommand)value;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a stored procedure.
        /// </summary>
        /// <param name="connectionString">The connection string for the SQL database to which the procedure belongs.</param>
        /// <param name="procedureName">The name of the procedure.</param>
        /// <param name="log">An event log to log exceptions to. May be null if no exception logging is desired.</param>
        public OracleStoredProcedure(string connectionString, string procedureName, EventLog.EventLog log)
        {
            // Check that parameters were supplied.
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(procedureName))
            {
                // Initialize the connection and command.
                Name = procedureName;
                Connection = new OracleConnection(connectionString);
                Command = new OracleCommand(Name, (OracleConnection)Connection);
                Command.CommandType = CommandType.StoredProcedure;
            }
            Log = log;
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Adds a boolean parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddBooleanParameter(string name, bool? value, ParameterType direction)
        {
            return AddParameter(name, value, DbType.Boolean, typeof(OracleParameter), direction);
        }

        /// <summary>
        /// Adds a 32-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddInt32Parameter(string name, Int32? value, ParameterType direction)
        {
            return AddParameter(name, value, DbType.Int32, typeof(OracleParameter), direction);
        }

        /// <summary>
        /// Adds a 64-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddInt64Parameter(string name, Int64? value, ParameterType direction)
        {
            return AddParameter(name, value, DbType.Int64, typeof(OracleParameter), direction);
        }

        /// <summary>
        /// Adds a date and time parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddDateTimeParameter(string name, DateTime value, ParameterType direction)
        {
            return AddParameter(name, value, DbType.DateTime2, typeof(OracleParameter), direction);
        }

        /// <summary>
        /// Adds an Unicode fixed length character parameter to the procedure.
        /// Strings longer than Sql.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddNCharParameter(string name, string value, int length, ParameterType direction)
        {
            // Truncate the string value if necessary.
            if (!string.IsNullOrWhiteSpace(value))
            {
                OracleUtility utility = new OracleUtility();
                if (value.Length > utility.MAX_NCHAR_LENGTH)
                {
                    value = value.Substring(0, utility.MAX_NVARCHAR_LENGTH);
                }
            }

            // Set the size of the parameter to twice the length of the string because each
            // Unicode character is 2 bytes long.
            return AddParameter(name, value, DbType.String, typeof(OracleParameter), direction, length * 2);
        }

        /// <summary>
        /// Adds an Unicode variable character parameter to the procedure.
        /// Strings longer than Sql.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddNVarCharParameter(string name, string value, int length, ParameterType direction)
        {
            // Truncate the string value if necessary.
            if (!string.IsNullOrWhiteSpace(value) && length != MAX_NVARCHAR_SIZE)
            {
                OracleUtility utility = new OracleUtility();
                if (value.Length > utility.MAX_NVARCHAR_LENGTH)
                {
                    value = value.Substring(0, utility.MAX_NVARCHAR_LENGTH);
                }
            }

            // Check whether to return a MAX sized parameter.
            if (length != MAX_NVARCHAR_SIZE)
            {
                // The parameter fits the requirements for a normal nvarchar parameter.
                // Set the size of the parameter to twice the length of the string because each
                // Unicode character is 2 bytes long.
                return AddParameter(name, value, DbType.String, typeof(OracleParameter), direction, length * 2);
            }
            else
            {
                // The parameter is too large for a normal nvarchar parameter.
                // Create a MAX sized parameter and set the size to the special value associated with max sized parameters.
                return AddParameter(name, value, DbType.String, typeof(OracleParameter), direction, MAX_NVARCHAR_SIZE);
            }
        }

        /// <summary>
        /// Adds an Non-Unicode variable character parameter to the procedure.
        /// Strings longer than Sql.MAX_VARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public override bool AddVarCharParameter(string name, string value, int length, ParameterType direction)
        {
            // Truncate the string value if necessary.
            if (!string.IsNullOrWhiteSpace(value) && length != MAX_VARCHAR_SIZE)
            {
                OracleUtility utility = new OracleUtility();
                if (value.Length > utility.MAX_VARCHAR_LENGTH)
                {
                    value = value.Substring(0, utility.MAX_VARCHAR_LENGTH);
                }
            }

            // Check whether to return a MAX sized parameter.
            if (length != MAX_VARCHAR_SIZE)
            {
                // The parameter fits the requirements for a normal nvarchar parameter.
                // Set the size of the parameter to the length of the string because each
                // ANSI character is 1 byte long.
                return AddParameter(name, value, DbType.String, typeof(OracleParameter), direction, length);
            }
            else
            {
                // The parameter is too large for a normal nvarchar parameter.
                // Create a MAX sized parameter and set the size to the special value associated with max sized parameters.
                return AddParameter(name, value, DbType.String, typeof(OracleParameter), direction, MAX_VARCHAR_SIZE);
            }
        }

        /// <summary>
        /// Gets a parameter's value from the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>The object that matches the value of the parameter, null if the parameter could not be found.</returns>
        public override object GetParameter(string name)
        {
            // Check that this procedure's command is initialized, and the name of the parameter was supplied.
            if (Command != null && !string.IsNullOrEmpty(name))
            {
                // The command is initialized, and the parameter name was supplied.
                return ((OracleCommand)Command).Parameters[GetParamName(name)].Value;
            }
            // The command was not initialized, or the name of the parameter was not supplied.
            return null;
        }

        /// <summary>
        /// Prepends a @ to the name of the parameter for use in SQL calls.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter name with a @ prepended.</returns>
        public override string GetParamName(string name)
        {
            return "@" + name;
        }
    }
}