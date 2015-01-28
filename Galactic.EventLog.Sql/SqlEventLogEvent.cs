using System;

namespace Galactic.EventLog.Sql
{
    /// <summary>
    /// SqlEventLogEvent is a lightweight data class containing properties that describe events in an SqlEventLog database.
    /// </summary>
    public class SqlEventLogEvent
    {
        /// <summary>
        /// The date of the event.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The source of this event in the database.
        /// </summary>
        public SqlEventLogNamedItem Source { get; set; }

        /// <summary>
        /// The category this event belongs to in the database.
        /// </summary>
        public SqlEventLogNamedItem Category { get; set; }

        /// <summary>
        /// The severity level this event is tagged as in the database.
        /// </summary>
        public SqlEventLogNamedItem SeverityLevel { get; set; }

        /// <summary>
        /// The details text of this event.
        /// </summary>
        public string Details { get; set; }
    }
}
