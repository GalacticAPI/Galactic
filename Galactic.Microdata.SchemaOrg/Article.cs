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
                    html.Append("<img itemprop=\"image\" src=\"" + (Image as Uri).ToString() + "\" >\n");
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
                html.Append("Created on: <meta itemprop=\"dateCreated\" content=\"" + DateCreated.ToString("yyyy-mm-dd") + "\">" + DateCreated.ToString("MMMM d, yyyy") + "\n");
            }

            // Write the date the work was last modified.
            if (DateModified.Ticks > 0)
            {
                html.Append("Modified on: <meta itemprop=\"dateModified\" content=\"" + DateModified.ToString("yyyy-mm-dd") + "\">" + DateModified.ToString("MMMM d, yyyy") + "\n");
            }

            // Write the date the work was published.
            if (DatePublished.Ticks > 0)
            {
                html.Append("Published on: <meta itemprop=\"datePublished\" content=\"" + DatePublished.ToString("yyyy-mm-dd") + "\">" + DatePublished.ToString("MMMM d, yyyy") + "\n");
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
