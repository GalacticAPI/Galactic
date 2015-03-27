using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/MediaObject
    /// An image, video, or audio object embedded in a web page. Note that a creative work
    /// may have many media objects associated with it on the same web page. For example,
    /// a page about a single song (MusicRecording) may have a music video (VideoObject),
    /// and a high and low bandwidth audio stream (2 AudioObject's).
    /// </summary>
    [DataContract(Name = "MediaObject", Namespace = "http://schema.org/MediaObject")]
    public class MediaObject : CreativeWork
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// A NewsArticle associated with the Media Object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "associatedArticle")]
        public NewsArticle AssociatedArticle;

        /// <summary>
        /// The bitrate of the media object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "bitrate")]
        public string Bitrate;

        /// <summary>
        /// File size in (mega/kilo) bytes.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "contentSize")]
        public string ContentSize;

        /// <summary>
        /// Actual bytes of the media object, for example the image file or video file.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "contentUrl")]
        public Uri ContentUrl;

        /// <summary>
        /// The duration of the item (movie, audio recording, event, etc.) in ISO 8601
        /// date format.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "duration")]
        public TimeSpan Duration;

        /// <summary>
        /// A URL pointing to a player for a specific video. In general, this is the
        /// information in the src element of an embed tag and should not be the same as
        /// the content of the loc tag.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "embedUrl")]
        public Uri EmbedUrl;

        /// <summary>
        /// The CreativeWork encoded by this media object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "encodesCreativeWork")]
        public CreativeWork EncodesCreativeWork;

        /// <summary>
        /// mp3, mpeg4, etc.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "encodingFormat")]
        public string EncodingFormat;

        /// <summary>
        /// Date the content expires and is no longer useful or available. Useful for videos.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "expires")]
        public DateTime Expires;

        /// <summary>
        /// Player type required—for example, Flash or Silverlight.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "playerType")]
        public string PlayerType;

        /*
        /// <summary>
        /// The production company or studio responsible for the item e.g. series, video game,
        /// episode etc.
        /// </summary>
        public Organization ProductionCompany;
        */

        /*
        /// <summary>
        /// A publication event associated with the episode, clip or media object.
        /// </summary>
        public PublicationEvent Publication;
        */

        /*
        /// <summary>
        /// The regions where the media is allowed. If not specified, then it's assumed to be
        /// allowed everywhere. Specify the countries in ISO 3166 format.
        /// </summary>
        public Place RegionsAllowed;
        */

        /// <summary>
        /// Indicates if use of the media require a subscription (either paid or free). Allowed
        /// values are true or false (note that an earlier version had 'yes', 'no').
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "requiresSubscription")]
        public bool RequiresSubscription;

        /// <summary>
        /// Date when this media object was uploaded to this site.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "uploadTime")]
        public DateTime UploadTime;
        
        // ----- PROPERTIES -----

        /// <summary>
        /// The height of the item.
        /// Expected Types: Quantitative Value or string.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "height")]
        public dynamic Height
        {
            get;
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "MediaObject"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "An image, video, or audio object embedded in a web page. Note that a creative work may have many media objects associated with it on the same web page. For example, a page about a single song (MusicRecording) may have a music video (VideoObject), and a high and low bandwidth audio stream (2 AudioObject's)."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/MediaObject"); } }

        /// <summary>
        /// The width of the item.
        /// Expected Types: Quantitative Value or string.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "width")]
        public dynamic Width
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
