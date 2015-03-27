using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/Event
    /// An event happening at a certain time and location, such as a concert, lecture,
    /// or festival. Ticketing information may be added via the 'offers' property.
    /// Repeated events may be structured as separate Event objects.
    /// </summary>
    [DataContract(Name = "Event", Namespace = "http://schema.org/Event")]
    class Event : Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The time admission will commence.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "doorTime")]
        public DateTime DoorTime;

        /// <summary>
        /// The duration of the item (movie, audio recording, event, etc.) in ISO 8601
        /// date format.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "duration")]
        public TimeSpan Duration;

        /// <summary>
        /// The end date and time of the item (in ISO 8601 date format).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "endDate")]
        public DateTime EndDate;

        /// <summary>
        /// An eventStatus of an event represents its status; particularly useful when an
        /// event is cancelled or rescheduled.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "eventStatus")]
        public EventStatusType EventStatus;

        /// <summary>
        /// An offer to provide this item—for example, an offer to sell a product, rent the
        /// DVD of a movie, or give away tickets to an event.
        /// </summary>
        //public Offer Offers;

        /// <summary>
        /// Used in conjunction with eventStatus for rescheduled or cancelled events. This
        /// property contains the previously scheduled start date. For rescheduled events,
        /// the startDate property should be used for the newly scheduled start date. In
        /// the (rare) case of an event that has been postponed and rescheduled multiple
        /// times, this field may be repeated.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "previousStartDate")]
        public DateTime PreviousStartDate;

        /// <summary>
        /// The CreativeWork that captured all or part of this Event.
        /// Inverse property: recordedAt.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "recordedIn")]
        public CreativeWork RecordedIn;

        /// <summary>
        /// The start date and time of the item (in ISO 8601 date format).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "startDate")]
        public DateTime StartDate;

        /// <summary>
        /// An Event that is part of this event. For example, a conference event includes
        /// many presentations, each of which is a subEvent of the conference.
        /// Supersedes subEvents.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "subEvent")]
        public Event SubEvent;

        /// <summary>
        /// An event that this event is a part of. For example, a collection of individual
        /// music performances might each have a music festival as their superEvent.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "superEvent")]
        public Event SuperEvent;

        /// <summary>
        /// The typical expected age range, e.g. '7-9', '11-'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "typicalAgeRange")]
        public string TypicalAgeRange;

        /// <summary>
        /// A work performed in some event, for example a play performed in a TheaterEvent.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "workPerformed")]
        public CreativeWork WorkPerformed;

        // ----- PROPERTIES -----

        /// <summary>
        /// A person or organization attending the event. Supersedes attendees.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "attendee")]
        public dynamic Attendee
        {
            get;
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "Event"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "An event happening at a certain time and location, such as a concert, lecture, or festival. Ticketing information may be added via the 'offers' property. Repeated events may be structured as separate Event objects."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/Event"); } }

        /// <summary>
        /// The location of the event, organization or action.
        /// Expected Types: Place or Postal Address.
        /// </summary>
        //public dynamic Location;

        /// <summary>
        /// An organizer of an Event.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "organizer")]
        public dynamic Organizer
        {
            get;
            set;
        }

        /// <summary>
        /// A performer at the event—for example, a presenter, musician, musical group or actor.
        /// Supersedes performers.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "performer")]
        public dynamic Performer
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
