using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;


namespace Galactic.Microdata.SchemaOrg
{
    /// <summary>
    /// http://schema.org/Thing
    /// The most generic type of item.
    /// </summary>
    [DataContract(Name = "Thing", Namespace = "http://schema.org/Thing")]
    public class Thing
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// An additional type for the item, typically used for adding more specific
        /// types from external vocabularies in microdata syntax. This is a relationship
        /// between something and a class that the thing is in. In RDFa syntax, it is
        /// better to use the native RDFa syntax - the 'typeof' attribute - for multiple
        /// types. Schema.org tools may have only weaker understanding of extra types,
        /// in particular those defined externally
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "additionalType")]
        public Uri AdditionalType;

        /// <summary>
        /// An alias for the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "alternateName")]
        public string AlternateName;

        /// <summary>
        /// A short description of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "description")]
        public string Description;

        /// <summary>
        /// The name of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name= "name")]
        public string Name;

        /// <summary>
        /// Indicates a potential Action, which describes an idealized action in which
        /// this thing would play an 'object' role.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "potentialAction")]
        public Action PotentialAction;

        /// <summary>
        /// URL of a reference Web page that unambiguously indicates the item's identity.
        /// E.g. the URL of the item's Wikipedia page, Freebase page, or official website.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "sameAs")]
        public Uri SameAs;

        /// <summary>
        /// URL of the item.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "url")]
        public Uri Url;

        // ----- PROPERTIES -----

        /// <summary>
        /// An image of the item.
        /// Expected Types: URI or ImageObject.
        /// </summary>
        [DataMember (EmitDefaultValue = false, Name = "image")]
        public dynamic Image
        {
            get; 
            set;
        }

        /// <summary>
        /// The Schema.org type of the item.
        /// </summary>
        public virtual string ItemType { get { return "Thing"; } }

        /// <summary>
        /// A short description of the Schema.org type associated with this item.
        /// </summary>
        public virtual string ItemTypeDescription { get { return "The most generic type of item."; } }

        /// <summary>
        /// The URL of the type definition on Schema.org.
        /// </summary>
        public virtual Uri ItemTypeUrl { get { return new Uri("http://schema.org/Thing"); } }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Thing()
        {

        }

        /// <summary>
        /// Construct a Thing from an ExpandoObject with like properties and values.
        /// </summary>
        /// <param name="expando">The ExpandoObject to use when populating the Thing.</param>
        public Thing (ExpandoObject expando)
        {
            if (expando != null)
            {
                foreach (string propertyName in ((IDictionary<string, object>)expando).Keys)
                {
                    // Check whether the propertyName is a field or property of the associated
                    // object and set the value of it.
                    FieldInfo fieldInfo = this.GetType().GetField(propertyName);
                    PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
                    object value = ((IDictionary<string, object>)expando)[propertyName];
                    if (fieldInfo != null)
                    {
                        // Check that this field can be set.
                        try
                        {
                            if (fieldInfo.FieldType.Name == "Uri" && value is string)
                            {
                                fieldInfo.SetValue(this, new Uri((string)value));
                            }
                            else
                            {
                                fieldInfo.SetValue(this, value);
                            }
                        }
                        catch
                        {
                            // There was an error setting the field's value, or it does not support setting.
                            // Skip this field.
                            continue;
                        }
                    }
                    else if (propertyInfo != null)
                    {
                        // Check that this property can be set.
                        try
                        {
                            if (propertyInfo.PropertyType.Name == "Uri" && value is string)
                            {
                                propertyInfo.SetValue(this, new Uri((string)value));
                            }
                            else
                            {
                                propertyInfo.SetValue(this, value);
                            }
                        }
                        catch
                        {
                            // There was an error setting the property's value, or it does not support setting.
                            // Skip this property.
                            continue;
                        }
                    }
                    else
                    {
                        // The propertyName does not correlate to a field or property on the object.
                        // Skip this propertyName.
                        continue;
                    }

                    // Set the field or property on the object.
                    //this.GetType().InvokeMember(propertyName, bindingFlags, null, this, new [] { ((IDictionary<string, object>)expando)[propertyName] });
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Converts a list of string to a single string of the same items separated by the
        /// supplied separator character.
        /// </summary>
        /// <param name="list">The list to convert to a string.</param>
        /// <param name="separator">The separator character to use when creating the string.</param>
        /// <returns>A string containing the items of the list separated by the separator character, or an empty string if there was an error processing the list.</returns>
        public static string ListToString(List<string> list, char separator)
        {
            if (list != null)
            {
                try
                {
                    // Build a string from the list.
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < list.Count; i++ )
                    {
                        builder.Append(list[i]);
                        if (i != list.Count - 1)
                        {
                            // Only append a separator if this is not the last item in the list.
                            builder.Append(separator);
                        }
                    }
                    return builder.ToString();
                }
                catch
                {
                    // There was an error building the string.
                    return "";
                }
            }
            else
            {
                // A list or separator was not supplied.
                return "";
            }
        }

        /// <summary>
        /// Returns a dictionary containing microdata about the item.
        /// NOTE:
        /// Each dictionary item has a key that is the name of a field or property, and its associated value.
        /// </summary>
        /// <returns>A dictionary containing microdata about the item.</returns>
        protected Dictionary<string, object> GetMicrodata()
        {
            Dictionary<string, object> microdata = new Dictionary<string,object>();

            // Iterate over all the properties and fields of the item using reflection and
            // add their HTML microdata representation to the StringBuilder.
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    // Check the property's attributes to see if there is a name to use during serialization.
                    string propertyName = property.Name;
                    DataMemberAttribute dataMemberAttribute = property.GetCustomAttribute<DataMemberAttribute>(true);
                    if (dataMemberAttribute != null)
                    {
                        propertyName = dataMemberAttribute.Name;
                    }
                    
                    // Add the property information.
                    microdata.Add(propertyName, property.GetValue(this));
                }
                FieldInfo[] fields = this.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    // Check the field's attributes to see if there is a name to use during serialization.
                    string fieldName = field.Name;
                    DataMemberAttribute dataMemberAttribute = field.GetCustomAttribute<DataMemberAttribute>(true);
                    if (dataMemberAttribute != null)
                    {
                        fieldName = dataMemberAttribute.Name;
                    }

                    // Add the field information.
                    microdata.Add(fieldName, field.GetValue(this));
                }

                // Remove ItemTypeDescription before returning.
                microdata.Remove("ItemTypeDescription");

                return microdata;
            }
            catch
            {
                // There was a problem adding the properties and fields.
                return null;
            }
        }

        /// <summary>
        /// Converts a list of strings stored in a comma (or other character) separated string
        /// into a list of strings.
        /// </summary>
        /// <param name="listString">The string representation of the list.</param>
        /// <param name="separators">An array of characters used to separate the strings in the list.</param>
        /// <returns>A List with each item from the listString as a member of the list, or an empty List if there was an error processing the listString.</returns>
        public static List<string> StringToList(string listString, char[] separators)
        {
            if (!string.IsNullOrWhiteSpace(listString) && separators.Length > 0)
            {
                List<string> list = new List<string>(listString.Split(separators));

                // Remove any leading or trailing space from each item in the list.
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].Trim();
                }

                return list;
            }
            else
            {
                // A listString or array of separators was not supplied.
                return new List<string>();
            }
        }

        /// <summary>
        /// Returns the item as an ExpandoObject.
        /// </summary>
        /// <returns>An ExpandoObject, or null if the item could not be converted.</returns>
        public ExpandoObject ToExpandoObject()
        {
            ExpandoObject expando = new ExpandoObject();

            // Iterate over all the properties and fields of the item using reflection and
            // adds them to the ExpandoObject.
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    ((IDictionary<string, Object>)expando).Add(property.Name, property.GetValue(this));
                }
                FieldInfo[] fields = this.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    ((IDictionary<string, Object>)expando).Add(field.Name, field.GetValue(this));
                }

                // Remove ItemTypeDescription before returning.
                ((IDictionary<string, Object>)expando).Remove("ItemTypeDescription");

                return expando;
            }
            catch
            {
                // There was a problem adding the properties and fields.
                return null;
            }
        }

        /// <summary>
        /// Returns the item as microdata annotated HTML.
        /// </summary>
        /// <param name="itemprop">The name of the property that this item is the value of in another item. May be null if this item
        /// is not a property of another.</param>
        /// <returns>Returns a string of microdata annotated HTML, or an empty string if the item could not be converted.</returns>
        public virtual string ToMicrodata(string itemprop = null)
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

            // Write the item's name.
            if (!string.IsNullOrWhiteSpace(Name))
            {
                html.Append("<h1 itemprop=\"name\">" + Name + "</h1>\n");
            }

            // Write an img tag for the item's associated image.
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

            // Write a description of the item.
            if (!string.IsNullOrWhiteSpace(Description))
            {
                html.Append("Description: <span itemprop=\"description\">" + Description + "</span>\n");
            }

            // Close out the containing div.
            html.Append("</div>\n");

            // Return the HTML generated.
            return html.ToString();
        }
    }
}
