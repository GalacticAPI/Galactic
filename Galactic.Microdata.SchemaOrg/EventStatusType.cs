using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/EventStatusType
    /// EventStatusType is an enumeration type whose instances represent several states
    /// that an Event may be in.
    /// </summary>
    [DataContract(Name = "EventStatusType", Namespace = "http://schema.org/EventStatusType")]
    public class EventStatusType : Intangible
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES ----- 

        /// <summary>
        /// An in-progress action (e.g, while watching the movie, or driving to a location).
        /// </summary>
        static public EventStatusType EventCancelled
        {
            get
            {
                return new EventStatusType()
                {
                    Name = "EventCancelled",
                    Description = "The event has been cancelled. If the event has multiple startDate values, all are assumed to be cancelled. Either startDate or previousStartDate may be used to specify the event's cancelled date(s).",
                    Url = new Uri("http://schema.org/EventCancelled")
                };

            }
        }

        /// <summary>
        /// The event has been postponed and no new date has been set.
        /// The event's previousStartDate should be set.
        /// </summary>
        static public EventStatusType EventPostponed
        {
            get
            {
                return new EventStatusType()
                {
                    Name = "EventPostponed",
                    Description = "The event has been postponed and no new date has been set. The event's previousStartDate should be set.",
                    Url = new Uri("http://schema.org/EventPostponed")
                };
            }
        }

        /// <summary>
        /// The event has been rescheduled. The event's previousStartDate should be set
        /// to the old date and the startDate should be set to the event's new date.
        /// (If the event has been rescheduled multiple times, the previousStartDate property
        /// may be repeated).
        /// </summary>
        static public EventStatusType EventRescheduled
        {
            get
            {
                return new EventStatusType()
                {
                    Name = "EventRescheduled",
                    Description = "The event has been rescheduled. The event's previousStartDate should be set to the old date and the startDate should be set to the event's new date. (If the event has been rescheduled multiple times, the previousStartDate property may be repeated).",
                    Url = new Uri("http://schema.org/EventRescheduled")
                };
            }
        }

        /// <summary>
        /// The event is taking place or has taken place on the startDate as scheduled.
        /// Use of this value is optional, as it is assumed by default.
        /// </summary>
        static public EventStatusType EventScheduled
        {
            get
            {
                return new EventStatusType()
                {
                    Name = "EventScheduled",
                    Description = "The event is taking place or has taken place on the startDate as scheduled. Use of this value is optional, as it is assumed by default.",
                    Url = new Uri("http://schema.org/EventScheduled")
                };
            }
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "EventStatusType"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "EventStatusType is an enumeration type whose instances represent several states that an Event may be in."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/EventStatusType"); } }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EventStatusType()
        {

        }

        /// <summary>
        /// Construct a EventStatusType from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the EventStatusType.</param>
        public EventStatusType (ExpandoObject expando) : base(expando)
        {
        }

        // ----- METHODS -----
    }
}
