using System;
using System.Runtime.Serialization;

namespace Galactic.EventLog
{
    /// <summary>
    /// Event contains the data necessary to log events for an application or service.
    /// </summary>
    [DataContract]
    public class Event : IComparable<Event>, IComparable<DateTime>
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The possible severity levels of a logged event.
        /// </summary>
        public enum SeverityLevels
        {
            /// <summary>
            /// Indicates that this event is informational in nature.
            /// </summary>
            Information,

            /// <summary>
            /// Indicates that this event contains a warning.
            /// </summary>
            Warning,

            /// <summary>
            /// Indicates that this event contains an error.
            /// </summary>
            Error,

            /// <summary>
            /// Indicates that this event contains an unknown security level.
            /// </summary>
            Unknown
        };

        // A value for sources that aren't specified.
        private const string UNKNOWN_SOURCE = "Unknown";

        // A value for categories that aren't specified.
        private const string UNKNOWN_CATEGORY = "Unknown";

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// A category that describes the nature of the event.
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// The date and time that the event occurred.
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// A detailed description of the event.
        /// </summary>
        [DataMember]
        public string Details { get; set; }

        /// <summary>
        /// The severity of the event.
        /// </summary>
        [DataMember]
        public SeverityLevels Severity { get; set; }

        /// <summary>
        /// An identifier for the application or service from which the event originated.
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Constructs an Event given its source, date, severity, category, and details about the event.
        /// </summary>
        /// <param name="source">An identifier for the application or service from which the event originated.</param>
        /// <param name="date">The date and time that the event occurred.</param>
        /// <param name="severity">The severity of the event.</param>
        /// <param name="category">A category that describes the nature of the event.</param>
        /// <param name="details">A detailed description of the event.</param>
        public Event(string source, DateTime date, SeverityLevels severity, string category, string details)
        {
            // Check that a source for the event was provided.
            if (!String.IsNullOrWhiteSpace(source))
            {
                // A source was provided, set its value.
                Source = source;
            }
            else
            {
                // No source was provided. Set its value to unknown.
                Source = UNKNOWN_SOURCE;
            }

            // Set the date.
            Date = date;

            // Set the severity.
            Severity = severity;

            // Check that a category for the event was provided.
            if (!string.IsNullOrWhiteSpace(category))
            {
                // A category was provided, set its value.
                Category = category;
            }
            else
            {
                // No category was provided. Set its value to unknown.
                Category = UNKNOWN_CATEGORY;
            }

            // Set the details of the event.
            Details = details;
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Compares this Event to another based upon the date of the event.
        /// </summary>
        /// <param name="other">The other event to compare against.</param>
        /// <returns>Less than zero if this event occurred before the other event.
        /// Zero if the events occurred at the same time.
        /// Greater than zero if this event occured after the other event.</returns>
        public int CompareTo(Event other)
        {
            // Check whether the Event supplied is null.
            if (other == null)
            {
                // It's null. Return that this event occurs after the other event.
                return 1;
            }
            else
            {
                // Compare the date and return the result.
                return Date.CompareTo(other.Date);
            }
        }

        /// <summary>
        /// Compares this Event to a DateTime based upon the date of the event.
        /// </summary>
        /// <param name="date">The DateTime to compare against.</param>
        /// <returns>Less than zero if this event occurred before the date supplied.
        /// Zero if this event occurred at the same time as the date supplied.
        /// Greater than zero if this event occured after the date supplied.</returns>
        public int CompareTo(DateTime date)
        {
            // Compare the date and return the result.
            return Date.CompareTo(date);
        }

        /// <summary>
        /// Gets a string representation of a provided severity level.
        /// </summary>
        /// <param name="severityLevel">The severity level to get a string representation of.</param>
        /// <returns>The string representation of the severity level, or an empty string if unknown.</returns>
        static public string GetSeverityLevelString(SeverityLevels severityLevel)
        {
            switch (severityLevel)
            {
                case SeverityLevels.Error:
                    return "Error";
                case SeverityLevels.Information:
                    return "Information";
                case SeverityLevels.Warning:
                    return "Warning";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the severity level, provided its name.
        /// </summary>
        /// <param name="name">The name of the severity level to get.</param>
        /// <returns>The severity level belonging to the name provided, or SeverityLevels.Unknown if the name does not match a severity level.</returns>
        static public SeverityLevels GetSeverityLevel(string name)
        {
            // Check that a name was provided.
            if (!string.IsNullOrWhiteSpace(name))
            {
                // A name was provided.
                switch (name)
                {
                    case "Error":
                        return SeverityLevels.Error;
                    case "Information":
                        return SeverityLevels.Information;
                    case "Warning":
                        return SeverityLevels.Warning;
                    default:
                        return SeverityLevels.Unknown;
                }
            }
            else
            {
                // No name was provided.
                return SeverityLevels.Unknown;
            }
        }
    }
}
