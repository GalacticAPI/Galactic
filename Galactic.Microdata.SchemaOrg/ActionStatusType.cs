using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/ActionStatusType
    /// The status of an Action.
    /// </summary>
    [DataContract(Name = "ActionStatusType", Namespace = "http://schema.org/ActionStatusType")]
    public class ActionStatusType : Intangible
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES ----- 

        /// <summary>
        /// An in-progress action (e.g, while watching the movie, or driving to a location).
        /// </summary>
        [DataMember]
        static public ActionStatusType ActiveActionStatus
        {
            get
            {
                return new ActionStatusType()
                {
                    Name = "ActiveActionStatus",
                    Description = "An in-progress action (e.g, while watching the movie, or driving to a location).",
                    Url = new Uri("http://schema.org/ActiveActionStatus")
                };

            }
        }

        /// <summary>
        /// An action that has already taken place.
        /// </summary>
        [DataMember]
        static public ActionStatusType CompletedActionStatus
        {
            get
            {
                return new ActionStatusType()
                {
                    Name = "CompletedActionStatus",
                    Description = "An action that has already taken place.",
                    Url = new Uri("http://schema.org/CompletedActionStatus")
                };
            }
        }

        /// <summary>
        /// An action that failed to complete. The action's error property and the HTTP
        /// return code contain more information about the failure.
        /// </summary>
        [DataMember]
        static public ActionStatusType FailedActionStatus
        {
            get
            {
                return new ActionStatusType()
                {
                    Name = "FailedActionStatus",
                    Description = "An action that failed to complete. The action's error property and the HTTP return code contain more information about the failure.",
                    Url = new Uri("http://schema.org/FailedActionStatus")
                };
            }
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        [DataMember]
        public override string ItemType { get { return "ActionStatusType"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        [DataMember]
        public override string ItemTypeDescription { get { return "The status of an Action."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        [DataMember]
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/ActionStatusType"); } }

        /// <summary>
        /// A description of an action that is supported.
        /// </summary>
        [DataMember]
        static public ActionStatusType PotentialActionStatus
        {
            get
            {
                return new ActionStatusType()
                {
                    Name = "PotentialActionStatus",
                    Description = "A description of an action that is supported.",
                    Url = new Uri("http://schema.org/PotentialActionStatus")
                };
            }
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
