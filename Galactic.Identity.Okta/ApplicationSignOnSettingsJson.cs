using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta Application SignOn Settings objects for JSON formatting.
    /// </summary>
    public record ApplicationSignOnSettingsJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Default Relay State JSON property name.
        /// </summary>
        public const string DEFAULT_RELAY_STATE = "app";

        /// <summary>
        /// SSO ACS URL JSON property name.
        /// </summary>
        public const string SSO_ACS_URL = "ssoAcsUrl";

        /// <summary>
        /// IDP Issuer JSON property name.
        /// </summary>
        public const string IDP_ISSUER = "idpIssuer";

        /// <summary>
        /// Audience JSON property name.
        /// </summary>
        public const string AUDIENCE = "audience";

        /// <summary>
        /// Recipient JSON property name.
        /// </summary>
        public const string RECIPIENT = "recipient";

        /// <summary>
        /// Destination JSON property name.
        /// </summary>
        public const string DESTINATION = "destination";

        /// <summary>
        /// Subject Name ID Template JSON property name.
        /// </summary>
        public const string SUBJECT_NAME_ID_TEMPLATE = "subjectNameIdTemplate";

        /// <summary>
        /// Subject Name ID Format JSON property name.
        /// </summary>
        public const string SUBJECT_NAME_ID_FORMAT = "subjectNameIdFormat";

        /// <summary>
        /// Response Signed JSON property name.
        /// </summary>
        public const string RESPONSE_SIGNED = "responseSigned";

        /// <summary>
        /// Assertion Signed JSON property name.
        /// </summary>
        public const string ASSERTION_SIGNED = "assertionSigned";

        /// <summary>
        /// Signature Algorithm JSON property name.
        /// </summary>
        public const string SIGNATURE_ALGORITHM = "signatureAlgorithm";

        /// <summary>
        /// Digest Algorithm JSON property name.
        /// </summary>
        public const string DIGEST_ALGORITHM = "digestAlgorithm";

        /// <summary>
        /// Honor Force Authn JSON property name.
        /// </summary>
        public const string HONOR_FORCE_AUTHN = "honorForceAuthn";

        /// <summary>
        /// Authn Context Class Ref JSON property name.
        /// </summary>
        public const string AUTHN_CONTEXT_CLASS_REF = "authnContextClassRef";

        /// <summary>
        /// SP Issuer JSON property name.
        /// </summary>
        public const string SP_ISSUER = "spIssuer";

        /// <summary>
        /// Request Compressed JSON property name.
        /// </summary>
        public const string REQUEST_COMPRESSED = "requestCompressed";

        /// <summary>
        /// Allow Multiple ACS Endpoints JSON property name.
        /// </summary>
        public const string ALLOW_MULTIPLE_ACS_ENDPOINTS = "allowMultipleAcsEndpoints";

        /// <summary>
        /// ACS Endpoint JSON property name.
        /// </summary>
        public const string ACS_ENDPOINTS = "acsEndpoints";

        /// <summary>
        /// Attribute Statements JSON property name.
        /// </summary>
        public const string ATTRIBUTE_STATEMENTS = "attributeStatements";


        // ----- PROPERTIES -----

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(DEFAULT_RELAY_STATE)]
        public string DefaultRelayState { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(SSO_ACS_URL)]
        public string SsoAcsUrl { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(IDP_ISSUER)]
        public string IdpIssuer { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(AUDIENCE)]
        public string Audience { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(RECIPIENT)]
        public string Recipient { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(DESTINATION)]
        public string Destination { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(SUBJECT_NAME_ID_TEMPLATE)]
        public string SubjectNameIdTemplate { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(SUBJECT_NAME_ID_FORMAT)]
        public string SubjectNameIdFormat { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(RESPONSE_SIGNED)]
        public bool ResponseSigned { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(ASSERTION_SIGNED)]
        public bool AssertionSigned { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(SIGNATURE_ALGORITHM)]
        public string SignatureAlgorithm { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(DIGEST_ALGORITHM)]
        public string DigestAlgorithm { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(HONOR_FORCE_AUTHN)]
        public bool HonorForceAuthn { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(AUTHN_CONTEXT_CLASS_REF)]
        public string AuthnContextClassRef { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(SP_ISSUER)]
        public string SpIssuer { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(REQUEST_COMPRESSED)]
        public bool RequestCompressed { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(ALLOW_MULTIPLE_ACS_ENDPOINTS)]
        public bool AllowMultipleAcsEndpoints { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(ACS_ENDPOINTS)]
        public object AcsEndpoints { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName(ATTRIBUTE_STATEMENTS)]
        public ApplicationSignOnAttributeStatementJson[] AttributeStatements { get; init; } = default!;
    }
}
