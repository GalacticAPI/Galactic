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
    /// http://schema.org/Article
    /// An article, such as a news article or piece of investigative report.
    /// Newspapers and magazines have articles of many different types and this is
    /// intended to cover them all.
    /// </summary>
    [DataContract(Name = "Article", Namespace = "http://schema.org/Article")]
    public class Article : CreativeWork
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The actual body of the article.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "articleBody")]
        public string ArticleBody;

        /// <summary>
        /// Articles may belong to one or more 'sections' in a magazine or newspaper,
        /// such as Sports, Lifestyle, etc.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "articleSection")]
        public string ArticleSection;

        /// <summary>
        /// Any description of pages that is not separated into pageStart and pageEnd; for example,
        /// "1-6, 9, 55" or "10-12, 46-49".
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "pagnation")]
        public string Pagnation;

        /// <summary>
        /// The number of words in the text of the Article.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "wordCount")]
        public int WordCount;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "Article"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "An article, such as a news article or piece of investigative report. Newspapers and magazines have articles of many different types and this is intended to cover them all."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/Article"); } }

        /// <summary>
        /// The page on which the work ends; for example "138" or "xvi".
        /// Expected Types: int or string.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageEnd")]
        public dynamic PageEnd
        {
            get; 
            set;
        }

        /// <summary>
        /// The page on which the work starts; for example "135" or "xiii".
        /// Expected Types: int or string.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageStart")]
        public dynamic PageStart
        {
            get;
            set;
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Article()
        {

        }

        /// <summary>
        /// Construct an Article from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the Article.</param>
        public Article (ExpandoObject expando) : base(expando)
        {
        }

        // ----- METHODS -----
    }
}
