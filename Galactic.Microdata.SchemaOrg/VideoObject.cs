using System;
using System.Collections.Generic;
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

        /*
        /// <summary>
        /// An actor, e.g. in tv, radio, movie, video games etc. Actors can be associated
        /// with individual items or with a series, episode, clip. Supersedes actors.
        /// </summary>
        public Person Actor;
        */

        /// <summary>
        /// The caption for this object.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "caption")]
        public string Caption;

        /*
        /// <summary>
        /// A director of e.g. tv, radio, movie, video games etc. content. Directors can be
        /// associated with individual items or with a series, episode, clip. Supersedes directors.
        /// </summary>
        public Person Director;
        */

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

        // ----- METHODS -----
    }
}
