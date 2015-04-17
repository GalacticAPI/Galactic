using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/CreativeWork
    /// The most generic kind of creative work, including books, movies, photographs,
    /// software programs, etc.
    /// </summary>
    [DataContract(Name = "CreativeWork", Namespace = "http://schema.org/CreativeWork")]
    public class CreativeWork : Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The subject matter of the content.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "about")]
        public Thing About;

        /// <summary>
        /// Indicates that the resource is compatible with the referenced accessibility
        /// API (WebSchemas wiki lists possible values).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "accessibilityAPI")]
        public string AccessibilityAPI;

        /// <summary>
        /// Identifies input methods that are sufficient to fully control the described
        /// resource (WebSchemas wiki lists possible values).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "accessibilityControl")]
        public string AccessibilityControl;

        /// <summary>
        /// Content features of the resource, such as accessible media, alternatives and
        /// supported enhancements for accessibility (WebSchemas wiki lists possible
        /// values).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "accessibilityFeature")]
        public string AccessibilityFeature;

        /// <summary>
        /// A characteristic of the described resource that is physiologically dangerous
        /// to some users. Related to WCAG 2.0 guideline 2.3 (WebSchemas wiki lists
        /// possible values).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "accessibilityHazard")]
        public string AccessibilityHazard;

        /// <summary>
        /// Specifies the Person that is legally accountable for the CreativeWork.
        /// </summary>
        public Person AccountablePerson;

        /*
        /// <summary>
        /// The overall rating, based on a collection of reviews or ratings, of the item.
        /// </summary>
        public AggregateRating AggregateRating;
        */

        /// <summary>
        /// A secondary title of the CreativeWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "alternativeHeadline")]
        public string AlternativeHeadline;

        /*
        /// <summary>
        /// A media object that encodes this CreativeWork. This property is a synonym for
        /// encoding.
        /// </summary>
        public MediaObject AssociatedMedia;
        */

        /*
        /// <summary>
        /// The intended audience of the item, i.e. the group for whom the item was created.
        /// </summary>
        public Audience Audience;
        */

        /*
        /// <summary>
        /// An embedded audio object.
        /// </summary>
        public AudioObject Audio;
        */

        /// <summary>
        /// An award won by this person or for this creative work. Supersedes awards.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "award")]
        public string Award;

        /// <summary>
        /// Fictional person connected with a creative work.
        /// </summary>
        public Person Character;

        /// <summary>
        /// The number of comments this CreativeWork (e.g. Article, Question or Answer) has
        /// received. This is most applicable to works published in Web sites with commenting
        /// system; additional comments may exist elsewhere.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "commentCount")]
        public int CommentCount = 0;

        /// <summary>
        /// The location of the content.
        /// </summary>
        public Place ContentLocation;

        /// <summary>
        /// Official rating of a piece of content—for example,'MPAA PG-13'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "contentRating")]
        public string ContentRating;

        /// <summary>
        /// The year during which the claimed copyright for the CreativeWork was first asserted.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "copyrightYear")]
        public double CopyrightYear;

        /// <summary>
        /// The date on which the CreativeWork was created.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateCreated")]
        public DateTime DateCreated;

        /// <summary>
        /// The date on which the CreativeWork was most recently modified.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "dateModified")]
        public DateTime DateModified;

        /// <summary>
        /// Date of first broadcast/publication.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name= "datePublished")]
        public DateTime DatePublished;

        /// <summary>
        /// A link to the page containing the comments of the CreativeWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discussionUrl")]
        public Uri DiscussionUrl;

        /// <summary>
        /// Specifies the Person who edited the CreativeWork.
        /// </summary>
        public Person Editor;

        /*
        /// <summary>
        /// An alignment to an established educational framework.
        /// </summary>
        public AlignmentObject EducationalAlignment;
        */

        /// <summary>
        /// The purpose of a work in the context of education; for example, 'assignment',
        /// 'group work'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "educationalUse")]
        public string EducationalUse;

        /*
        /// <summary>
        /// A media object that encodes this CreativeWork. This property is a synonym for associatedMedia.
        /// Supersedes encodings.
        /// </summary>
        public MediaObject Encoding;
        */

        /// <summary>
        /// A creative work that this work is an example/instance/realization/derivation of.
        /// Inverse property: workExample.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "exampleOfWork")]
        public CreativeWork ExampleOfWork;

        /// <summary>
        /// Genre of the creative work or group.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "genre")]
        public string Genre;

        /// <summary>
        /// Indicates a CreativeWork that is (in some sense) a part of this CreativeWork.
        /// Inverse property: isPartOf.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "hasPart")]
        public CreativeWork HasPart;

        /// <summary>
        /// Headline of the article.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "headline")]
        public string Headline;

        /// <summary>
        /// The language of the content. please use one of the language codes from the IETF BCP 47 standard.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "inLanguage")]
        public string InLanguage;

        /// <summary>
        /// A count of a specific user interactions with this item—for example, 20 UserLikes, 5 UserComments,
        /// or 300 UserDownloads. The user interaction type should be one of the sub types of UserInteraction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "interactionCount")]
        public string InteractionCount;

        /// <summary>
        /// The predominant mode of learning supported by the learning resource. Acceptable values are 'active',
        /// 'expositive', or 'mixed'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "interactivityText")]
        public string InteractivityText;

        /// <summary>
        /// A resource that was used in the creation of this resource. This term can be repeated for multiple
        /// sources. For example, http://example.com/great-multiplication-intro.html.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "isBasedOnUrl")]
        public Uri IsBasedOnUrl;

        /// <summary>
        /// Indicates whether this content is family friendly.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "isFamilyFriendly")]
        public bool IsFamilyFriendly;

        /// <summary>
        /// Indicates a CreativeWork that this CreativeWork is (in some sense) part of.
        /// Inverse property: hasPart.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "isPartOf")]
        public CreativeWork IsPartOf;

        /// <summary>
        /// Keywords or tags used to describe this content. Multiple entries in a keywords list are typically delimited
        /// by commas.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "keywords")]
        public string Keywords;

        /// <summary>
        /// The predominant type or kind characterizing the learning resource. For example, 'presentation', 'handout'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "learningResourceType")]
        public string LearningResourceType;

        /// <summary>
        /// Indicates that the CreativeWork contains a reference to, but is not necessarily about a concept.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "mentions")]
        public Thing Mentions;

        /*
        /// <summary>
        /// An offer to provide this item—for example, an offer to sell a product, rent the DVD of a movie, or give away tickets
        /// to an event.
        /// </summary>
        public Offer Offers;
        */

        /*
        /// <summary>
        /// The publisher of the creative work.
        /// </summary>
        public Organization Publisher;
        */

        /// <summary>
        /// Link to page describing the editorial principles of the organization primarily responsible for the creation of the CreativeWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "publishingPrinciples")]
        public Uri PublishingPrinciples;

        /*
        /// <summary>
        /// The Event where the CreativeWork was recorded. The CreativeWork may capture all or part of the event.
        /// Inverse property: recordedIn.
        /// </summary>
        public Event RecordedAt;
        */

        /*
        /// <summary>
        /// The place and time the release was issued, expressed as a PublicationEvent.
        /// </summary>
        public PublicationEvent ReleasedEvent;
        */

        /*
        /// <summary>
        /// A review of the item. Supersedes reviews.
        /// </summary>
        public Review Review;
        */

        /*
        /// <summary>
        /// The Organization on whose behalf the creator was working.
        /// </summary>
        public Organization SourceOrganization;
        */

        /// <summary>
        /// The textual content of this CreativeWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "text")]
        public string Text;

        /// <summary>
        /// A thumbnail image relevant to the Thing.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnailUrl")]
        public Uri ThumbnailUrl;

        /// <summary>
        /// Approximate or typical time it takes to work with or through this learning resource for the typical
        /// intended target audience, e.g. 'P30M', 'P1H25M'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "timeRequired")]
        public TimeSpan TimeRequired;

        /// <summary>
        /// The typical expected age range, e.g. '7-9', '11-'.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "typicalAgeRange")]
        public string TypicalAgeRange;

        /// <summary>
        /// The version of the CreativeWork embodied by a specified resource.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "version")]
        public double Version;

        /*
        /// <summary>
        /// An embedded video object.
        /// </summary>
        public VideoObject Video;
        */

        /// <summary>
        /// Example/instance/realization/derivation of the concept of this creative work. eg. The paperback edition, first edition, or eBook.
        /// Inverse property: exampleOfWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "workExample")]
        public CreativeWork WorkExample;

        // ----- PROPERTIES -----

        /// <summary>
        /// The author of this content. Please note that author is special in that HTML 5
        /// provides a special mechanism for indicating authorship via the rel tag. That is
        /// equivalent to this and may be used interchangeably.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "author")]
        public dynamic Author
        {
            get; 
            set;
        }

        /// <summary>
        /// A citation or reference to another creative work, such as another publication,
        /// web page, scholarly article, etc.
        /// Expected Types: string or CreativeWork.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "citation")]
        public dynamic Citation
        {
            get; 
            set;
        }


        /// <summary>
        /// Comments, typically from users, on this CreativeWork.
        /// Expected Types: Comment or UserComments
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "comment")]
        public dynamic Comment
        {
            get;
            set;
        }

        /// <summary>
        /// A secondary contributor to the CreativeWork.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "contributor")]
        public dynamic Contributor
        {
            get;
            set;
        }

        /// <summary>
        /// The party holding the legal copyright to the CreativeWork.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "copyrightHolder")]
        public dynamic CopyrightHolder
        {
            get;
            set;
        }

        /// <summary>
        /// The creator/author of this CreativeWork or UserComments. This is the same as the
        /// Author property for CreativeWork.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "creator")]
        public dynamic Creator
        {
            get;
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "CreativeWork"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "The most generic kind of creative work, including books, movies, photographs, software programs, etc."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/CreativeWork"); } }

        /// <summary>
        /// A license document that applies to this content, typically indicated by URL.
        /// Expected Types: CreativeWork or Uri.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "license")]
        public dynamic License
        {
            get;
            set;
        }

        /// <summary>
        /// The position of an item in a series or sequence of items.
        /// Expected Types: string or int.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "position")]
        public dynamic Position
        {
            get;
            set;
        }

        /// <summary>
        /// The person or organization who produced the work (e.g. music album, movie, tv/radio
        /// series etc.).
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "producer")]
        public dynamic Producer
        {
            get;
            set;
        }

        /// <summary>
        /// The service provider, service operator, or service performer; the goods producer.
        /// Another party (a seller) may offer those services or goods on behalf of the provider.
        /// A provider may also serve as the seller. Supersedes carrier.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider")]
        public dynamic Provider
        {
            get;
            set;
        }

        /// <summary>
        /// Organization or person who adapts a creative work to different languages, regional
        /// differences and technical requirements of a target market.
        /// Expected Types: Person or Organization.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "translator")]
        public dynamic Translator
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CreativeWork()
        {

        }

        /// <summary>
        /// Construct a CreativeWork from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the CreativeWork.</param>
        public CreativeWork (ExpandoObject expando) : base(expando)
        {
        }

        // ----- METHODS -----
    }
}
