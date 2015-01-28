using System;

namespace Galactic.EventLog
{
    /// <summary>
    /// An interface for classes that whish log their exceptions to an event log.
    /// </summary>
    public interface IExceptionLogger
    {
        // ---------- PROPERTIES ----------

        /// <summary>
        /// The event log that will receive events from this logger.
        /// </summary>
        EventLog Log { get; set; }

        // ---------- METHODS ----------

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        bool LogException(Exception e);

    }
}
