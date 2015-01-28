using System;

namespace Galactic.Calendar
{
    /// <summary>
    /// Event is a class that provides data about an event on a calendar.
    /// </summary>
    public class Event
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The priority of events on the calendar.
        /// </summary>
        public enum PriorityLevels { High = 1, Medium, Low };

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The event's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The event's location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// A URL providing more information about the event's location.
        /// </summary>
        public Uri LocationUrl { get; set; }

        /// <summary>
        /// Whether the event has been canceled.
        /// </summary>
        public bool? Canceled { get; set; }

        /// <summary>
        /// Indicates that the event does not have an end time specified.
        /// </summary>
        public bool? NoEndTime { get; set; }

        /// <summary>
        /// The priority of the event on the calendar.
        /// </summary>
        public PriorityLevels Priority { get; set; }

        /// <summary>
        /// The date and time the event starts.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date and time the event ends.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Whether the event lasts all day long.
        /// </summary>
        public bool? AllDayEvent { get; set; }

        /// <summary>
        /// The name of the contact person for this event.
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// The e-mail address of the contact person for this event.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// The phone number of the contact person for this event.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Whether the event is reoccurring.
        /// </summary>
        public bool? Reoccurring { get; set; }

        /// <summary>
        /// The date and time that the event was last updated.
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// The name of the last person to update the event.
        /// </summary>
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// The date and time that the event details were last updated.
        /// </summary>
        public DateTime? DetailsLastUpdated { get; set; }

        /// <summary>
        /// The name of the last person to update the event details.
        /// </summary>
        public string DetailsLastUpdatedBy { get; set; }

        /// <summary>
        /// Whether the event is on multiple calendars.
        /// </summary>
        public bool? OnMultipleCalendars { get; set; }

        
        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Event()
        {
            Priority = PriorityLevels.Medium;
        }

        // ---------- METHODS ----------
    }
}
