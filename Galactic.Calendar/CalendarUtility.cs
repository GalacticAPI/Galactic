using System;
using System.Collections.Generic;

namespace Galactic.Calendar
{
    /// <summary>
    /// Provides various utility methods for use with Calendars.
    /// </summary>
    public abstract class CalendarUtility
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// A list of the names of calendars available via the calendar provider.
        /// </summary>
        public abstract List<string> CalendarNames
        {
            get;
        }

        // ---------- CONSTRUCTORS ----------

        // ---------- METHODS ----------

        /// <summary>
        /// Gets a calendar from the provider.
        /// </summary>
        /// <param name="name">The name of the calendar to retrieve.</param>
        /// <param name="startDate">Events starting on or after this date will be included in the list returned.</param>
        /// <param name="endDate">Events starting on or before this date will be included in the list returned.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if an name that is empty, null,
        /// or full of whitespace is provided.</exception>
        /// <returns>The calendar with the supplied name, or null if a calendar of that name does not exist
        /// or could not be retrieved.</returns>
        public abstract Calendar GetCalendar(string name, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Saves a calendar to the provider.
        /// </summary>
        /// <param name="calendar">The calendar object to save.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a calendar is not provided.</exception>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public abstract bool SaveCalendar(Calendar calendar);
    }
}
