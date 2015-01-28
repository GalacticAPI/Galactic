using System.Collections.Generic;

namespace Galactic.Calendar
{
    /// <summary>
    /// Calendar is a class that contains data about a calendar and its events.
    /// </summary>
    public class Calendar
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The name of the calendar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the calendar.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The list of events associated with the calendar.
        /// </summary>
        public List<Event> Events { get; set; }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Calendar()
        {
            Events = new List<Event>();
        }

        // ---------- METHODS ----------
    }
}
