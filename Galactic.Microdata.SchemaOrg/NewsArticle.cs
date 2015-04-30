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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NewsArticle()
        {

        }

        /// <summary>
        /// Construct a NewsArticle from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the NewsArticle.</param>
        public NewsArticle (ExpandoObject expando) : base(expando)
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

            // Write the article's headline.
            if (!string.IsNullOrWhiteSpace(Headline))
            {
                html.Append("<h1 itemprop=\"headline\">" + Headline + "</h1>\n");
            }

            // Write an img tag for the article's associated image.
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

            // Write a description of the article.
            if (!string.IsNullOrWhiteSpace(Description))
            {
                html.Append("Description: <span itemprop=\"description\">" + Description + "</span>\n");
            }

            // Write the author of the work.
            if (Author != null)
            {
                html.Append("Authored by: ");
                html.Append(Author.ToMicrodata("author"));
            }

            // Write the creator of the work.
            if (Creator != null)
            {
                html.Append("Created by: ");
                html.Append(Creator.ToMicrodata("creator"));
            }

            // Write the editor of the work.
            if (Editor != null)
            {
                html.Append("Edited by: ");
                html.Append(Editor.ToMicrodata("editor"));
            }

            // Write the producer of the work.
            if (Producer != null)
            {
                html.Append("Produced by: ");
                html.Append(Producer.ToMicrodata("producer"));
            }

            // Write the provider of the work.
            if (Provider != null)
            {
                html.Append("Provided by: ");
                html.Append(Provider.ToMicrodata("provider"));
            }

            // Write the translator of the work.
            if (Translator != null)
            {
                html.Append("Translated by: ");
                html.Append(Translator.ToMicrodata("translator"));
            }

            // Write the date the work was created.
            if (DateCreated.Ticks > 0)
            {
                html.Append("Created on: <meta itemprop=\"dateCreated\" content=\"" + DateCreated.ToString("yyyy-MM-dd") + "\">" + DateCreated.ToString("MMMM d, yyyy") + "\n");
            }

            // Write the date the work was last modified.
            if (DateModified.Ticks > 0)
            {
                html.Append("Modified on: <meta itemprop=\"dateModified\" content=\"" + DateModified.ToString("yyyy-MM-dd") + "\">" + DateModified.ToString("MMMM d, yyyy") + "\n");
            }

            // Write the date the work was published.
            if (DatePublished.Ticks > 0)
            {
                html.Append("Published on: <meta itemprop=\"datePublished\" content=\"" + DatePublished.ToString("yyyy-MM-dd") + "\">" + DatePublished.ToString("MMMM d, yyyy") + "\n");
            }

            // Write the section the article is associated with.
            if (!string.IsNullOrWhiteSpace(ArticleSection))
            {
                html.Append("<meta itemprop=\"section\" content=\"" + ArticleSection + "\" >\n");
            }

            // Write the word count of the article.
            if (WordCount > 0)
            {
                html.Append("<meta itemprop=\"wordCount\" content=\"" + WordCount + "\" >\n");
            }

            // Write the section in the paper or magazine that the article is associated with.
            if (!string.IsNullOrWhiteSpace(PrintSection))
            {
                html.Append("<meta itemprop=\"printSection\" content=\"" + PrintSection + "\" >\n");
            }

            // Write the edition of the paper or magazine that the article is published in.
            if (!string.IsNullOrWhiteSpace(PrintEdition))
            {
                html.Append("<meta itemprop=\"printEdition\" content=\"" + PrintEdition + "\" >\n");
            }

            // Write the page number in the paper or magazine that the article is printed in.
            if (!string.IsNullOrWhiteSpace(PrintPage))
            {
                html.Append("<meta itemprop=\"printPage\" content=\"" + PrintPage + "\" >\n");
            }

            // Write the number of the column in the paper or magazine that the article is printed in.
            if (!string.IsNullOrWhiteSpace(PrintColumn))
            {
                html.Append("<meta itemprop=\"printColumn\" content=\"" + PrintColumn + "\" >\n");
            }

            // Write the article's dateline.
            if (!string.IsNullOrWhiteSpace(Dateline))
            {
                html.Append("Dateline: <span itemprop=\"dateline\">" + Dateline + "</span>\n");
            }

            // Write the article's body text.
            if (!string.IsNullOrWhiteSpace(ArticleBody))
            {
                html.Append("<div itemprop=\"articleBody\">" + ArticleBody + "</div>\n");
            }

            // Write a copyright notice for the work.
            if (CopyrightYear > 0)
            {
                html.Append("Copyright &copy <span itemprop=\"copyrightYear\">" + CopyrightYear + "</span>");
                if (CopyrightHolder != null)
                {
                    if (CopyrightHolder is Person)
                    {
                        if (!string.IsNullOrWhiteSpace((CopyrightHolder as Person).GivenName) &&
                            !string.IsNullOrWhiteSpace((CopyrightHolder as Person).FamilyName))
                        {
                            html.Append(" <span itemprop=\"copyrightHolder\">" + (CopyrightHolder as Person).GivenName + " "
                                + (CopyrightHolder as Person).FamilyName + "</span>\n");
                        }
                    }
                    else
                    {
                        html.Append(" <span itemprop=\"copyrightHolder\">" + (CopyrightHolder as Thing).Name + "</span>\n");
                    }
                }
            }

            // Close out the containing div.
            html.Append("</div>\n");

            // Return the HTML generated.
            return html.ToString();
        }
    }
}
