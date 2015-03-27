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
        [DataMember]
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
        [DataMember]
        public ImageObject Thumbnail;

        /// <summary>
        /// If this MediaObject is an AudioObject or VideoObject, the transcript of that object.
        /// </summary>
        [DataMember]
        public string Transcript;

        /// <summary>
        /// The frame size of the video.
        /// </summary>
        [DataMember]
        public string VideoFrameSize;

        /// <summary>
        /// The quality of the video.
        /// </summary>
        [DataMember]
        public string VideoQuality;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        [DataMember]
        public override string ItemType { get { return "VideoObject"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        [DataMember]
        public override string ItemTypeDescription { get { return "A video file."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        [DataMember]
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/VideoObject"); } }

        /// <summary>
        /// The composer of the soundtrack.
        /// Expected Types: MusicGroup or Person.
        /// </summary>
        [DataMember]
        public dynamic MusicBy
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
