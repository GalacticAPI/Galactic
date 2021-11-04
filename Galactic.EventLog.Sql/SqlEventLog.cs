using Galactic.Sql;
using System;
using System.Collections.Generic;

namespace Galactic.EventLog.Sql
{
    /// <summary>
    /// SqlEventLog is a class that logs activity to a SQL Database table.
    /// </summary>
    public class SqlEventLog : EventLog
    {
        // ---------- CONSTANTS ----------

        // The name of the stored procedure that adds an event category to the log.
        private const string ADD_CATEGORY_PROCEDURE_NAME = "AddCategory";

        // The name of the stored procedure that adds an event to the log.
        private const string ADD_EVENT_PROCEDURE_NAME = "AddEvent";

        // The name of the stored procedure that adds an event severity level to the log.
        private const string ADD_SEVERITY_LEVEL_PROCEDURE_NAME = "AddSeverityLevel";

        // The name of the stored procedure that adds an event source to the log.
        private const string ADD_SOURCE_PROCEDURE_NAME = "AddSource";

        // The name of the stored procedure that deletes an event category from the log.
        private const string DELETE_CATEGORY_PROCEDURE_NAME = "DeleteCategory";

        // The name of the stored procedure that deletes an event from the log.
        private const string DELETE_EVENT_PROCEDURE_NAME = "DeleteEvent";

        // The name of the stored procedure that deletes an event severity level from the log.
        private const string DELETE_SEVERITY_LEVEL_PROCEDURE_NAME = "DeleteSeverityLevel";

        // The name of the stored procedure that deletes an event source from the log.
        private const string DELETE_SOURCE_PROCEDURE_NAME = "DeleteSource";

        // The name of the stored procedure that gets event categories from the log.
        private const string GET_CATEGORIES_PROCEDURE_NAME = "GetCategories";

        // The name of the stored procedure that gets events from the log.
        private const string GET_EVENTS_PROCEDURE_NAME = "GetEvents";

        // The name of the stored procedure that gets event severity levels from the log.
        private const string GET_SEVERITY_LEVELS_PROCEDURE_NAME = "GetSeverityLevels";

        // The name of the stored procedure that gets event sources from the log.
        private const string GET_SOURCES_PROCEDURE_NAME = "GetSources";

        // The name of the stored procedure that updates an event category in the log.
        private const string UPDATE_CATEGORY_PROCEDURE_NAME = "UpdateCategory";

        // The name of the stored procedure that updates an event in the log.
        private const string UPDATE_EVENT_PROCEDURE_NAME = "UpdateEvent";

        // The name of the stored procedure that updates an event severity level in the log.
        private const string UPDATE_SEVERITY_LEVEL_PROCEDURE_NAME = "UpdateSeverityLevel";

        // The name of the stored procedure that updates an event source in the log.
        private const string UPDATE_SOURCE_PROCEDURE_NAME = "UpdateSource";

        // The name of the begin parameter used in stored procedures.
        private const string BEGIN_PARAMETER_NAME = "begin";

        // The name of the categoryId column in the database.
        private const string CATEGORY_ID_COLUMN_NAME = "categoryId";

        // The name of the categoryId parameter used in stored procedures.
        private const string CATEGORY_ID_PARAMETER_NAME = "categoryId";

        // The name of the date column in the database.
        private const string DATE_COLUMN_NAME = "date";

        // The name of the date parameter used in stored procedures.
        private const string DATE_PARAMETER_NAME = "date";

        // The name of the details column in database.
        private const string DETAILS_COLUMN_NAME = "details";

        // The name of the details parameter used in stored procedures.
        private const string DETAILS_PARAMETER_NAME = "details";

        // The name of the end parameter used in stored procedures.
        private const string END_PARAMETER_NAME = "end";

        // The name of the id column in the database.
        private const string ID_COLUMN_NAME = "id";

        // The name of the id parameter used in stored procedures.
        private const string ID_PARAMETER_NAME = "id";

        // The name of the name column in the database.
        private const string NAME_COLUMN_NAME = "name";

        // The name of the name parameter used in stored procedures.
        private const string NAME_PARAMETER_NAME = "name";

        // The name of the severityId column in the database.
        private const string SEVERITY_ID_COLUMN_NAME = "severityId";

        // The name of the severityId parameter used in stored procedures.
        private const string SEVERITY_ID_PARAMETER_NAME = "severityId";

        // The name of the sourceId column in the database.
        private const string SOURCE_ID_COLUMN_NAME = "sourceId";

        // The name of the sourceId parameter used in stored procedures.
        private const string SOURCE_ID_PARAMETER_NAME = "sourceId";

        // The length of the category name parameter used in stored procedures.
        private const int CATEGORY_NAME_PARAMETER_LENGTH = 80;

        // The length of the details parameter used in stored procedures.
        private const int DETAILS_PARAMETER_LENGTH = 4000;

        // The length of the source name parameter used in stored procedures.
        private const int SOURCE_NAME_PARAMETER_LENGTH = 80;

