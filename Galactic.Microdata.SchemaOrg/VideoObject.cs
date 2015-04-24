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
    /// http://schema.org/VideoObject
    /// A video file.
    /// </summary>
    [DataContract(Name = "VideoObject", Namespace = "http://schema.org/VideoObject")]
    public class VideoObject : MediaObject
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// An actor, e.g. in tv, radio, movie, video games etc. Actors can be associated
        /// with individual items or with a series, episode, clip. Supersedes actors.
        /// </summary>
        public Person Actor;

        /// <summary>
        /// The caption for this object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "caption")]
        public string Caption;

        /// <summary>
        /// A director of e.g. tv, radio, movie, video games etc. content. Directors can be
        /// associated with individual items or with a series, episode, clip. Supersedes directors.
        /// </summary>
        public Person Director;

        /// <summary>
        /// Thumbnail image for an image or video.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "thumbnail")]
        public ImageObject Thumbnail;

        /// <summary>
        /// If this MediaObject is an AudioObject or VideoObject, the transcript of that object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "transcript")]
        public string Transcript;

        /// <summary>
        /// The frame size of the video.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "videoFrameSize")]
        public string VideoFrameSize;

        /// <summary>
        /// The quality of the video.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "videoQuality")]
        public string VideoQuality;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "VideoObject"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "A video file."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/VideoObject"); } }

        /// <summary>
        /// The composer of the soundtrack.
        /// Expected Types: MusicGroup or Person.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "musicBy")]
        public dynamic MusicBy
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VideoObject()
        {

        }

        /// <summary>
        /// Construct a VideoObject from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the VideoObject.</param>
        public VideoObject (ExpandoObject expando) : base(expando)
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

            // Write the object's name.
            if (!string.IsNullOrWhiteSpace(Name))
            {
                html.Append("<h2 itemprop=\"name\">" + Name + "</h2>\n");
            }

            // Write the video's thumbnail.
            if (Thumbnail != null)
            {
                if (Thumbnail.ContentUrl != null)
                {
                    html.Append("<meta itemprop=\"thumbnail\" content=\"" + Thumbnail.ContentUrl + "\" />");
                }
            }

            // Write the video object's video tag.
            if (EmbedUrl != null)
            {
                html.Append("<video src=\"" + ContentUrl.ToString() + "\"");

                if (Image != null)
                {
                    if (Image is ImageObject)
                    {
                        html.Append("poster=\"" + (Image as ImageObject).ContentUrl.ToString() + "\" ");
                    }
                    else
                    {
                        html.Append("poster=\"" + (Image as Uri).ToString() + "\" ");
                    }
                }
                if (Width != null)
                {
                    html.Append("width=\"" + Width.ToString() + "\" ");
                }
                if (Height != null)
                {
                    html.Append("height=\"" + Height.ToString() + "\" ");
                }
                html.Append("controls >\n");
            }

            // Write the duration of the video.
            if (Duration.Ticks > 0)
            {
                html.Append("<meta itemprop=\"duration\" content=\"P" + Duration.TotalHours + "H" + Duration.Minutes + "M" + Duration.Seconds + "S\" />");
            }

            // Write a description of the object.
            if (!string.IsNullOrWhiteSpace(Description))
            {
                html.Append("Description: <span itemprop=\"description\">" + Description + "</span>\n");
            }

            // Write a transcript of the video.
            if (!string.IsNullOrWhiteSpace(Transcript))
            {
                html.Append("Transcript: <span itemprop=\"transcript\">" + Transcript + "</span>\n");
            }

            // The director of the video.
            if (Director != null)
            {
                html.Append("Directed by: ");
                html.Append(MusicBy.ToMicrodata("director"));
            }

            // The group or person responsible for the video's music.
            if (MusicBy != null)
            {
                html.Append("Music by: ");
                html.Append(MusicBy.ToMicrodata("musicBy"));
            }

            // An actor in the video.
            if (Actor != null)
            {
                html.Append("Acted by: ");
                html.Append(MusicBy.ToMicrodata("actor"));
            }

            // Close out the containing div.
            html.Append("</div>\n");

            // Return the HTML generated.
            return html.ToString();
        }
    }
}
