using System;
using System.Collections.Generic;
using System.Data;
using ParameterType = Galactic.Sql.StoredProcedure.ParameterType;

namespace Galactic.Sql
{
    /// <summary>
    /// An Interface for SQL Stored Procedures.
    /// </summary>
    public interface IStoredProcedure
    {
        // ---------- PROPERTIES ----------

        /// <summary>
        /// The name of the stored procedure.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The connection to the database.
        /// </summary>
        IDbConnection Connection { get; set; }

        /// <summary>
        /// The command to be sent to the database.
        /// </summary>
        IDbCommand Command { get; set; }

        // ---------- METHODS ----------

        /// <summary>
        /// Adds a parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="valueType">The database type of the value to add with the parameter.</param>
        /// <param name="parameterType">The object type of the parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <param name="maxSize">The maximum size of the parameter's value in bytes (if relevant).</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddParameter(string name, Object value, DbType valueType, Type parameterType, ParameterType direction, int? maxSize = null);

        /// <summary>
        /// Adds a boolean parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddBooleanParameter(string name, bool? value, ParameterType direction);

        /// <summary>
        /// Adds a 32-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddInt32Parameter(string name, Int32? value, ParameterType direction);

        /// <summary>
        /// Adds a 64-bit integer parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddInt64Parameter(string name, Int64? value, ParameterType direction);

        /// <summary>
        /// Adds a date and time parameter to the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddDateTimeParameter(string name, DateTime value, ParameterType direction);

        /// <summary>
        /// Adds an Unicode fixed length character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the fixed length character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddNCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Adds an Unicode variable character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_NVARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddNVarCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Adds an Non-Unicode variable character parameter to the procedure.
        /// Strings longer than SqlUtility.MAX_VARCHAR_LENGTH and the length argument are truncated to fit.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="length">The length of the variable character parameter.</param>
        /// <param name="direction">The direction of the parameter. Use one of the class parameter constants. Defaults to IN.</param>
        /// <returns>True if the parameter is added, false otherwise.</returns>
        bool AddVarCharParameter(string name, string value, int length, ParameterType direction);

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <returns>A List containing SqlRows or null if the Connection and Command for this procedure are not initialized.</returns>
        List<SqlRow> Execute();

        /// <summary>
        /// Executes a non query procedure (Insert, Update, Delete).
        /// </summary>
        /// <returns>The number of rows affected by the query.</returns>
        /// <exception cref="System.ApplicationException">Thrown if there is an error executing the stored procedure.</exception>
        int ExecuteNonQuery();

        /// <summary>
        /// Gets a parameter's value from the procedure.
        /// </summary>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>The object that matches the value of the parameter, null if the parameter could not be found.</returns>
        object GetParameter(string name);

        /// <summary>
        /// Prepends a @ to the name of the parameter for use in SQL calls.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter name with a @ prepended.</returns>
        string GetParamName(string name);
    }
}
