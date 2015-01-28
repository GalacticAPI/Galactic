using Galactic.EventLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Galactic.Sql
{
    /// <summary>
    /// An SQL Stored Procedure.
    /// </summary>
    public abstract class StoredProcedure : IExceptionLogger, IStoredProcedure
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The type of parameter used by the stored procedure.
        /// </summary>
        public enum ParameterType
        {
            /// <summary>
            /// A parameter that provides data to the procedure.
            /// </summary>
            In,

            /// <summary>
            /// A parameter that returns data from a procedure.
            /// </summary>
            Out,

            /// <summary>
            /// A parameter that provides and returns data from a procedure.
            /// </summary>
            InOut,

            /// <summary>
            /// The return value of a procedure.
            /// </summary>
            Return
        }

        // ---------- VARIABLES ----------

        /// <summary>
        /// The event log that will receive events from this logger.
        /// </summary>
        protected EventLog.EventLog log = null;

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The event log that will receive events from this logger.
        /// </summary>
        public EventLog.EventLog Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
            }
        }

        /// <summary>
        /// The name of the stored procedure.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The connection to the database.
        /// </summary>
        public abstract IDbConnection Connection { get; set; }

        /// <summary>
        /// The command to be sent to the database.
        /// </summary>
        public abstract IDbCommand Command { get; set; }

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

        /// <summary>
        /// Adds a parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="valueType">The database type of the value to add with the parameter.</param>
        /// <param name="parameterType">The object type of the parameter. Must be dervied from System.Data.Common.DbParameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <param name="maxSize">The maximum size of the parameter's value in bytes (if relevant).</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public bool AddParameter(string name, Object value, DbType valueType, Type parameterType, ParameterType direction, int? maxSize = null)
        {
            // Check that this procedure's command is initialized, the name of the parameter was supplied and the parameter is not null.
            if (Command != null && !string.IsNullOrEmpty(name) && parameterType != null)
            {
                // The command is initialized, the parameter name was supplied, and the supplied parameter is not null.

                // Check if the parameter type specified is subclassed from DbParameter.
                if (parameterType.IsSubclassOf(typeof(DbParameter)))
                {
                    // Create the parameter.
                    DbParameter parameter = (DbParameter)Activator.CreateInstance(parameterType);

                    // Set the name of the parameter.
                    parameter.ParameterName = GetParamName(name);

                    // Set the database type of the parameter.
                    parameter.DbType = valueType;

                    // Set the maximum size of the parameter's value in bytes if applicable.
                    if (maxSize != null)
                    {
                        parameter.Size = (int)maxSize;
                    }

                    // Set the type of the parameter.
                    if (direction == ParameterType.Out)
                    {
                        parameter.Direction = ParameterDirection.Output;
                    }
                    else if (direction == ParameterType.InOut)
                    {
                        parameter.Direction = ParameterDirection.InputOutput;
                    }
                    else if (direction == ParameterType.Return)
                    {
                        parameter.Direction = ParameterDirection.ReturnValue;
                    }
                    else
                    {
                        parameter.Direction = ParameterDirection.Input;
                    }

                    // Add the parameter to the command to be used for the procedure.
                    Command.Parameters.Add(parameter);

                    // Check if the value supplied is null.
                    if (value != null)
                    {
                        // The value supplied was not null.

                        // Set the value supplied with the parameter.
                        ((DbParameter)Command.Parameters[GetParamName(name)]).Value = value;
                    }
                    else
                    {
                        // The value is null. Add a NULL value.
                        ((DbParameter)Command.Parameters[GetParamName(name)]).Value = DBNull.Value;
                    }
                    return true;
                }
                else
                {
                    // parameterType is not subclassed from DbParameter
                    return false;
                }
            }
            // The command was not initialized, or the name of the parameter was not supplied.
            return false;
        }

        /// <summary>
        /// Adds a boolean parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddBooleanParameter(string name, bool? value, ParameterType direction);

        /// <summary>
        /// Adds a 32-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddInt32Parameter(string name, Int32? value, ParameterType direction);

        /// <summary>
        /// Adds a 64-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddInt64Parameter(string name, Int64? value, ParameterType direction);

        /// <summary>
        /// Adds a date and time parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddDateTimeParameter(string name, DateTime value, ParameterType direction);

        /// <summary>
        /// Adds an Unicode fixed length character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the fixed length character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddNCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Adds an Unicode variable character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddNVarCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Adds an Non-Unicode variable character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_VARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        public abstract bool AddVarCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <returns>A List containing SqlRows or null if the Connection and Command for this procedure are not initialized.</returns>
        /// <exception cref="System.ApplicationException">Thrown if there is an error executing the stored procedure.</exception>
        public List<SqlRow> Execute()
        {
            // Check whether the connection and command are initialized.
            if (Connection != null && Command != null)
            {
                // The List of records to return.
                List<SqlRow> rowList = new List<SqlRow>();

                // The connection and command are initialized.
                try
                {
                    Connection.Open();

                    //Initialize a reader to read the results of the procedure.
                    DbDataReader reader = (DbDataReader)Command.ExecuteReader();

                    // Read all records.
                    while (reader.Read())
                    {
                        // The SqlRow for storing the results of the procedure.
                        SqlRow row = new SqlRow();

                        // Read all fields.
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // The name of the field read.
                            string fieldName = reader.GetName(i);

                            // The value of the field read.
                            Object valueObject = reader[fieldName];

                            // Check if the value of the object is null.
                            if (valueObject != DBNull.Value)
                            {
                                // The value is not null.
                                // Add the value to the row.
                                row.Add(i, fieldName, reader[fieldName]);
                            }
                            else
                            {
                                // The value is null.
                                // Add a null value to the row.
                                row.Add(i, fieldName, null);
                            }
                        }
                        // Add the row the list of rows.
                        rowList.Add(row);
                    }
                    reader.Close();
                    return rowList;
                }
                catch (DbException e)
                {
                    throw new ApplicationException("Error executing " + Name + " stored procedure.", e);
                }
                finally
                {
                    if (Connection != null)
                    {
                        Connection.Close();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Executes a non query procedure (Insert, Update, Delete).
        /// </summary>
        /// <returns>The number of rows affected by the query.</returns>
        /// <exception cref="System.ApplicationException">Thrown if there is an error executing the stored procedure.</exception>
        public int ExecuteNonQuery()
        {
            // The number of rows affected by the procedure.
            int rowsAffected = 0;

            // Check whether the connection and command are initialized.
            if (Connection != null && Command != null)
            {
                // The connection and command are initialized.
                try
                {
                    Connection.Open();
                    rowsAffected = Command.ExecuteNonQuery();
                }
                catch (DbException e)
                {
                    throw new ApplicationException("Error executing " + Name + " stored procedure.", e);
                }
                finally
                {
                    if (Connection != null)
                    {
                        Connection.Close();
                    }
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// Gets a parameter's value from the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>The object that matches the value of the parameter, null if the parameter could not be found.</returns>
        public abstract object GetParameter(string name);

        /// <summary>
        /// Prepends a @ to the name of the parameter for use in SQL calls.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter name with a @ prepended.</returns>
        public abstract string GetParamName(string name);

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        public virtual bool LogException(Exception e)
        {
            if (Log != null)
            {
                Log.Log(new Event(typeof(StoredProcedure).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
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
