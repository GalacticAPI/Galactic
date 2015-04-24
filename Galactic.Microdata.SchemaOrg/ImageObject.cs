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
    /// http://schema.org/ImageObject
    /// An image file.
    /// </summary>
    [DataContract(Name = "ImageObject", Namespace = "http://schema.org/ImageObject")]
    public class ImageObject : MediaObject
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The caption for this object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "caption")]
        public string Caption;

        /// <summary>
        /// exif data for this object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "exifData")]
        public string ExifData;

        /// <summary>
        /// Indicates whether this image is representative of the content of the page.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "representativeOfPage")]
        public bool RepresentativeOfPage;

        /// <summary>
        /// Thumbnail image for an image or video.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "thumbnail")]
        public ImageObject Thumbnail;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "ImageObject"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "An image file."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/ImageObject"); } }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImageObject()
        {

        }

        /// <summary>
        /// Construct an ImageObject from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the ImageObject.</param>
        public ImageObject (ExpandoObject expando) : base(expando)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Returns the item as microdata annotated HTML.
        /// </summary>
        /// <param name="itemprop">The name of the property that this item is the value of in another item. May be null if this item
        /// is not a property of another.</param>
        /// <returns>Returns a string of microdata annotated HTML, or an empty string if the item could not be converted.</returns>
        public override string ToMicrodata(string itemprop = null)
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

            // Write the image's name.
            if (!string.IsNullOrWhiteSpace(Name))
            {
                html.Append("<h2 itemprop=\"name\">" + Name + "</h2>\n");
            }
            
            // Write the an image tag for the image.
            if (ContentUrl != null)
            {
                html.Append("<img itemprop=\"contentUrl\" src=\"" + ContentUrl.ToString() + "\" ");
                if (!string.IsNullOrWhiteSpace(Caption))
                {
                    html.Append("alt=\"" + Caption + "\" ");
                }

                if (Width != null)
                {
                    html.Append("width=\"" + Width.ToString() + "\" ");
                }
                if (Height != null)
                {
                    html.Append("height=\"" + Height.ToString() + "\" ");
                }
                html.Append(">\n");
            }
            
            // Write a description of the image.
            if (!string.IsNullOrWhiteSpace(Description))
            {
                html.Append("Description: <span itemprop=\"description\">" + Description + "</span>\n");
            }

            // Close out the containing div.
            html.Append("</div>\n");

            // Return the HTML generated.
            return html.ToString();
        }
    }
}
