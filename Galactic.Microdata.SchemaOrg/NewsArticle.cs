using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/NewsArticle
    /// A utility class that serves as the umbrella for a number of 'intangible'
    /// things such as quantities, structured values, etc.
    /// </summary>
    [DataContract(Name = "NewsArticle", Namespace = "http://schema.org/NewsArticle")]
    public class NewsArticle : Article
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The location where the NewsArticle was produced.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "dateline")]
        public string Dateline;

        /// <summary>
        /// The number of the column in which the NewsArticle appears in the print edition.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "printColumn")]
        public string PrintColumn;

        /// <summary>
        /// The edition of the print product in which the NewsArticle appears.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "printEdition")]
        public string PrintEdition;

        /// <summary>
        /// If this NewsArticle appears in print, this field indicates the name of the page on
        /// which the article is found. Please note that this field is intended for the exact
        /// page name (e.g. A5, B18).
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "printPage")]
        public string PrintPage;

        /// <summary>
        /// If this NewsArticle appears in print, this field indicates the print section in
        /// which the article appeared.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "printSection")]
        public string PrintSection;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public override string ItemType { get { return "NewsArticle"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public override string ItemTypeDescription { get { return "A news article."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public override Uri ItemTypeUrl { get { return new Uri("http://schema.org/NewsArticle"); } }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----
    }
}
