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
    /// http://schema.org/Place
    /// Entities that have a somewhat fixed, physical extension.
    /// </summary>
    [DataContract(Name = "Place", Namespace = "http://schema.org/Place")]
    public class Place : Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----
        
        /// <summary>
        /// Physical address of the item.
        /// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "address")]
        //public PostalAddress Address;

        /// <summary>
        /// The overall rating, based on a collection of reviews or ratings,
        /// of the item.
        /// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "aggregateRating")]
        //public AggregateRating AggregateRating;

        /// <summary>
        /// The basic containment relation between places.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "containedIn")]
        public Place ContainedIn;

        /// <summary>
        /// Upcoming or past event associated with this place, organization,
        /// or action. Supersedes events.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "event")]
        public Event Event;

        /// <summary>
        /// The fax number.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "faxNumber")]
        public string FaxNumber;

        /// <summary>
        /// The Global Location Number (GLN, sometimes also referred
        /// to as International Location Number or ILN) of the
        /// respective organization, person, or place. The GLN is a
        /// 13-digit number used to identify parties and physical
        /// locations.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "globalLocationNumber")]
        public string GlobalLocationNumber;

        /// <summary>
        /// A count of a specific user interactions with this item—for
        /// example, 20 UserLikes, 5 UserComments, or 300 UserDownloads.
        /// The user interaction type should be one of the sub types of
        /// UserInteraction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "interactionCount")]
        public string InteractionCount;

        /// <summary>
        /// The International Standard of Industrial Classification of All
        /// Economic Activities (ISIC), Revision 4 code for a particular
        /// organization, business person, or place.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "isicV4")]
        public string IsicV4;

        /// <summary>
        /// The opening hours of a certain place.
        /// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "openingHoursSpecification")]
        //public OpeningHoursSpecification OpeningHoursSpecification;

        /// <summary>
        /// A review of the item.
        /// Supersedes reviews.
        /// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "review")]
        //public Review Review;

        /// <summary>
        /// The telephone number.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "telephone")]
        public string Telephone;

        // ----- PROPERTIES -----

        /// <summary>
        /// The geo coordinates of the place.
        /// Expected Types: GeoCoordinates or GeoShape.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "geo")]
        public dynamic Geo
        {
            get;
            set;
        }

        /// <summary>
        /// A URL to a map of the place. Supersedes map, maps.
        /// Expected Types: URL or Map.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "hasMap")]
        public dynamic HasMap
        {
            get;
            set;
        }

        /// <summary>
        /// An associated logo.
        /// Expected Types: ImageObject or URL.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "logo")]
        public dynamic Logo
        {
            get;
            set;
        }

        /// <summary>
        /// A photograph of this place.
        /// Supersedes photos.
        /// Expected Types: Photograph or ImageObject
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "photo")]
        public dynamic Photo
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Place()
        {

        }

        /// <summary>
        /// Construct a Place from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the Place.</param>
        public Place(ExpandoObject expando) : base(expando)
        {
        }

        /// <summary>
        /// Returns the item as microdata annotated HTML.
        /// </summary>
        /// <param name="itemprop">The name of the property that this item is the value of in another item. May be null if this item
        /// is not a property of another.</param>
        /// <returns>Returns a string of microdata annotated HTML, or an empty string if the item could not be converted.</returns>
        public virtual string ToMicrodata(string itemprop = null)
        {
            Dictionary<string, object> microdata = GetMicrodata();

            StringBuilder html = new StringBuilder();

            // Write the containing div.
            html.Append("<div itemscope ");
            if (!string.IsNullOrWhiteSpace(itemprop))
            {
                html.Append("itemprop=\"" + itemprop + "\" ");
            }
            html.Append("itemtype=\"" + microdata["ItemTypeUrl"] + "\">\n");

            // Write the place's name.
            if (!string.IsNullOrWhiteSpace(Name))
            {
                html.Append("<h1 itemprop=\"name\">" + Name + "</h1>\n");
            }

            // Write an img tag for the item's associated image.
            if (Image != null)
            {
                if (Image is ImageObject)
                {
                    html.Append((Image as ImageObject).ToMicrodata("image"));
                }
                else
                {
                    html.Append("<img itemprop=\"image\" src=\"" + Image.ToString() + "\" >\n");
                }
            }

            // Write an img tag for the item's associated photo.
            if (Photo != null)
            {
                html.Append(Photo.ToMicrodata("photo"));
            }

            // Write an img tag for the item's associated logo.
            if (Logo != null)
            {
                if (Logo is ImageObject)
                {
                    html.Append((Logo as ImageObject).ToMicrodata("logo"));
                }
                else
                {
                    html.Append("<img itemprop=\"logo\" src=\"" + Logo.ToString() + "\" >\n");
                }
            }

            // Write a description of the item.
            if (!string.IsNullOrWhiteSpace(Description))
            {
                html.Append("Description: <span itemprop=\"description\">" + Description + "</span>\n");
            }

            // Write the person's telephone number.
            if (!string.IsNullOrWhiteSpace(Telephone))
            {
                html.Append("Telephone Number: <span itemprop=\"telephone\">" + Telephone + "</span>\n");
            }

            // Write the person's FAX number.
            if (!string.IsNullOrWhiteSpace(FaxNumber))
            {
                html.Append("FAX Number: <span itemprop=\"faxNumber\">" + FaxNumber + "</span>\n");
            }

            // Close out the containing div.
            html.Append("</div>\n");

            // Return the HTML generated.
            return html.ToString();
        }
    }
}
