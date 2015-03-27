using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/Action
    /// An action performed by a direct agent and indirect participants upon a direct object.
    /// Optionally happens at a location with the help of an inanimate instrument. The
    /// execution of the action may produce a result. Specific action sub-type documentation
    /// specifies the exact expectation of each argument/role. 
    /// </summary>
    [DataContract(Name = "Action", Namespace = "http://schema.org/Action")]
    public class Action : Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// Indicates the current disposition of the Action.
        /// </summary>
        [DataMember]
        public ActionStatusType ActionStatus;

        /// <summary>
        /// The endTime of something. For a reserved event or service (e.g.
        /// FoodEstablishmentReservation), the time that it is expected to end. For actions
        /// that span a period of time, when the action was performed. e.g. John wrote a book
        /// from January to *December*. Note that Event uses startDate/endDate instead of
        /// startTime/endTime, even when describing dates with times. This situation may be
        /// clarified in future revisions.
        /// </summary>
        [DataMember]
        public DateTime EndTime;

        /// <summary>
        /// For failed actions, more information on the cause of the failure.
        /// </summary>
        [DataMember]
        public Thing Error;

        /// <summary>
        /// The object that helped the agent perform the action. e.g. John wrote a book with
        /// *a pen*.
        /// </summary>
        [DataMember]
        public Thing Instrument;

        /// <summary>
        /// The object upon the action is carried out, whose state is kept intact or changed.
        /// Also known as the semantic roles patient, affected or undergoer (which change
        /// their state) or theme (which doesn't). e.g. John read *a book*.
        /// </summary>
        [DataMember]
        public Thing Object;

        /// <summary>
        /// The result produced in the action. e.g. John wrote *a book*.
        /// </summary>
        [DataMember]
        public Thing Result;

        /// <summary>
        /// The startTime of something. For a reserved event or service (e.g.
        /// FoodEstablishmentReservation), the time that it is expected to start. For actions
        /// that span a period of time, when the action was performed. e.g. John wrote a book
        /// from *January* to December. Note that Event uses startDate/endDate instead of
        /// startTime/endTime, even when describing dates with times. This situation may be
        /// clarified in future revisions.
        /// </summary>
        [DataMember]
        public DateTime StartTime;
        
        /*
        /// <summary>
        /// Indicates a target EntryPoint for an Action.
        /// </summary>
        public EntryPoint Target;
        */
        
        // ----- PROPERTIES -----

        /// <summary>
        /// The direct performer or driver of the action (animate or inanimate). e.g.
        /// *John* wrote a book.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember]
        public dynamic Agent
        {
            get;
            set;
        }

        /// <summary>
        /// The location of the event, organization or action.
        /// Expected Types: Place or PostalAddress.
        /// </summary>
        [DataMember]
        public dynamic Location
        {
            get;
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        [DataMember]
        public override string ItemType { get { return "Action"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        [DataMember]
        public override string ItemTypeDescription { get { return "An action performed by a direct agent and indirect participants upon a direct object. Optionally happens at a location with the help of an inanimate instrument. The execution of the action may produce a result. Specific action sub-type documentation specifies the exact expectation of each argument/role."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        [DataMember]
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/Action"); } }

        /// <summary>
        /// Other co-agents that participated in the action indirectly. e.g. John wrote
        /// a book with *Steve*.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember]
        public dynamic Participant
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
