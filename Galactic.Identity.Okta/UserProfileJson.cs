using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Profile data for JSON formatting.
    /// </summary>
    public record UserProfileJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// City JSON property name.
        /// </summary>
        public const string CITY = "city";

        /// <summary>
        /// CostCenter JSON property name.
        /// </summary>
        public const string COST_CENTER = "costCenter";

        /// <summary>
        /// CountryCode JSON property name.
        /// </summary>
        public const string COUNTRY_CODE = "countryCode";

        /// <summary>
        /// Department JSON property name.
        /// </summary>
        public const string DEPARTMENT = "department";

        /// <summary>
        /// DisplayName JSON property name.
        /// </summary>
        public const string DISPLAY_NAME = "displayName";

        /// <summary>
        /// Division JSON property name.
        /// </summary>
        public const string DIVISION = "division";

        /// <summary>
        /// Email JSON property name.
        /// </summary>
        public const string EMAIL = "email";

        /// <summary>
        /// EmployeeNumber JSON property name.
        /// </summary>
        public const string EMPLOYEE_NUMBER = "employeeNumber";

        /// <summary>
        /// FirstName JSON property name.
        /// </summary>
        public const string FIRST_NAME = "firstName";

        /// <summary>
        /// HonorificPrefix JSON property name.
        /// </summary>
        public const string HONORIFIC_PREFIX = "honorificPrefix";

        /// <summary>
        /// HonorificSuffix JSON property name.
        /// </summary>
        public const string HONORIFIC_SUFFIX = "honorificSuffix";

        /// <summary>
        /// LastName JSON property name.
        /// </summary>
        public const string LAST_NAME = "lastName";

        /// <summary>
        /// Locale JSON property name.
        /// </summary>
        public const string LOCALE = "locale";

        /// <summary>
        /// Login JSON property name.
        /// </summary>
        public const string LOGIN = "login";

        /// <summary>
        /// Manager JSON property name.
        /// </summary>
        public const string MANAGER = "manager";

        /// <summary>
        /// ManagerId JSON property name.
        /// </summary>
        public const string MANAGER_ID = "managerId";

        /// <summary>
        /// MiddleName JSON property name.
        /// </summary>
        public const string MIDDLE_NAME = "middleName";

        /// <summary>
        /// MobilePhone JSON property name.
        /// </summary>
        public const string MOBILE_PHONE = "mobilePhone";

        /// <summary>
        /// NickName JSON property name.
        /// </summary>
        public const string NICK_NAME = "nickName";

        /// <summary>
        /// Organization JSON property name.
        /// </summary>
        public const string ORGANIZATION = "organization";

        /// <summary>
        /// PostalAddress JSON property name.
        /// </summary>
        public const string POSTAL_ADDRESS = "postalAddress";

        /// <summary>
        /// PrimaryPhone JSON property name.
        /// </summary>
        public const string PRIMARY_PHONE = "primaryPhone";

        /// <summary>
        /// ProfileUrl JSON property name.
        /// </summary>
        public const string PROFILE_URL = "profileUrl";

        /// <summary>
        /// SecondEmail JSON property name.
        /// </summary>
        public const string SECOND_EMAIL = "secondEmail";

        /// <summary>
        /// State JSON property name.
        /// </summary>
        public const string STATE = "state";

        /// <summary>
        /// StreetAddress JSON property name.
        /// </summary>
        public const string STREET_ADDRESS = "streetAddress";

        /// <summary>
        /// TimeZone JSON property name.
        /// </summary>
        public const string TIME_ZONE = "timezone";

        /// <summary>
        /// UserType JSON property name.
        /// </summary>
        public const string USER_TYPE = "userType";

        /// <summary>
        /// ZipCode JSON property name.
        /// </summary>
        public const string ZIP_CODE = "zipCode";

        // ----- PROPERTIES -----

        /// <summary>
        /// The city or locality component of the User's address.
        /// </summary>
        [JsonPropertyName(CITY)]
        public string City { get; init; } = default!;

        /// <summary>
        /// Name of a cost center assigned to the User.
        /// </summary>
        [JsonPropertyName(COST_CENTER)]
        public string CostCenter { get; init; } = default!;

        /// <summary>
        /// The country name component of the User's address.
        /// </summary>
        [JsonPropertyName(COUNTRY_CODE)]
        public string CountryCode { get; init; } = default!;

        /// <summary>
        /// The name of the User's department.
        /// </summary>
        [JsonPropertyName(DEPARTMENT)]
        public string Department { get; init; } = default!;

        /// <summary>
        /// The name of the User, suitable for display to end users.
        /// </summary>
        [JsonPropertyName(DISPLAY_NAME)]
        public string DisplayName { get; init; } = default!;

        /// <summary>
        /// The name of the User's division.
        /// </summary>
        [JsonPropertyName(DIVISION)]
        public string Division { get; init; } = default!;

        /// <summary>
        /// The primary e-mail address of the User.
        /// </summary>
        [JsonPropertyName(EMAIL)]
        public string Email { get; init; } = default!;

        /// <summary>
        /// The organization or company assigned unique identifier for the User.
        /// </summary>
        [JsonPropertyName(EMPLOYEE_NUMBER)]
        public string EmployeeNumber { get; init; } = default!;

        /// <summary>
        /// The given name of the User.
        /// </summary>
        [JsonPropertyName(FIRST_NAME)]
        public string FirstName { get; init; } = default!;

        /// <summary>
        /// The honorific prefix(es) of the User, or title in most Western languages.
        /// </summary>
        [JsonPropertyName(HONORIFIC_PREFIX)]
        public string HonorificPrefix { get; init; } = default!;

        /// <summary>
        /// The honorific suffix(es) of the User.
        /// </summary>
        [JsonPropertyName(HONORIFIC_SUFFIX)]
        public string HonorificSuffix { get; init; } = default!;

        /// <summary>
        /// The family name of the User.
        /// </summary>
        [JsonPropertyName(LAST_NAME)]
        public string LastName { get; init; } = default!;

        /// <summary>
        /// The User's default location for purposes of localizing items such as
        /// currency, date time format, numerical representations, etc.
        /// </summary>
        [JsonPropertyName(LOCALE)]
        public string Locale { get; init; } = default!;

        /// <summary>
        /// Unique identifier for the User.
        /// </summary>
        [JsonPropertyName(LOGIN)]
        public string Login { get; init; } = default!;

        /// <summary>
        /// The display name of the User's manager.
        /// </summary>
        [JsonPropertyName(MANAGER)]
        public string Manager { get; init; } = default!;

        /// <summary>
        /// The id of the User's manager.
        /// </summary>
        [JsonPropertyName(MANAGER_ID)]
        public string ManagerId { get; init; } = default!;

        /// <summary>
        /// The middle name(s) of the User.
        /// </summary>
        [JsonPropertyName(MIDDLE_NAME)]
        public string MiddleName { get; init; } = default!;

        /// <summary>
        /// The mobile phone number of the User.
        /// </summary>
        [JsonPropertyName(MOBILE_PHONE)]
        public string MobilePhone { get; init; } = default!;

        /// <summary>
        /// The casual way to address the User in real life.
        /// </summary>
        [JsonPropertyName(NICK_NAME)]
        public string NickName { get; init; } = default!;

        /// <summary>
        /// The name of the User's organization.
        /// </summary>
        [JsonPropertyName(ORGANIZATION)]
        public string Organization { get; init; } = default!;

        /// <summary>
        /// The mailing address component of the User's address.
        /// </summary>
        [JsonPropertyName(POSTAL_ADDRESS)]
        public string PostalAddress { get; init; } = default!;

        /// <summary>
        /// The primary phone number of the User such as home number.
        /// </summary>
        [JsonPropertyName(PRIMARY_PHONE)]
        public string PrimaryPhone { get; init; } = default!;

        /// <summary>
        /// The URL of the User's online profile (e.g. web page).
        /// </summary>
        [JsonPropertyName(PROFILE_URL)]
        public string ProfileUrl { get; init; } = default!;

        /// <summary>
        /// The secondary e-mail address of the User, typically used for account
        /// recovery.
        /// </summary>
        [JsonPropertyName(SECOND_EMAIL)]
        public string SecondEmail { get; init; } = default!;

        /// <summary>
        /// The state or region component of the User's address.
        /// </summary>
        [JsonPropertyName(STATE)]
        public string State { get; init; } = default!;

        /// <summary>
        /// The full street address component of the User's address.
        /// </summary>
        [JsonPropertyName(STREET_ADDRESS)]
        public string StreetAddress { get; init; } = default!;

        /// <summary>
        /// The User's time zone.
        /// </summary>
        [JsonPropertyName(TIME_ZONE)]
        public string TimeZone { get; init; } = default!;

        /// <summary>
        /// Used to describe the organization to user relationship such as "Employee"
        /// or "Contractor".
        /// </summary>
        [JsonPropertyName(USER_TYPE)]
        public string UserType { get; init; } = default!;

        /// <summary>
        /// The ZIP code or postal code component of the User's address.
        /// </summary>
        [JsonPropertyName(ZIP_CODE)]
        public string ZipCode { get; init; } = default!;
    }
}