        // The length of the severity level name parameter used in stored procedures.
        private const int SEVERITY_LEVEL_NAME_PARAMETER_LENGTH = 20;

        // ---------- VARIABLES ----------

        // The connection string to use for this event log.
        private readonly string connectionString = null;

        // The type of stored procedure to use for the event log.
        private readonly Type storedProcedureType;

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a new SQL event log.
        /// </summary>
        /// <param name="connectionString">The connection string to use for this event log.</param>
        /// <param name="storedProcedureType">The type of stored procedure used for the event log.</param>
        public SqlEventLog(string connectionString, Type storedProcedureType)
        {
            // Check that we retrieved a connection string.
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // We did not retrieve a string.
                // Throw an exception.
                throw new ArgumentNullException("connectionString");
            }

            this.connectionString = connectionString;

            // Set the type of stored procedure to use for the event log.
            if (storedProcedureType != null && storedProcedureType.IsSubclassOf(typeof(StoredProcedure)))
            {
                this.storedProcedureType = storedProcedureType;
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Create a stored procedure of the desired database type.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect with the database.</param>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="log">An event log used for logging with the stored procedure.</param>
        /// <param name="storedProcedureType">The type of stored procedure to create.</param>
        /// <returns>A stored procedure of the appropriate type for the database, or null if an error occurred or not enough information was supplied to create the procedure.</returns>
        private static IStoredProcedure CreateStoredProcedure(string connectionString, string procedureName, EventLog log, Type storedProcedureType)
        {
            if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(procedureName))
            {
                // Check what type of stored procedure to create.
                if (storedProcedureType != null && storedProcedureType.IsSubclassOf(typeof(StoredProcedure)))
                {
                    return (StoredProcedure)storedProcedureType.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(EventLog) }).Invoke(new object[] { connectionString, procedureName, log });
                }
                else
                {
                    // The type supplied is not a subclass of StoredProcedure.
                    return null;
                }
            }
            else
            {
                // Not enough information was provided to create the stored procedure.
                return null;
            }
        }

        /// <summary>
        /// Finds events that match the filters provided.
        /// See each parameter for information on how to filter the search.
        /// </summary>
        /// <param name="source">Searches for all events with sources that match the string provided. Returns events of all sources on a null or empty string.</param>
        /// <param name="severity">Searches for events with the specified severity level. Returns events of all severity levels on null.</param>
        /// <param name="category">Searches for events with categories that match the string provided. Returns events of all categories on a null or empty string.</param>
        /// <param name="begin">Returns events with a date/time on or after the date/time supplied. Does not put a lower range on event dates on a null date/time.</param>
        /// <param name="end">Returns events with a date/time on or before the date/time supplied. Does not put an upper range on event dates on a null date/time.</param>
        /// <returns>A list of events sorted from earliest to latest that match the given search parameters.</returns>
        override public List<Event> Find(string source, Event.SeverityLevels? severity, string category, DateTime? begin, DateTime? end)
        {
            // Get the source ID of the source string provided.
            int sourceId = 0;
            if (!string.IsNullOrWhiteSpace(source))
            {
                List<SqlEventLogNamedItem> sources = GetSources(0, source);
                if (sources.Count > 0)
                {
                    sourceId = sources[0].Id;
                }
                else
                {
                    return new List<Event>();
                }
            }

            // Get the severity level ID of the severity level provided.
            int severityId = 0;
            if (severity != null)
            {
                // Get the string for the severity level provided.
                string severityLevelString = Event.GetSeverityLevelString((Event.SeverityLevels)severity);

                // Check that the severity level provided corresponds with a known severity level.
                if (!string.IsNullOrWhiteSpace(severityLevelString))
                {
                    // The severity level provided is known.
                    List<SqlEventLogNamedItem> severityLevels = GetSeverityLevels(0, Event.GetSeverityLevelString((Event.SeverityLevels)severity));

                    if (severityLevels.Count > 0)
                    {
                        severityId = severityLevels[0].Id;
                    }
                    else
                    {
                        return new List<Event>();
                    }
                }
                else
                {
                    // The severity level provided is unknown.
                    return new List<Event>();
                }
            }

            // Get the category ID of the category string provided.
            int categoryId = 0;
            if (!string.IsNullOrWhiteSpace(category))
            {
                List<SqlEventLogNamedItem> categories = GetCategories(0, category);
                if (categories.Count > 0)
                {
                    categoryId = categories[0].Id;
                }
                else
                {
                    return new List<Event>();
                }
            }

            // Get the matching events from the log.
            List<SqlEventLogEvent> logEvents = GetEvents(sourceId, categoryId, severityId, begin, end);

            // Convert the SqlEventLogEvents to Events.
            List<Event> events = new List<Event>();
            foreach (SqlEventLogEvent logEvent in logEvents)
            {
                events.Add(new Event(logEvent.Source.Name, logEvent.Date, Event.GetSeverityLevel(logEvent.SeverityLevel.Name), logEvent.Category.Name, logEvent.Details));
            }

            // Return the list of events.
            return events;
        }

        /// <summary>
        /// Logs the event to the event log.
        /// </summary>
        /// <param name="eventToLog">The event to log.</param>
        /// <returns>True if the event was logged successfully. False otherwise.</returns>
        override public bool Log(Event eventToLog)
        {
            // Check that an event was provided.
            if (eventToLog != null)
            {
                // An event was provided.

                // Check that the required properties of an event were populated.
                if (!string.IsNullOrWhiteSpace(eventToLog.Source) &&
                    !string.IsNullOrWhiteSpace(eventToLog.Category) &&
                    !string.IsNullOrWhiteSpace(eventToLog.Details))
                {
                    // The required properties of an event were populated.
                    // Add the event to the log.
                    return AddEvent(eventToLog.Source, eventToLog.Category, eventToLog.Severity, eventToLog.Details);
                }
                else
                {
                    // The required properties were not populated.
                    return false;
                }
            }
            else
            {
                // An event was not provided.
                return false;
            }
        }

        /// <summary>
        /// Adds an event category to the log.
        /// Categories help to classify the type of event that occurred.
        /// </summary>
        /// <param name="name">The name of the event category.</param>
        /// <returns>True if the event category was added, false otherwise.</returns>
        protected bool AddCategory(string name)
        {
            return ExecuteAddByNameStoredProcedure(name, ADD_CATEGORY_PROCEDURE_NAME, CATEGORY_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Adds an event to the log.
        /// </summary>
        /// <param name="source">A name for the source of the event.</param>
        /// <param name="category">A category that classifies the type of event.</param>
        /// <param name="severityLevel">The importance of the event.</param>
        /// <param name="details">A detailed description of the event.</param>
        /// <returns>True if the event was added, false otherwise.</returns>
        protected bool AddEvent(string source, string category, Event.SeverityLevels severityLevel, string details)
        {
            // Check that all parameters were provided.
            if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(details))
            {
                // All parameters were provided.

                // Check whether the event source already exist in the log.
                List<SqlEventLogNamedItem> sources = GetSources(0, source);
                if (sources.Count == 0)
                {
                    // The event source does not already exist in the log. Add it.
                    bool addSuccessful = AddSource(source);
                    if (!addSuccessful)
                    {
                        // The add was not successful.
                        return false;
                    }
                    // Re-get the event sources from the log so that sources has the newly added source.
                    sources = GetSources(0, source);
                }

                // Check whether the event category already exists in the log.
                List<SqlEventLogNamedItem> categories = GetCategories(0, category);
                if (categories.Count == 0)
                {
                    // The event category does not already exist in the log. Add it.
                    bool addSuccessful = AddCategory(category);
                    if (!addSuccessful)
                    {
                        // The add was not successful.
                        return false;
                    }
                    // Re-get the event categories from the log so that categories has the newly added category.
                    categories = GetCategories(0, category);
                }

                // Check whether the event severity level already exists in the log.
                string severityLevelString = Event.GetSeverityLevelString(severityLevel);
                List<SqlEventLogNamedItem> severityLevels = new List<SqlEventLogNamedItem>();
                if (!string.IsNullOrWhiteSpace(severityLevelString))
                {
                    severityLevels = GetSeverityLevels(0, severityLevelString);
                }
                if (severityLevels.Count == 0)
                {
                    // The event severity level does not exist in the log.
                    bool addSuccessful = AddSeverityLevel(severityLevelString);
                    if (!addSuccessful)
                    {
                        // The add was not successful.
                        return false;
                    }
                    // Re-get the event severity levels from the log so that severityLevels has the newly added severity level.
                    severityLevels = GetSeverityLevels(0, severityLevelString);
                }

                // The stored procedure to add an event to the log.
                IStoredProcedure addEvent = CreateStoredProcedure(connectionString, ADD_EVENT_PROCEDURE_NAME, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool sourceIdParameterAdded = addEvent.AddInt32Parameter(SOURCE_ID_PARAMETER_NAME, sources[0].Id, StoredProcedure.ParameterType.In);
                bool categoryParameterAdded = addEvent.AddInt32Parameter(CATEGORY_ID_PARAMETER_NAME, categories[0].Id, StoredProcedure.ParameterType.In);
                bool severityLevelParameterAdded = addEvent.AddInt32Parameter(SEVERITY_ID_PARAMETER_NAME, severityLevels[0].Id, StoredProcedure.ParameterType.In);
                bool detailsParameterAdded = addEvent.AddNVarCharParameter(DETAILS_PARAMETER_NAME, details, DETAILS_PARAMETER_LENGTH, StoredProcedure.ParameterType.In);

                // Check that all parameters were added.
                if (sourceIdParameterAdded && categoryParameterAdded && severityLevelParameterAdded && detailsParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to add the event to the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = addEvent.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected by the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // Rows were not affected.
                        return false;
                    }
                }
                else
                {
                    // The parameters were not added.
                    return false;
                }
            }
            else
            {
                // One or more parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Adds an event severity level to the log.
        /// Severity levels help to identify the importance of an event.
        /// </summary>
        /// <param name="name">The name for the severity level</param>
        /// <returns>True if the event severity level was added, false otherwise.</returns>
        protected bool AddSeverityLevel(string name)
        {
            return ExecuteAddByNameStoredProcedure(name, ADD_SEVERITY_LEVEL_PROCEDURE_NAME, SEVERITY_LEVEL_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Adds an event source to the log.
        /// Event sources identify where the event originated.
        /// </summary>
        /// <param name="name">The name of the event source.</param>
        /// <returns>True if the event source was added, false otherwise.</returns>
        protected bool AddSource(string name)
        {
            return ExecuteAddByNameStoredProcedure(name, ADD_SOURCE_PROCEDURE_NAME, SOURCE_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Deletes an event category from the log.
        /// </summary>
        /// <param name="id">The id of the event category in the database to delete.</param>
        /// <returns>True if the event category was deleted, false otherwise.</returns>
        protected bool DeleteCategory(int id)
        {
            return ExecuteDeleteByIdStoredProcedure(id, DELETE_CATEGORY_PROCEDURE_NAME);
        }

        /// <summary>
        /// Deletes an event from the log.
        /// </summary>
        /// <param name="date">The date/time of the event to delete.</param>
        /// <param name="sourceId">The ID of the source of the event to delete.</param>
        /// <returns>True if the event was deleted, false otherwise.</returns>
        protected bool DeleteEvent(DateTime date, int sourceId)
        {
            // Check that parameters were provided.
            if (sourceId > 0)
            {
                // Parameters were provided.

                // The stored procedure to delete an event from the log.
                IStoredProcedure deleteEvent = CreateStoredProcedure(connectionString, DELETE_EVENT_PROCEDURE_NAME, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool dateParameterAdded = deleteEvent.AddDateTimeParameter(DATE_PARAMETER_NAME, date, StoredProcedure.ParameterType.In);
                bool sourceIdParameterAdded = deleteEvent.AddInt32Parameter(SOURCE_ID_PARAMETER_NAME, sourceId, StoredProcedure.ParameterType.In);

                // Check that the parameters were added.
                if (dateParameterAdded && sourceIdParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to delete the event from the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = deleteEvent.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected due to the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // No rows were affected.
                        return false;
                    }
                }
                else
                {
                    // The parameters were not added.
                    return false;
                }
            }
            else
            {
                // Parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Deletes an event severity level from the log.
        /// </summary>
        /// <param name="id">The id of the event severity level in the database to delete.</param>
        /// <returns>True if the event severity level was deleted, false otherwise.</returns>
        protected bool DeleteSeverityLevel(int id)
        {
            return ExecuteDeleteByIdStoredProcedure(id, DELETE_SEVERITY_LEVEL_PROCEDURE_NAME);
        }

        /// <summary>
        /// Deletes an event source from the log.
        /// Note: Deleting an event source, deletes all events originating from this source in the log.
        /// </summary>
        /// <param name="id">The ID of the event source in the database to delete.</param>
        /// <returns>True if the event source was deleted, false otherwise.</returns>
        protected bool DeleteSource(int id)
        {
            return ExecuteDeleteByIdStoredProcedure(id, DELETE_SOURCE_PROCEDURE_NAME);
        }

        /// <summary>
        /// Executes a stored procedure that requires a name to add an item to the log.
        /// </summary>
        /// <param name="name">The value of the name to add.</param>
        /// <param name="procedureName">The name of the procedure that adds the item to the database.</param>
        /// <param name="nameParameterLength">The maximum allowable length of a name value in the database.</param>
        /// <returns>True if the procedure executed successfully, false otherwise.</returns>
        private bool ExecuteAddByNameStoredProcedure(string name, string procedureName, int nameParameterLength)
        {
            // Check that parameters were provided.
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(procedureName) && nameParameterLength > 0)
            {
                // Parameters were provided.

                // The stored procedure to add an item to the log.
                IStoredProcedure addItem = CreateStoredProcedure(connectionString, procedureName, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool nameParameterAdded = addItem.AddNVarCharParameter(NAME_PARAMETER_NAME, name, nameParameterLength, StoredProcedure.ParameterType.In);

                // Check that the parameter was added.
                if (nameParameterAdded)
                {
                    // The parameter was added.

                    // Execute the stored procedure to add the item to the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = addItem.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected due to the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // No rows were affected.
                        return false;
                    }
                }
                else
                {
                    // The parameter was not added.
                    return false;
                }
            }
            else
            {
                // Parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Executes a stored procedure that requires an ID to delete an item from the log.
        /// </summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <param name="procedureName">The name of the procedure that deletes the item to the database.</param>
        /// <returns>True if the procedure executed successfully, false otherwise.</returns>
        private bool ExecuteDeleteByIdStoredProcedure(int id, string procedureName)
        {
            // Check that parameters were provided.
            if (id > 0 && !string.IsNullOrWhiteSpace(procedureName))
            {
                // Parameters were provided.

                // The stored procedure to delete an item from the log.
                IStoredProcedure deleteItem = CreateStoredProcedure(connectionString, procedureName, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool idParameterAdded = deleteItem.AddInt32Parameter(ID_PARAMETER_NAME, id, StoredProcedure.ParameterType.In);

                // Check that the parameter was added.
                if (idParameterAdded)
                {
                    // The parameter was added.

                    // Execute the stored procedure to delete the item from the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = deleteItem.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected due to the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // No rows were affected.
                        return false;
                    }
                }
                else
                {
                    // The parameter was not added.
                    return false;
                }
            }
            else
            {
                // Parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Executes a stored procedure that retrieves items from the log based upon their id or name.
        /// </summary>
        /// <param name="id">The id of the items to get. Provide a 0 ID to return items with any ID from the log.</param>
        /// <param name="name">The name of the items to get. Provide and empty or null string to return items with any name from the log.</param>
        /// <param name="procedureName">The name of the procedure that deletes the item to the database.</param>
        /// <param name="nameParameterLength">The maximum allowable length of a name value in the database.</param>
        /// <returns>A list of items that match the criteria provided, or null on an error.</returns>
        private List<SqlEventLogNamedItem> ExecuteGetByIdAndNameStoredProcedure(int id, string name, string procedureName, int nameParameterLength)
        {
            // Check that parameters were provided.
            if (id >= 0 && !string.IsNullOrWhiteSpace(procedureName) && nameParameterLength > 0)
            {
                // Parameters were provided.

                // The stored procedure to get items from the log.
                IStoredProcedure getItems = CreateStoredProcedure(connectionString, procedureName, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool idParameterAdded = false;

                // Check whether a null value needs to be provided.
                if (id != 0)
                {
                    // Add the parameter normally.
                    idParameterAdded = getItems.AddInt32Parameter(ID_PARAMETER_NAME, id, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Add a null value.
                    idParameterAdded = getItems.AddInt32Parameter(ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }

                bool nameParameterAdded = false;
                nameParameterAdded = getItems.AddNVarCharParameter(NAME_PARAMETER_NAME, name, nameParameterLength, StoredProcedure.ParameterType.In);

                // Check that the parameters were added.
                if (idParameterAdded && nameParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to get the items from the log.

                    List<SqlRow> rows = null;
                    try
                    {
                        rows = getItems.Execute();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error executing the stored procedure.
                        return null;
                    }

                    // Convert the rows into named items to return.
                    List<SqlEventLogNamedItem> items = new List<SqlEventLogNamedItem>();
                    foreach (SqlRow row in rows)
                    {
                        SqlEventLogNamedItem item = new SqlEventLogNamedItem();
                        int? rowId = (int?)row[ID_COLUMN_NAME];
                        // Check that a value was returned from the ID column.
                        if (rowId != null)
                        {
                            // A value was returned.
                            // Set the the item's Id with the value returned. 
                            item.Id = (int)rowId;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        string rowName = (string)row[NAME_COLUMN_NAME];
                        // Check that a value was returned from the name column.
                        if (!string.IsNullOrWhiteSpace(rowName))
                        {
                            // A value was returned.
                            // Set the item's name with the value returned.
                            item.Name = rowName;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        // Add the item to the list.
                        items.Add(item);
                    }

                    // Return the list of items.
                    return items;
                }
                else
                {
                    // The parameters were not added.
                    return null;
                }
            }
            else
            {
                // Parameters were not provided.
                return null;
            }
        }

        /// <summary>
        /// Executes a stored procedure that updates a named item in the log.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="name">The name value for the item.</param>
        /// <param name="procedureName">The name of the procedure that updates the item in the database.</param>
        /// <param name="nameParameterLength">The maximum allowable length of a name value in the database.</param>
        /// <returns>True if the procedure executed successfully, false otherwise.</returns>
        private bool ExecuteUpdateByIdAndNameStoredProcedure(int id, string name, string procedureName, int nameParameterLength)
        {
            // Check that parameters were provided.
            if (id >= 0 && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(procedureName) && nameParameterLength > 0)
            {
                // Parameters were provided.

                // The stored procedure to update an item in the log.
                IStoredProcedure updateItem = CreateStoredProcedure(connectionString, procedureName, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool idParameterAdded = updateItem.AddInt32Parameter(ID_PARAMETER_NAME, id, StoredProcedure.ParameterType.In);

                bool nameParameterAdded = updateItem.AddNVarCharParameter(NAME_PARAMETER_NAME, name, nameParameterLength, StoredProcedure.ParameterType.In);

                // Check that the parameters were added.
                if (idParameterAdded && nameParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to update the item in the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = updateItem.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected due to the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // No rows were affected.
                        return false;
                    }
                }
                else
                {
                    // The parameters were not added.
                    return false;
                }
            }
            else
            {
                // Parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Gets event categories from the log that match the specified criteria.
        /// Note: See parameter descriptions for information on filtering your results.
        /// </summary>
        /// <param name="id">The ID of the event category in the database. Provide a 0 ID to return event categories with any ID from the log.</param>
        /// <param name="name">The name of the event category to return. Provide and empty or null string to return event categories with any name from the log.</param>
        /// <returns>A list of categories that match the criteria provided.</returns>
        protected List<SqlEventLogNamedItem> GetCategories(int id, string name)
        {
            return ExecuteGetByIdAndNameStoredProcedure(id, name, GET_CATEGORIES_PROCEDURE_NAME, CATEGORY_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Gets events from the log that match specified criteria.
        /// Note: See parameter descriptions for information on filtering your results.
        /// </summary>
        /// <param name="sourceId">The ID of the source from which events originated in the database. Provide a 0 ID to return events with any ID from the log.</param>
        /// <param name="categoryId">The ID of the category to which an event belongs in the database. Provide a 0 ID to return events with any ID from the log.</param>
        /// <param name="severityId">The ID of the severity level to which an event belongs in the database. Provide a 0 ID to return events with any ID from the log.</param>
        /// <param name="begin">Return events that occur on or after this date. Provide a null date to specify no lower bound on the date of events to return.</param>
        /// <param name="end">Returns events that occur on or before this date. Provide a null date to specify no upper bound on the date of events to return.</param>
        /// <returns>A list of events that match the criteria provided.</returns>
        protected List<SqlEventLogEvent> GetEvents(int sourceId, int categoryId, int severityId, DateTime? begin, DateTime? end)
        {
            // Check that parameters were provided and are valid.
            if (sourceId >= 0 && categoryId >= 0 && severityId >= 0 && ((begin == null || end == null) || (begin != null && end != null && begin <= end)))
            {
                // Parameters were provided.

                // The stored procedure to get items from the log.
                IStoredProcedure getItems = CreateStoredProcedure(connectionString, GET_EVENTS_PROCEDURE_NAME, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool sourceIdParameterAdded = false;

                // Check whether a null value needs to be provided.
                if (sourceId != 0)
                {
                    // Add the parameter normally.
                    sourceIdParameterAdded = getItems.AddInt32Parameter(SOURCE_ID_PARAMETER_NAME, sourceId, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Add a null value.
                    sourceIdParameterAdded = getItems.AddInt32Parameter(SOURCE_ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }

                bool categoryIdParameterAdded = false;

                // Check whether a null value needs to be provided.
                if (categoryId != 0)
                {
                    // Add the parameter normally.
                    categoryIdParameterAdded = getItems.AddInt32Parameter(CATEGORY_ID_PARAMETER_NAME, categoryId, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Add a null value.
                    categoryIdParameterAdded = getItems.AddInt32Parameter(CATEGORY_ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }

                bool severityIdParameterAdded = false;

                // Check whether a null value needs to be provided.
                if (severityId != 0)
                {
                    // Add the parameter normally.
                    severityIdParameterAdded = getItems.AddInt32Parameter(SEVERITY_ID_PARAMETER_NAME, severityId, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Add a null value.
                    severityIdParameterAdded = getItems.AddInt32Parameter(SEVERITY_ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }

                bool beginParameterAdded = false;
                // Check whether a null value was supplied.
                if (begin != null)
                {
                    // A non-null value was supplied.
                    beginParameterAdded = getItems.AddDateTimeParameter(BEGIN_PARAMETER_NAME, (DateTime)begin, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // A null value was supplied. Don't add a parameter.
                    beginParameterAdded = true;
                }

                bool endParameterAdded = false;
                // Check whether a null value was supplied.
                if (end != null)
                {
                    // A non-null value was supplied.
                    endParameterAdded = getItems.AddDateTimeParameter(END_PARAMETER_NAME, (DateTime)end, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // A null value was supplied. Don't add a parameter.
                    endParameterAdded = true;
                }

                // Check that the parameters were added.
                if (sourceIdParameterAdded && categoryIdParameterAdded && severityIdParameterAdded &&
                    beginParameterAdded && endParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to get the items from the log.

                    List<SqlRow> rows = null;
                    try
                    {
                        rows = getItems.Execute();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error executing the stored procedure.
                        return null;
                    }

                    // Convert the rows into events to return.
                    List<SqlEventLogEvent> logEvents = new List<SqlEventLogEvent>();
                    foreach (SqlRow row in rows)
                    {
                        SqlEventLogEvent logEvent = new SqlEventLogEvent();

                        DateTime? rowDate = (DateTime?)row[DATE_COLUMN_NAME];
                        // Check that a value was returned from the date column.
                        if (rowDate.HasValue)
                        {
                            // A value was returned.
                            // Set the the event's Date with the value returned. 
                            logEvent.Date = rowDate.GetValueOrDefault();
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        int? rowSourceId = (int?)row[SOURCE_ID_COLUMN_NAME];
                        // Check that a value was returned from the source ID column.
                        if (rowSourceId != null)
                        {
                            // A value was returned.
                            // Set the the event's SourceId with the value returned.
                            SqlEventLogNamedItem source = new SqlEventLogNamedItem
                            {
                                Id = (int)rowSourceId,
                                Name = GetSources((int)rowSourceId, null)[0].Name
                            };
                            logEvent.Source = source;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        int? rowCategoryId = (int?)row[CATEGORY_ID_COLUMN_NAME];
                        // Check that a value was returned from the category ID column.
                        if (rowCategoryId != null)
                        {
                            // A value was returned.
                            // Set the the event's CategoryId with the value returned. 
                            SqlEventLogNamedItem category = new SqlEventLogNamedItem
                            {
                                Id = (int)rowCategoryId,
                                Name = GetCategories((int)rowCategoryId, null)[0].Name
                            };
                            logEvent.Category = category;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        int? rowSeverityLevelId = (int?)row[SEVERITY_ID_COLUMN_NAME];
                        // Check that a value was returned from the severity level ID column.
                        if (rowSeverityLevelId != null)
                        {
                            // A value was returned.
                            // Set the the event's SeverityLevelId with the value returned.
                            SqlEventLogNamedItem severityLevel = new SqlEventLogNamedItem
                            {
                                Id = (int)rowSeverityLevelId,
                                Name = GetSeverityLevels((int)rowSeverityLevelId, null)[0].Name
                            };
                            logEvent.SeverityLevel = severityLevel;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        string rowDetails = (string)row[DETAILS_COLUMN_NAME];
                        // Check that a value was returned from the details column.
                        if (!string.IsNullOrWhiteSpace(rowDetails))
                        {
                            // A value was returned.
                            // Set the event's details with the value returned.
                            logEvent.Details = rowDetails;
                        }
                        else
                        {
                            // No value returned.
                            return null;
                        }

                        // Add the event to the list.
                        logEvents.Add(logEvent);
                    }

                    // Return the list of events.
                    return logEvents;
                }
                else
                {
                    // The parameters were not added.
                    return null;
                }
            }
            else
            {
                // Parameters were not provided.
                return null;
            }
        }

        /// <summary>
        /// Gets event severity levels from the log that match the specified criteria.
        /// Note: See parameter descriptions for information on filtering your results.
        /// </summary>
        /// <param name="id">The ID of the event severity level in the database. Provide a 0 ID to return event severity levels with any ID from the log.</param>
        /// <param name="name">The name of the event severity level to return. Provide an empty or null string to return event severity levels with any name from the log.</param>
        /// <returns>A list of event severity levels that match the criteria provided.</returns>
        protected List<SqlEventLogNamedItem> GetSeverityLevels(int id, string name)
        {
            return ExecuteGetByIdAndNameStoredProcedure(id, name, GET_SEVERITY_LEVELS_PROCEDURE_NAME, SEVERITY_LEVEL_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Gets event sources from the log that match the specified criteria.
        /// Note: See parameter descriptions for information on filtering your results.
        /// </summary>
        /// <param name="id">The ID of the event source in the database. Provide a 0 ID to return event sources with any ID from the log.</param>
        /// <param name="name">The name of the event source to return. Provide an empty or null string to return event sources with any name from the log.</param>
        /// <returns>A list of event sources that match the criteria provided.</returns>
        protected List<SqlEventLogNamedItem> GetSources(int id, string name)
        {
            return ExecuteGetByIdAndNameStoredProcedure(id, name, GET_SOURCES_PROCEDURE_NAME, SOURCE_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Updates the attributes of an existing event category.
        /// </summary>
        /// <param name="id">The ID of the event category in the database.</param>
        /// <param name="name">The name to give the event category.</param>
        /// <returns>True if the update succeeded, false otherwise.</returns>
        protected bool UpdateCategory(int id, string name)
        {
            return ExecuteUpdateByIdAndNameStoredProcedure(id, name, UPDATE_CATEGORY_PROCEDURE_NAME, CATEGORY_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Updates the attributes of an existing event.
        /// </summary>
        /// <param name="date">The date/time of the event to modify.</param>
        /// <param name="sourceId">The source ID of the event to modify.</param>
        /// <param name="category">A category that classifies the type of event.</param>
        /// <param name="severityLevel">The importance of the event. Provide a null to keep its current value.</param>
        /// <param name="details">The new details text assigned to the event. Provide an empty or null string to keep its current value.</param>
        /// <returns>True if the update succeeded, false otherwise.</returns>
        protected bool UpdateEvent(DateTime date, int sourceId, string category, Event.SeverityLevels? severityLevel, string details)
        {
            // Check that all parameters were provided.
            if (sourceId > 0)
            {
                // All parameters were provided.

                // Check whether the event exists in the log.
                List<SqlEventLogEvent> logEvents = GetEvents(sourceId, 0, 0, date, date);
                if (logEvents.Count == 0)
                {
                    // The event does not exist in the log.
                    return false;
                }

                // Check whether the event category already exists in the log.
                List<SqlEventLogNamedItem> categories = new List<SqlEventLogNamedItem>();
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categories = GetCategories(0, category);
                    if (categories.Count == 0)
                    {
                        // The event category does not already exist in the log. Add it.
                        bool addSuccessful = AddCategory(category);
                        if (!addSuccessful)
                        {
                            // The add was not successful.
                            return false;
                        }
                        // Re-get the event categories from the log so that categories has the newly added category.
                        categories = GetCategories(0, category);
                    }
                }

                // Check whether the event severity level already exists in the log.
                List<SqlEventLogNamedItem> severityLevels = new List<SqlEventLogNamedItem>();
                if (severityLevel != null)
                {
                    string severityLevelString = Event.GetSeverityLevelString((Event.SeverityLevels)severityLevel);
                    if (!string.IsNullOrWhiteSpace(severityLevelString))
                    {
                        severityLevels = GetSeverityLevels(0, severityLevelString);
                    }
                    if (severityLevels.Count == 0)
                    {
                        // The event severity level does not exist in the log.
                        bool addSuccessful = AddSeverityLevel(severityLevelString);
                        if (!addSuccessful)
                        {
                            // The add was not successful.
                            return false;
                        }
                        // Re-get the event severity levels from the log so that severityLevels has the newly added severity level.
                        severityLevels = GetSeverityLevels(0, severityLevelString);
                    }
                }

                // The stored procedure to update the event in the log.
                IStoredProcedure updateEvent = CreateStoredProcedure(connectionString, UPDATE_EVENT_PROCEDURE_NAME, null, storedProcedureType);

                // Add parameters to the stored procedure.
                bool dateParameterAdded = updateEvent.AddDateTimeParameter(DATE_PARAMETER_NAME, date, StoredProcedure.ParameterType.In);
                bool sourceIdParameterAdded = updateEvent.AddInt32Parameter(ID_PARAMETER_NAME, sourceId, StoredProcedure.ParameterType.In);
                bool categoryParameterAdded = false;
                // Check whether we need to supply a null value.
                if (categories != null && categories.Count > 0)
                {
                    // Provide a normal value.
                    categoryParameterAdded = updateEvent.AddInt32Parameter(CATEGORY_ID_PARAMETER_NAME, categories[0].Id, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Provide a null value.
                    categoryParameterAdded = updateEvent.AddInt32Parameter(CATEGORY_ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }
                bool severityLevelParameterAdded = false;
                // Check whether we need to supply a null value.
                if (severityLevels != null && severityLevels.Count > 0)
                {
                    // Provie a normal value.
                    severityLevelParameterAdded = updateEvent.AddInt32Parameter(SEVERITY_ID_PARAMETER_NAME, severityLevels[0].Id, StoredProcedure.ParameterType.In);
                }
                else
                {
                    // Provide a null value.
                    severityLevelParameterAdded = updateEvent.AddInt32Parameter(SEVERITY_ID_PARAMETER_NAME, null, StoredProcedure.ParameterType.In);
                }
                bool detailsParameterAdded = updateEvent.AddNVarCharParameter(DETAILS_PARAMETER_NAME, details, DETAILS_PARAMETER_LENGTH, StoredProcedure.ParameterType.In);

                // Check that all parameters were added.
                if (dateParameterAdded && sourceIdParameterAdded && categoryParameterAdded && severityLevelParameterAdded && detailsParameterAdded)
                {
                    // The parameters were added.

                    // Execute the stored procedure to update the event in the log.
                    int rowsAffected = 0;
                    try
                    {
                        rowsAffected = updateEvent.ExecuteNonQuery();
                    }
                    catch (ApplicationException)
                    {
                        // There was an error running the stored procedure.
                        return false;
                    }

                    // Check that there were rows affected by the procedure.
                    if (rowsAffected > 0)
                    {
                        // Rows were affected.
                        return true;
                    }
                    else
                    {
                        // Rows were not affected.
                        return false;
                    }
                }
                else
                {
                    // The parameters were not added.
                    return false;
                }
            }
            else
            {
                // One or more parameters were not provided.
                return false;
            }
        }

        /// <summary>
        /// Updates the attributes of an existing event severity level.
        /// </summary>
        /// <param name="id">The ID of the event severity level in the database.</param>
        /// <param name="name">The name to give the event severity level.</param>
        /// <returns>True if the update succeeded, false otherwise.</returns>
        protected bool UpdateSeverityLevel(int id, string name)
        {
            return ExecuteUpdateByIdAndNameStoredProcedure(id, name, UPDATE_SEVERITY_LEVEL_PROCEDURE_NAME, SEVERITY_LEVEL_NAME_PARAMETER_LENGTH);
        }

        /// <summary>
        /// Updates the attributes of an existing event source.
        /// </summary>
        /// <param name="id">The ID of the event source in the database.</param>
        /// <param name="name">The name to give the event source.</param>
        /// <returns>True if the update succeeded, false otherwise.</returns>
        protected bool UpdateSource(int id, string name)
        {
            return ExecuteUpdateByIdAndNameStoredProcedure(id, name, UPDATE_SOURCE_PROCEDURE_NAME, SOURCE_NAME_PARAMETER_LENGTH);
        }
    }
}
