using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application SignOn Attribute Statement objects for JSON formatting.
    /// </summary>
    public record ApplicationSignOnAttributeStatementJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Attribute type JSON property name.
        /// </summary>
        public const string TYPE = "type";

        /// <summary>
        /// Attribute name JSON property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Attribute namespace JSON property name.
        /// </summary>
        public const string NAMESPACE = "namespace";

        /// <summary>
        /// Attribute values JSON property name.
        /// </summary>
        public const string VALUES = "values";

        /// <summary>
        /// Attribute filter type JSON property name.
        /// </summary>
        public const string FILTER_TYPE = "filterType";

        /// <summary>
        /// Attribute filter value JSON property name.
        /// </summary>
        public const string FILTER_VALUE = "filterValue";

        // ----- PROPERTIES -----

        /// <summary>
        /// Attribute type.
        /// </summary>
        [JsonPropertyName(TYPE)]
        public string Type { get; init; } = default!;

        /// <summary>
        /// Attribute name.
        /// </summary>
        [JsonPropertyName(NAME)]
        public string Name { get; init; } = default!;

        /// <summary>
        /// Attribute namespace.
        /// </summary>
        [JsonPropertyName(NAMESPACE)]
        public string Namespace { get; init; } = default!;

        /// <summary>
        /// Attribute values.
        /// </summary>
        [JsonPropertyName(VALUES)]
        public object[] Values { get; init; } = default!;

        /// <summary>
        /// Attribute filter type.
        /// </summary>
        [JsonPropertyName(FILTER_TYPE)]
        public string FilterType { get; init; } = default!;

        /// <summary>
        /// Attribute filter value.
        /// </summary>
        [JsonPropertyName(FILTER_VALUE)]
        public string FilterValue { get; init; } = default!;
    }
}
