using System;
using System.Collections.Generic;
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

        // ----- METHODS -----
    }
}
