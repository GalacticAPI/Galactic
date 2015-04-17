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
        public override string ItemType { get { return "ActionStatusType"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "The status of an Action."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/ActionStatusType"); } }

        /// <summary>
        /// A description of an action that is supported.
        /// </summary>
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionStatusType()
        {

        }

        /// <summary>
        /// Construct a ActionStatusType from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the ActionStatusType.</param>
        public ActionStatusType (ExpandoObject expando) : base(expando)
        {
        }
    }
}
