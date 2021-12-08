using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing HAL Link data for JSON formatting.
    /// A Link Object represents a hyperlink from the containing resource to a URI.
    /// Defined in: https://datatracker.ietf.org/doc/html/draft-kelly-json-hal-06
    /// </summary>
    public record LinkJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Deprecation JSON property name.
        /// </summary>
        public const string DEPRECATION = "deprecation";

        /// <summary>
        /// Href JSON property name.
        /// </summary>
        public const string HREF = "href";

        /// <summary>
        /// HrefLang JSON property name.
        /// </summary>
        public const string HREF_LANG = "hrefLang";

        /// <summary>
        /// Name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Profile JSON property name.
        /// </summary>
        public const string PROFILE = "profile";

        /// <summary>
        /// Templated JSON property name.
        /// </summary>
        public const string TEMPLATED = "templated";

        /// <summary>
        /// Title JSON property name.
        /// </summary>
        public const string TITLE = "title";

        /// <summary>
        /// Type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        // ----- PROPERTIES -----

        /// <summary>
        /// (Optional) Its presence indicates that the link is to be deprecated
        /// (i.e. removed) at a future date.Its value is a URL that SHOULD provide
        /// further information about the deprecation.
        /// </summary>
        [JsonPropertyName(DEPRECATION)]
        public string Deprecation { get; init; } = default!;

        /// <summary>
        /// (Required) Its value is either a URI [RFC3986] or a URI Template [RFC6570].
        /// If the value is a URI Template then the Link Object SHOULD have a
        /// "templated" attribute whose value is true.
        /// </summary>
        [JsonPropertyName(HREF)]
        public string Href { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value is a string and is intended for indicating the 
        /// language of the target resource(as defined by [RFC5988]).
        /// </summary>
        [JsonPropertyName(HREF_LANG)]
        public string HrefLang { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value MAY be used as a secondary key for selecting Link
        /// Objects which share the same relation type.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value is a string which is a URI that hints about the
        /// profile of the target resource.
        /// </summary>
        [JsonPropertyName(PROFILE)]
        public string Profile { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value is boolean and SHOULD be true when the Link Object's
        /// "href" property is a URI Template.
        /// Its value SHOULD be considered false if it is undefined or any other
        /// value than true.
        /// </summary>
        [JsonPropertyName(TEMPLATED)]
        public bool Templated { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value is a string and is intended for labelling the
        /// link with a human-readable identifier(as defined by [RFC5988]).
        /// </summary>
        [JsonPropertyName(TITLE)]
        public string Title { get; init; } = default!;

        /// <summary>
        /// (Optional) Its value is a string used as a hint to indicate the media
        /// type expected when dereferencing the target resource.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;
    }
}
