using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Profile data for JSON formatting.
    /// </summary>
    public record UserProfileJson
    {
        /// <summary>
        /// The city or locality component of the User's address.
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; init; } = default!;

        /// <summary>
        /// Name of a cost center assigned to the User.
        /// </summary>
        [JsonPropertyName("costCenter")]
        public string CostCenter { get; init; } = default!;

        /// <summary>
        /// The country name component of the User's address.
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; init; } = default!;

        /// <summary>
        /// The name of the User's department.
        /// </summary>
        [JsonPropertyName("department")]
        public string Department { get; init; } = default!;

        /// <summary>
        /// The name of the User, suitable for display to end users.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string DisplayName { get; init; } = default!;

        /// <summary>
        /// The name of the User's division.
        /// </summary>
        [JsonPropertyName("division")]
        public string Division { get; init; } = default!;

        /// <summary>
        /// The primary e-mail address of the User.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; init; } = default!;

        /// <summary>
        /// The organization or company assigned unique identifier for the User.
        /// </summary>
        [JsonPropertyName("employeeNumber")]
        public string EmployeeNumber { get; init; } = default!;

        /// <summary>
        /// The given name of the User.
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; init; } = default!;

        /// <summary>
        /// The honorific prefix(es) of the User, or title in most Western languages.
        /// </summary>
        [JsonPropertyName("honorificPrefix")]
        public string HonorificPrefix { get; init; } = default!;

        /// <summary>
        /// The honorific suffix(es) of the User.
        /// </summary>
        [JsonPropertyName("honorificSuffix")]
        public string HonorificSuffix { get; init; } = default!;

        /// <summary>
        /// The family name of the User.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; init; } = default!;

        /// <summary>
        /// The User's default location for purposes of localizing items such as
        /// currency, date time format, numerical representations, etc.
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; init; } = default!;

        /// <summary>
        /// Unique identifier for the User.
        /// </summary>
        [JsonPropertyName("login")]
        public string Login { get; init; } = default!;

        /// <summary>
        /// The display name of the User's manager.
        /// </summary>
        [JsonPropertyName("manager")]
        public string Manager { get; init; } = default!;

        /// <summary>
        /// The id of the User's manager.
        /// </summary>
        [JsonPropertyName("managerId")]
        public string ManagerId { get; init; } = default!;

        /// <summary>
        /// The middle name(s) of the User.
        /// </summary>
        [JsonPropertyName("middleName")]
        public string MiddleName { get; init; } = default!;

        /// <summary>
        /// The mobile phone number of the User.
        /// </summary>
        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; init; } = default!;

        /// <summary>
        /// The casual way to address the User in real life.
        /// </summary>
        [JsonPropertyName("nickName")]
        public string NickName { get; init; } = default!;

        /// <summary>
        /// The name of the User's organization.
        /// </summary>
        [JsonPropertyName("organization")]
        public string Organization { get; init; } = default!;

        /// <summary>
        /// The mailing address component of the User's address.
        /// </summary>
        [JsonPropertyName("postalAddress")]
        public string PostalAddress { get; init; } = default!;

        /// <summary>
        /// The primary phone number of the User such as home number.
        /// </summary>
        [JsonPropertyName("primaryPhone")]
        public string PrimaryPhone { get; init; } = default!;

        /// <summary>
        /// The URL of the User's online profile (e.g. web page).
        /// </summary>
        [JsonPropertyName("profileUrl")]
        public string ProfileUrl { get; init; } = default!;

        /// <summary>
        /// The secondary e-mail address of the User, typically used for account
        /// recovery.
        /// </summary>
        [JsonPropertyName("secondEmail")]
        public string SecondEmail { get; init; } = default!;

        /// <summary>
        /// The state or region component of the User's address.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; init; } = default!;

        /// <summary>
        /// The full street address component of the User's address.
        /// </summary>
        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; init; } = default!;

        /// <summary>
        /// The User's time zone.
        /// </summary>
        [JsonPropertyName("timezone")]
        public string TimeZone { get; init; } = default!;

        /// <summary>
        /// Used to describe the organization to user relationship such as "Employee"
        /// or "Contractor".
        /// </summary>
        [JsonPropertyName("userType")]
        public string UserType { get; init; } = default!;

        /// <summary>
        /// The ZIP code or postal code component of the User's address.
        /// </summary>
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; init; } = default!;
    }
}
