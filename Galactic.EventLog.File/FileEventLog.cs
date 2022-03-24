using System;
using System.Collections.Generic;

namespace Galactic.EventLog.File
{
    public class FileEventLog : EventLog
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // Path to the directory where logs will be written.
        string logPath;

        // The name of the log file. 
        string logName;

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates new file event log.
        /// </summary>
        /// <param name="logPath">The path to the log file directory. If no path is specified, logs will be created in current working directory.</param>
        public FileEventLog(string logName, string logPath = "")
        {
            // Check that we retrieved a log path.
            if (string.IsNullOrWhiteSpace(logPath))
            {
                // We did not retrieve a path.
                // Throw an exception.
                throw new ArgumentNullException("logPath");
            }

            this.logPath = logPath;
        }

        // ---------- METHODS ----------

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
                    return WriteLogEvent(eventToLog.Source, eventToLog.Category, eventToLog.Severity, eventToLog.Details);
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
            return new();
        }

        /// <summary>
        /// Write event to log file.
        /// </summary>
        /// <param name="source">A name for the source of the event.</param>
        /// <param name="category">A category that classifies the type of event.</param>
        /// <param name="severityLevel">The importance of the event.</param>
        /// <param name="details">A detailed description of the event.</param>
        /// <returns>True if the event was added, false otherwise.</returns>
        public bool WriteLogEvent(string source, string category, Event.SeverityLevels severityLevel, string details)
        {
            // Check that all parameters were provided.
            if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(details))
            {
                return true;
            }
            else
            {
                // One or more parameters were not provided.
                return false;
            }
        }
    }
}