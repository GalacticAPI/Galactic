
using Galactic.Cryptography;
using SearchOperatorType = Galactic.Identity.GoogleWorkspace.GoogleWorkspaceClient.SearchOperatorType;
using Google.Apis.Admin.Directory.directory_v1.Data;
using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
using GoogleUser = Google.Apis.Admin.Directory.directory_v1.Data.User;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HashAlgorithmName = System.Security.Cryptography.HashAlgorithmName;

namespace Galactic.Identity.GoogleWorkspace
{
    public class User : Identity.User
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Addresses property name.
        /// </summary>
        public const string ADDRESSES = "addresses";

        /// <summary>
        /// Addresses[].CountryCode property name.
        /// </summary>
        public const string ADDRESSES_COUNTRY_CODE = "countryCode";

        /// <summary>
        /// Addresses[].Locality property name.
        /// </summary>
        public const string ADDRESSES_LOCALITY = "locality";

        /// <summary>
        /// Addresses[].PostalCode property name.
        /// </summary>
        public const string ADDRESSES_POSTAL_CODE = "postalCode";

        /// <summary>
        /// Addresses[].Region property name.
        /// </summary>
        public const string ADDRESSES_REGION = "region";

        /// <summary>
        /// Addresses[].StreetAddress property name.
        /// </summary>
        public const string ADDRESSES_STREET_ADDRESS = "streetAddress";

        /// <summary>
        /// AgreedToTerms property name.
        /// </summary>
        public const string AGREED_TO_TERMS = "agreedToTerms";

        /// <summary>
        /// Aliases property name.
        /// </summary>
        public const string ALIASES = "aliases";

        /// <summary>
        /// Archived property name.
        /// </summary>
        public const string ARCHIVED = "archived";

        /// <summary>
        /// ChangePasswordAtNextLogin property name.
        /// </summary>
        public const string CHANGE_PASSWORD_AT_NEXT_LOGIN = "changePasswordAtNextLogin";

        /// <summary>
        /// CreationTime property name.
        /// </summary>
        public const string CREATION_TIME = "creationTime";

        /// <summary>
        /// CustomerId property name.
        /// </summary>
        public const string CUSTOMER_ID = "customerId";

        /// <summary>
        /// CustomSchemas property name.
        /// </summary>
        public const string CUSTOM_SCHEMAS = "customSchemas";

        /// <summary>
        /// DeletionTime property name.
        /// </summary>
        public const string DELETION_TIME = "deletionTime";

        /// <summary>
        /// Emails property name.
        /// </summary>
        public const string EMAILS = "emails";

        /// <summary>
        /// ETag property name.
        /// </summary>
        public const string ETAG = "etag";

        /// <summary>
        /// ExternalIds property name.
        /// </summary>
        public const string EXTERNAL_IDS = "externalIds";

        /// <summary>
        /// Gender property name.
        /// </summary>
        public const string GENDER = "gender";

        /// <summary>
        /// HashFunction property name.
        /// </summary>
        public const string HASH_FUNCTION = "hashFunction";

        /// <summary>
        /// Id property name.
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// IMs property name.
        /// </summary>
        public const string IMS = "ims";

        /// <summary>
        /// IsEnforcedIn2Sv property name.
        /// </summary>
        public const string IS_ENFORCED_IN_2_SV = "isEnforcedIn2Sv";

        /// <summary>
        /// IsEnrolledIn2Sv property name.
        /// </summary>
        public const string IS_ENROLLED_IN_2_SV = "isEnrolledIn2Sv";

        /// <summary>
        /// IncludeInGlobalAddressList property name.
        /// </summary>
        public const string INCLUDE_IN_GLOBAL_ADDRESS_LIST = "includeInGlobalAddressList";

        /// <summary>
        /// IPWhitelisted property name.
        /// </summary>
        public const string IP_WHITELISTED = "ipWhitelisted";

        /// <summary>
        /// IsAdmin property name.
        /// </summary>
        public const string IS_ADMIN = "isAdmin";

        /// <summary>
        /// IsDelegatedAdmin property name.
        /// </summary>
        public const string IS_DELEGATED_ADMIN = "isDelegatedAdmin";

        /// <summary>
        /// IsMailboxSetup property name.
        /// </summary>
        public const string IS_MAILBOX_SETUP = "isMailboxSetup";

        /// <summary>
        /// Keywords property name.
        /// </summary>
        public const string KEYWORDS = "keywords";

        /// <summary>
        /// Kind property name.
        /// </summary>
        public const string KIND = "kind";

        /// <summary>
        /// Languages property name.
        /// </summary>
        public const string LANGUAGES = "languages";

        /// <summary>
        /// LastLoginTime property name.
        /// </summary>
        public const string LAST_LOGIN_TIME = "lastLoginTime";

        /// <summary>
        /// Locations property name.
        /// </summary>
        public const string LOCATIONS = "locations";

        /// <summary>
        /// Name property name.
        /// </summary>
        public const string NAME = "name";

        /// <summary>
        /// Name.FamilyName property name.
        /// </summary>
        public const string NAME_FAMILY_NAME = "familyName";

        /// <summary>
        /// Name.FullName property name.
        /// </summary>
        public const string NAME_FULL_NAME = "fullName";

        /// <summary>
        /// Name.GivenName property name.
        /// </summary>
        public const string NAME_GIVEN_NAME = "givenName";

        /// <summary>
        /// NonEditableAliases property name.
        /// </summary>
        public const string NON_EDITABLE_ALIASES = "nonEditableAliases";

        /// <summary>
        /// Notes property name.
        /// </summary>
        public const string NOTES = "notes";

        /// <summary>
        /// OrgUnitPath property name.
        /// </summary>
        public const string ORG_UNIT_PATH = "orgUnitPath";

        /// <summary>
        /// Organizations property name.
        /// </summary>
        public const string ORGANIZATIONS = "organizations";

        /// <summary>
        /// Organizations[].Department property name.
        /// </summary>
        public const string ORGANIZATIONS_DEPARTMENT = "department";

        /// <summary>
        /// Organizations[].Title property name.
        /// </summary>
        public const string ORGANIZATIONS_TITLE = "title";

        /// <summary>
        /// Password property name.
        /// </summary>
        public const string PASSWORD = "password";

        /// <summary>
        /// Phones property name.
        /// </summary>
        public const string PHONES = "phones";

        /// <summary>
        /// PosixAccounts property name.
        /// </summary>
        public const string POSIX_ACCOUNTS = "posixAccounts";

        /// <summary>
        /// PrimaryEmail property name.
        /// </summary>
        public const string PRIMARY_EMAIL = "primaryEmail";

        /// <summary>
        /// RecoveryEmail property name.
        /// </summary>
        public const string RECOVERY_EMAIL = "recoveryEmail";

        /// <summary>
        /// RecoveryPhone property name.
        /// </summary>
        public const string RECOVERY_PHONE = "recoveryPhone";

        /// <summary>
        /// Relations property name.
        /// </summary>
        public const string RELATIONS = "relations";

        /// <summary>
        /// SshPublicKeys property name.
        /// </summary>
        public const string SSH_PUBLIC_KEYS = "sshPublicKeys";

        /// <summary>
        /// Suspended property name.
        /// </summary>
        public const string SUSPENDED = "suspended";

        /// <summary>
        /// SuspensionReason property name.
        /// </summary>
        public const string SUSPENSION_REASON = "suspensionReason";

        /// <summary>
        /// ThumbnailPhotoETag property name.
        /// </summary>
        public const string THUMBNAIL_PHOTO_ETAG = "thumbnailPhotoETag";

        /// <summary>
        /// ThumbnailPhotoUrl property name.
        /// </summary>
        public const string THUMBNAIL_PHOTO_URL = "thumbnailPhotoUrl";

        /// <summary>
        /// Websites property name.
        /// </summary>
        public const string WEBSITES = "websites";

        // Search fields.

        /// <summary>
        /// Matches all address fields.
        /// </summary>
        public const string SEARCH_ADDRESS = "address";

        /// <summary>
        /// A country.
        /// </summary>
        public const string SEARCH_ADDRESS_COUNTRY = "addressCountry";

        /// <summary>
        /// An extended address, such as one including a sub-region.
        /// </summary>
        public const string SEARCH_ADDRESS_EXTENDED = "addressExtended";

        /// <summary>
        /// A town or city of the address.
        /// </summary>
        public const string SEARCH_ADDRESS_LOCALITY = "addressLocality";

        /// <summary>
        /// A post office box.
        /// </summary>
        public const string SEARCH_ADDRESS_PO_BOX = "addressPoBox";

        /// <summary>
        /// A ZIP or postal code.
        /// </summary>
        public const string SEARCH_ADDRESS_POSTAL_CODE = "addressPostalCode";

        /// <summary>
        /// An abbreviated province or state.
        /// </summary>
        public const string SEARCH_ADDRESS_REGION = "addressRegion";

        /// <summary>
        /// A street address.
        /// </summary>
        public const string SEARCH_ADDRESS_STREET = "addressStreet";

        /// <summary>
        /// The email address of a user's direct manager.
        /// </summary>
        public const string SEARCH_DIRECT_MANAGER = "directManager";

        /// <summary>
        /// The ID of a user's direct manager.
        /// </summary>
        public const string SEARCH_DIRECT_MANAGER_ID = "directManagerId";


        /// <summary>
        /// The user's e-mail addresses, including aliases.
        /// </summary>
        public const string SEARCH_EMAIL = "email";

        /// <summary>
        /// External ID value.
        /// </summary>
        public const string SEARCH_EXTERNAL_ID = "externalId";

        /// <summary>
        /// A user's family or last name.
        /// </summary>
        public const string SEARCH_FAMILY_NAME = "familyName";

        /// <summary>
        /// A user's given or first name.
        /// </summary>
        public const string SEARCH_GIVEN_NAME = "givenName";

        /// <summary>
        /// IM network ID.
        /// </summary>
        public const string SEARCH_IM = "im";

        /// <summary>
        /// Whether a user has super administrator privileges.
        /// </summary>
        public const string SEARCH_IS_ADMIN = "isAdmin";

        /// <summary>
        /// Whether a user has delegated administrator privileges.
        /// </summary>
        public const string SEARCH_IS_DELEGATED_ADMIN = "isDelegatedAdmin";

        /// <summary>
        /// Whether 2-step verification is enforced for the user.
        /// </summary>
        public const string SEARCH_IS_ENFORCED_IN_2_SV = "isEnforcedIn2Sv";

        /// <summary>
        /// Whether a user is enrolled in 2-step verification.
        /// </summary>
        public const string SEARCH_IS_ENROLLED_IN_2_SV = "isEnrolledIn2Sv";

        /// <summary>
        /// Whether a user's account is suspended.
        /// </summary>
        public const string SEARCH_IS_SUSPENDED = "isSuspended";

        /// <summary>
        /// The email address of a user's manager either directly or up the management chain.
        /// </summary>
        public const string SEARCH_MANAGER = "manager";

        /// <summary>
        /// The ID of a user's manager either directly or up the management chain.
        /// </summary>
        public const string SEARCH_MANAGER_ID = "managerId";

        /// <summary>
        /// The concatenated value of givenName and familyName.
        /// </summary>
        public const string SEARCH_NAME = "name";

        /// <summary>
        /// A cost center of an organization.
        /// </summary>
        public const string SEARCH_ORG_COST_CENTER = "orgCostCenter";

        /// <summary>
        /// A department within the organization.
        /// </summary>
        public const string SEARCH_ORG_DEPARTMENT = "orgDepartment";

        /// <summary>
        /// An organization's description.
        /// </summary>
        public const string SEARCH_ORG_DESCRIPTION = "orgDescription";

        /// <summary>
        /// An organization name.
        /// </summary>
        public const string SEARCH_ORG_NAME = "orgName";

        /// <summary>
        /// A user's title within the organization.
        /// </summary>
        public const string SEARCH_ORG_TITLE = "orgTitle";

        /// <summary>
        /// The full path of an org unit. This matches all org unit chains under the target. For example, 'orgUnitPath=/'
        /// returns all users in the organization.
        /// </summary>
        public const string SEARCH_ORG_UNIT_PATH = "orgUnitPath";

        /// <summary>
        /// A user's phone number.
        /// </summary>
        public const string SEARCH_PHONE = "phone";


        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Google Workspace.
        /// </summary>
        protected GoogleWorkspaceClient gws = null;

        /// <summary>
        /// The type of search operators supported by each searh field.
        /// </summary>
        public static Dictionary<string, SearchOperatorType[]> SearchOperatorsSupported = new()
        {
            [SEARCH_NAME] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_EMAIL] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains, SearchOperatorType.Starts },
            [SEARCH_GIVEN_NAME] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains, SearchOperatorType.Starts },
            [SEARCH_FAMILY_NAME] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains, SearchOperatorType.Starts },
            [SEARCH_IS_ADMIN] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_IS_DELEGATED_ADMIN] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_IS_SUSPENDED] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_IM] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_EXTERNAL_ID] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_MANAGER] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_MANAGER_ID] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_DIRECT_MANAGER] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_DIRECT_MANAGER_ID] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_ADDRESS] = new SearchOperatorType[] { SearchOperatorType.Contains },
            [SEARCH_ADDRESS_PO_BOX] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_EXTENDED] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_STREET] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_LOCALITY] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_REGION] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_POSTAL_CODE] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ADDRESS_COUNTRY] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ORG_NAME] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ORG_TITLE] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ORG_DESCRIPTION] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_ORG_COST_CENTER] = new SearchOperatorType[] { SearchOperatorType.Exact, SearchOperatorType.Contains },
            [SEARCH_PHONE] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_ORG_UNIT_PATH] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_IS_ENROLLED_IN_2_SV] = new SearchOperatorType[] { SearchOperatorType.Exact },
            [SEARCH_IS_ENFORCED_IN_2_SV] = new SearchOperatorType[] { SearchOperatorType.Exact }
        };

        /// <summary>
        /// The backing native data representing the User in Google Workspace.
        /// </summary>
        protected GoogleUser user = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// A list of the user's addresses. The maximum allowed data size is 10Kb.
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES)]
        public List<UserAddress> Addresses
        {
            get => (List<UserAddress>)user.Addresses;
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES, value) });
        }

        /// <summary>
        /// Indicates if the user is forced to change their password at next login. This setting doesn't apply when the user signs in via a third-party identity provider.
        /// </summary>
        [DirectorySystemPropertyName(CHANGE_PASSWORD_AT_NEXT_LOGIN)]
        public bool ChangePasswordAtNextLogin
        {
            get
            {
                if (user.ChangePasswordAtNextLogin != null)
                {
                    return (bool)user.ChangePasswordAtNextLogin;
                }
                else
                {
                    // No property was set, so they won't be forced to change their password on next login.
                    return false;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(CHANGE_PASSWORD_AT_NEXT_LOGIN, value) });
        }

        /// <summary>
        /// The user's city.
        /// (Google: For the first address in the list of addresses associated with the user.)
        /// </summary>
        public override string City
        {
            get => Locality;
            set
            {
                Locality = value;
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// (Google: For the first addresses in the list of address associated with the user.)
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES_COUNTRY_CODE)]
        public override string CountryCode
        {
            get
            {
                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // Return the country code associated with the first address in the user's list of addresses.
                    UserAddress address = user.Addresses[0];
                    return address.CountryCode;
                }
                else
                {
                    // No addresses associated with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES_COUNTRY_CODE, value) });
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        [DirectorySystemPropertyName(CREATION_TIME)]
        public override DateTime? CreationTime => user.CreationTime;

        /// <summary>
        /// The user's department.
        /// (Google: For the first organization in the list of organizations associated with the user.)
        /// </summary>
        [DirectorySystemPropertyName(ORGANIZATIONS_DEPARTMENT)]
        public override string Department
        {
            get
            {
                if (user.Organizations != null && user.Organizations.Count > 0)
                {
                    // Return the department associated with the first organization in the user's list of organizations.
                    UserOrganization org = user.Organizations[0];
                    return org.Department;
                }
                else
                {
                    // No organization associated with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ORGANIZATIONS_DEPARTMENT, value) });
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public override string DisplayName
        {
            get => FullName;
            set
            {
                FullName = value;
            }
        }

        /// <summary>
        /// A list of the user's email addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        [DirectorySystemPropertyName(EMAILS)]
        public List<string> Emails
        {
            get
            {
                // Get the list of user e-mail objects.
                IList<UserEmail> userEmails = user.Emails;
                
                // Create a new list of e-mail addresses to return.
                List<string> emails = new();

                // Populate the return list with the e-mail addresses from the user e-mail objects.
                foreach (UserEmail userEmail in userEmails)
                {
                    // If the e-mail is the user's primary e-mail. Make it first in the list.
                    if (userEmail.Primary != null && (bool)userEmail.Primary)
                    {
                        emails.Insert(0, userEmail.Address);
                    }
                    else
                    {
                        // Add other e-mail addresses to the end.
                        emails.Add(userEmail.Address);
                    }
                }

                // Return the list of e-mail addresses.
                return emails;
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(EMAILS, value) });
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public override List<string> EmailAddresses
        {
            get => Emails;
            set
            {
                Emails = value;
            }
        }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// (Google: Google Workspace doesn't explicitly define how employee ids should be noted, other than that it supports various external ids.
        /// This method returns the "organization" id of the user, which corresponds with "Employee ID" field in Google Workspace Admin.
        /// </summary>
        public override string EmployeeNumber
        {
            get
            {
                {
                    if (user.ExternalIds != null)
                    {
                        // Returns the "organization" external id.
                        foreach (UserExternalId externalId in user.ExternalIds)
                        {
                            if (externalId.Type == "organization")
                            {
                                return externalId.Value;
                            }
                        }
                        // No employee id was found.
                        return null;
                    }
                    else
                    {
                        // No addresses assocaited with the user.
                        return null;
                    }
                }
            }
            set
            {
                UserExternalId extId = new();
                extId.Type = "organization";
                extId.Value = value;
                IList<UserExternalId> extIds = user.ExternalIds;
                bool existingFound = false;
                foreach (UserExternalId id in extIds)
                {
                    if (id.Type == "organization")
                    {
                        // Update the existing entry.
                        id.Value = extId.Value;
                        existingFound = true;
                    }
                }
                if (!existingFound)
                {
                    // An existing employee id wasn't found. Add one.
                    user.ExternalIds.Add(extId);
                }
                user = gws.UpdateUser(UniqueId, new() { new(EXTERNAL_IDS, extIds) });
            }
        }

        /// <summary>
        /// A list of external IDs for the user, such as an employee or network ID. The maximum allowed data size is 2Kb.
        /// </summary>
        public List<UserExternalId> ExternalIds
        {
            get => (List<UserExternalId>)user.ExternalIds;
            set => user = gws.UpdateUser(UniqueId, new() { new(EXTERNAL_IDS, value) });
        }

        /// <summary>
        /// The user's last name. Required when creating a user account.
        /// </summary>
        [DirectorySystemPropertyName(NAME_FAMILY_NAME)]
        public string FamilyName
        {
            get => user.Name.FamilyName;
            set => user = gws.UpdateUser(UniqueId, new() { new(NAME_FAMILY_NAME, value) });
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public override string FirstName
        {
            get => GivenName;
            set
            {
                GivenName = value;
            }
        }

        /// <summary>
        /// The user's full name formed by concatenating their first and last name values.
        /// </summary>
        [DirectorySystemPropertyName(NAME_FULL_NAME)]
        public string FullName
        {
            get => user.Name.FullName;
            set => user = gws.UpdateUser(UniqueId, new() { new(NAME_FULL_NAME, value) });
        }

        /// <summary>
        /// An object containing the user's gender. Maximum allowed data size for this field is 1Kb.
        /// </summary>
        [DirectorySystemPropertyName(GENDER)]
        public UserGender Gender
        {
            get => (UserGender)user.Gender;
            set => user = gws.UpdateUser(UniqueId, new() { new(GENDER, value) });
        }

        /// <summary>
        /// The user's first name. Required whten creating a user account.
        /// </summary>
        [DirectorySystemPropertyName(NAME_GIVEN_NAME)]
        public string GivenName
        {
            get => user.Name.GivenName;
            set => user = gws.UpdateUser(UniqueId, new() { new(NAME_GIVEN_NAME, value) });
        }

        /// <summary>
        /// The list of groups this object is a member of, or null if they couldn't be retrieved.
        /// </summary>
        public override List<Identity.Group> Groups
        {
            get => gws.GetMemberGroups(UniqueId);
        }

        /// <summary>
        /// Stores the hash format of the password property. We recommend sending the password property value as a base 16 bit hexadecimal-encoded hash value.
        /// The following hashFunction values are allowed:
        /// DES
        /// MD5 - hash prefix is $1$
        /// SHA2-256 - hash prefix is $5$
        /// SHA2-512 - hash prefix is $6$
        /// If rounds are specified as part of the prefix, they must be 10,000 or fewer.
        /// </summary>
        [DirectorySystemPropertyName(HASH_FUNCTION)]
        public string HashFunction
        {
            get => user.HashFunction;
            set => user = gws.UpdateUser(UniqueId, new() { new(HASH_FUNCTION, value) });
        }

        /// <summary>
        /// The unique ID for the user. A user id can be used as a user request URI's userKey.
        /// </summary>
        [DirectorySystemPropertyName(ID)]
        public string Id
        {
            get => user.Id;
        }

        /// <summary>
        /// The user's Instant Messenger (IM) accounts. A user account can have multiple ims properties, but only one of these ims properties can be the primary IM contact.
        /// </summary>
        [DirectorySystemPropertyName(IMS)]
        public List<UserIm> Ims
        {
            get => (List<UserIm>)user.Ims;
            set => user = gws.UpdateUser(UniqueId, new() { new(IMS, value) });
        }

        /// <summary>
        /// Indicates if the user's profile is visible in the Google Workspace global address list when the contact sharing feature is enabled for the domain.
        /// </summary>
        [DirectorySystemPropertyName(INCLUDE_IN_GLOBAL_ADDRESS_LIST)]
        public bool IncludeInGlobalAddressList
        {
            get
            {
                if (user.IncludeInGlobalAddressList != null)
                {
                    return user.IncludeInGlobalAddressList.Value;
                }
                else
                {
                    return false;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(INCLUDE_IN_GLOBAL_ADDRESS_LIST, value) });
        }

        /// <summary>
        /// If true, the user's IP address is whitelisted.
        /// </summary>
        [DirectorySystemPropertyName(IP_WHITELISTED)]
        public bool IpWhitelisted
        {
            get
            {
                if (user.IpWhitelisted != null)
                {
                    return user.IpWhitelisted.Value;
                }
                else
                {
                    return false;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(IP_WHITELISTED, value) });
        }

        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public override bool IsDisabled => Suspended;

        /// <summary>
        /// A list of the user's keywords. The maximum allowed data size is 1Kb.
        /// </summary>
        [DirectorySystemPropertyName(KEYWORDS)]
        public List<UserKeyword> Keywords
        {
            get => (List<UserKeyword>)user.Keywords;
            set => user = gws.UpdateUser(UniqueId, new() { new(KEYWORDS, value) });
        }

        /// <summary>
        ///  The type of the API resource. For Users resources, the value is admin#directory#user.
        /// </summary>
        public string Kind
        {
            get => user.Kind;
        }

        /// <summary>
        /// A list of the user's languages. The maximum allowed data size is 1Kb.
        /// </summary>
        [DirectorySystemPropertyName(LANGUAGES)]
        public List<UserLanguage> Languages
        {
            get => (List<UserLanguage>)user.Languages;
            set => user = gws.UpdateUser(UniqueId, new() { new(LANGUAGES, value) });
        }

        /// <summary>
        /// The last time the user logged into the user's account.
        /// </summary>
        [DirectorySystemPropertyName(LAST_LOGIN_TIME)]
        public DateTime? LastLoginTime
        {
            get => user.LastLoginTime;
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public override string LastName
        {
            get => FamilyName;
            set
            {
                FamilyName = value;
            }
        }

        /// <summary>
        /// The town or city of the address.
        /// (Google: For the first address in the list of addresses associated with the user.)
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES_LOCALITY)]
        public string Locality
        {
            get
            {
                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // Return the locality associated with the first address in the user's list of addresses.
                    UserAddress address = user.Addresses[0];
                    return address.Locality;
                }
                else
                {
                    // No addresses associated with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES_LOCALITY, value) });
        }

        /// <summary>
        /// A list of the user's locations. The maximum allowed data size is 10Kb.
        /// </summary>
        [DirectorySystemPropertyName(LOCATIONS)]
        public List<UserLocation> Locations
        {
            get => (List<UserLocation>)user.Locations;
            set => user = gws.UpdateUser(UniqueId, new() { new(LOCATIONS, value) });
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        public override string Login
        {
            get => PrimaryEmailAddress;
            set
            {
                PrimaryEmailAddress = value;
            }
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public override string ManagerId
        {
            get
            {
                foreach(UserRelation relation in user.Relations)
                {
                    if (relation.Type == "manager")
                    {
                        return relation.Value;
                    }
                }
                return null;
            }
            set
            {
                IList<UserRelation> relations = user.Relations;

                // Update the existing manager relation if it exists.
                bool existingUpdated = false;
                foreach (UserRelation relation in relations)
                {
                    if (relation.Type == "manager")
                    {
                        // Update the existing relation.
                        relation.Value = value;
                        existingUpdated = true;
                    }
                }
                if (!existingUpdated)
                {
                    // Create a new relation and add it to the list.
                    UserRelation relation = new UserRelation();
                    relation.Type = "manager";
                    relation.Value = value;
                    relations.Add(relation);
                }

                // Update the group with the new relation value.
                user = gws.UpdateUser(UniqueId, new() { new(RELATIONS, relations) });
            }
        }

        /// <summary>
        /// The full name of the user's manager.
        /// (Google: Google doesn't have a name field for relations. This returns the relation value field. Same value as ManagerId.)
        /// </summary>
        public override string ManagerName => ManagerId;

        /// <summary>
        /// The user's middle name. (Not implemented by Google Workspace.)
        /// </summary>
        public override string MiddleName
        {
            get => "";
            set { }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public override string MobilePhone
        {
            get
            {
                if (user.Phones != null)
                {
                    // Search for a phone with the type "mobile".
                    foreach (UserPhone phone in user.Phones)
                    {
                        if (phone.Type == "mobile")
                        {
                            return phone.Value;
                        }
                    }
                    // No mobile phone number was found.
                    return null;
                }
                else
                {
                    // No phones associated with the user.
                    return null;
                }
            }
            set
            {
                IList<UserPhone> phones = user.Phones;

                // Update the existing mobile phone if it exists.
                bool existingUpdated = false;
                foreach (UserPhone phone in phones)
                {
                    if (phone.Type == "mobile")
                    {
                        // Update the existing phone.
                        phone.Value = value;
                        existingUpdated = true;
                    }
                }
                if (!existingUpdated)
                {
                    // Create a new phone and add it to the list.
                    UserPhone phone = new UserPhone();
                    phone.Type = "mobile";
                    phone.Value = value;
                    phones.Add(phone);
                }

                // Update the group with the new phone value.
                user = gws.UpdateUser(UniqueId, new() { new(PHONES, phones) });
            }
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// (Google: For the first organization in the list of organizations associated with the user.)
        /// </summary>
        public override string Organization
        {
            get
            {
                if (user.Organizations != null && user.Organizations.Count > 0)
                {
                    // Return the name of the first organization in the user's list of organizations.
                    UserOrganization org = user.Organizations[0];
                    return org.Name;
                }
                else
                {
                    // No organization associated with the user.
                    return null;
                }
            }
            set
            {
                IList<UserOrganization> orgs = user.Organizations;

                // Update the existing organization if it exists
                if (orgs.Count > 0)
                {
                    orgs[0].Name = value;
                }
                else
                {
                    // Add a new organization and set its name.
                    UserOrganization org = new();
                    org.Name = value;
                    orgs.Add(org);
                }

                // Update the group with the new organization value.
                user = gws.UpdateUser(UniqueId, new() { new(ORGANIZATIONS, orgs) });
            }
        }

        /// <summary>
        /// A list of organizations the user belongs to. The maximum allowed data size is 10Kb.
        /// </summary>
        [DirectorySystemPropertyName(ORGANIZATIONS)]
        public List<UserOrganization> Organizations
        {
            get => (List<UserOrganization>)user.Organizations;
            set => user = gws.UpdateUser(UniqueId, new() { new(ORGANIZATIONS, value) });
        }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public override bool PasswordChangeRequiredAtNextLogin => ChangePasswordAtNextLogin;

        /// <summary>
        /// Whether the user's password has expired.
        /// (Google: Not supported. Will always return false.)
        /// </summary>
        public override bool PasswordExpired => false;

        /// <summary>
        /// The date and time that the user's password was last set.
        /// (Google: Not supported. Will always return null.)
        /// </summary>
        public override DateTime? PasswordLastSet => null;

        /// <summary>
        /// A list of the user's phone numbers. The maximum allowed data size is 1Kb.
        /// </summary>
        [DirectorySystemPropertyName(PHONES)]
        public List<UserPhone> Phones
        {
            get => (List<UserPhone>)user.Phones;
            set => user = gws.UpdateUser(UniqueId, new() { new(PHONES, value) });
        }

        /// <summary>
        /// The user's physical address.
        /// (Google: For the first address in the list of addresses associated with the user.)
        /// </summary>
        public override string PhysicalAddress
        {
            get => StreetAddress;
            set => StreetAddress = value;
        }

        /// <summary>
        /// A list of POSIX account information for the user.
        /// </summary>
        [DirectorySystemPropertyName(POSIX_ACCOUNTS)]
        public List<UserPosixAccount> PosixAccounts
        {
            get => (List<UserPosixAccount>)user.PosixAccounts;
            set => user = gws.UpdateUser(UniqueId, new() { new(POSIX_ACCOUNTS, value) });
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// (Google: Google Workspace doesn't have a defined concept of a mailing address. This returns the street address of the first address in the list of addresses associated with the user.)
        /// </summary>
        public override string PostalAddress
        {
            get => StreetAddress;
            set => StreetAddress = value;
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// (Google: The postal code of the first address in the list of addresses associated with the user.)
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES_POSTAL_CODE)]
        public override string PostalCode
        {
            get
            {
                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // Return the postal code associated with the first address in the user's list of addresses.
                    UserAddress address = user.Addresses[0];
                    return address.PostalCode;
                }
                else
                {
                    // No addresses associated with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES_POSTAL_CODE, value) });
        }

        /// <summary>
        /// The user's primary email address. This property is required in a request to create a user account. The primaryEmail must be unique and cannot be an alias of another user.
        /// </summary>
        [DirectorySystemPropertyName(PRIMARY_EMAIL)]
        public string PrimaryEmail
        {
            get => user.PrimaryEmail;
            set => user = gws.UpdateUser(UniqueId, new() { new(PRIMARY_EMAIL, value) });
        }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public override string PrimaryEmailAddress
        {
            get => PrimaryEmail;
            set
            {
                PrimaryEmail = value;
            }
        }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        public override string PrimaryPhone
        {
            get
            {
                if (user.Phones != null)
                {
                    // Return the primary phone number from the user's list of phones.
                    foreach (UserPhone phone in user.Phones)
                    {
                        if (phone.Primary != null && (bool)phone.Primary)
                        {
                            return phone.Value;
                        }
                    }
                    // No primary phone associated with the user.
                    return null;
                }
                else
                {
                    // No phones associated with the user.
                    return null;
                }
            }
            set
            {
                IList<UserPhone> phones = user.Phones;

                // Update the existing primary phone if it exists.
                bool existingUpdated = false;
                foreach (UserPhone phone in phones)
                {
                    if (phone.Primary != null && phone.Primary.Value)
                    {
                        // Update the existing phone.
                        phone.Value = value;
                        existingUpdated = true;
                    }
                }
                if (!existingUpdated)
                {
                    // Create a new phone and add it to the list.
                    UserPhone phone = new UserPhone();
                    phone.Value = value;
                    phone.Primary = true;
                    phones.Add(phone);
                }

                // Update the group with the new phone value.
                user = gws.UpdateUser(UniqueId, new() { new(PHONES, phones) });
            }
        }

        /// <summary>
        /// Recovery email of the user.
        /// </summary>
        [DirectorySystemPropertyName(RECOVERY_EMAIL)]
        public string RecoveryEmail
        {
            get => user.RecoveryEmail;
            set => user = gws.UpdateUser(UniqueId, new() { new(RECOVERY_EMAIL, value) });
        }

        /// <summary>
        /// The abbreviated province or state.
        /// (Google: The region associated the first address in the list of addresses associated with the user.
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES_REGION)]
        public string Region
        {
            get
            {
                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // Return the region associated with the first address in the user's list of addresses.
                    UserAddress address = user.Addresses[0];
                    return address.Region;
                }
                else
                {
                    // No addresses assocaited with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES_REGION, value) });
        }

        /// <summary>
        /// A list of the user's relationships to other users. The maximum allowed data size for this field is 2Kb.
        /// </summary>
        [DirectorySystemPropertyName(RELATIONS)]
        public List<UserRelation> Relations
        {
            get => (List<UserRelation>)user.Relations;
            set => user = gws.UpdateUser(UniqueId, new() { new(RELATIONS, value) });
        }

        /// <summary>
        /// A list of SSH public keys.
        /// </summary>
        [DirectorySystemPropertyName(SSH_PUBLIC_KEYS)]
        public List<UserSshPublicKey> SshPublicKeys
        {
            get => (List<UserSshPublicKey>)user.SshPublicKeys;
            set => user = gws.UpdateUser(UniqueId, new() { new(SSH_PUBLIC_KEYS, value) });
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        public override string State
        {
            get => Region;
            set
            {
                Region = value;
            }
        }

        /// <summary>
        /// The street address portion of the user's first address in the list of addresses associated with their account.
        /// </summary>
        [DirectorySystemPropertyName(ADDRESSES_STREET_ADDRESS)]
        public string StreetAddress
        {
            get
            {
                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // Return the street address associated with the first address in the user's list of addresses.
                    UserAddress address = user.Addresses[0];
                    return address.StreetAddress;
                }
                else
                {
                    // No addresses associated with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ADDRESSES_STREET_ADDRESS, value) });
        }

        /// <summary>
        /// Indicates if user is suspended.
        /// </summary>
        [DirectorySystemPropertyName(SUSPENDED)]
        public bool Suspended
        {
            get
            {
                if (user.Suspended != null)
                {
                    return (bool)user.Suspended;
                }
                else
                {
                    // No suspension value was provided.
                    return false;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(SUSPENDED, value) });
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        [DirectorySystemPropertyName(ORGANIZATIONS_TITLE)]
        public override string Title
        {
            get
            {
                if (user.Organizations != null && user.Organizations.Count > 0)
                {
                    // Return the title associated with the first organization in the user's list of organizations.
                    UserOrganization org = user.Organizations[0];
                    return org.Title;
                }
                else
                {
                    // No organization assocaited with the user.
                    return null;
                }
            }
            set => user = gws.UpdateUser(UniqueId, new() { new(ORGANIZATIONS_TITLE, value) });
        }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public override string Type => Kind;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public override string UniqueId => Id;

        /// <summary>
        /// A list of the user's websites.
        /// </summary>
        [DirectorySystemPropertyName(WEBSITES)]
        public List<UserWebsite> Websites
        {
            get => (List<UserWebsite>)user.Websites;
            set => user = gws.UpdateUser(UniqueId, new() { new(WEBSITES, value) });
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes a Google Workspace user from a native object representing its properties.
        /// </summary>
        /// <param name="gws">A Google Workspace object used to query and manipulate the user.</param>
        /// <param name="json">A Google Workspace native object representing this user's properties.</param>
        public User(GoogleWorkspaceClient gws, GoogleUser user)
        {
            if (gws != null && user != null)
            {
                // Initialize the client.
                this.gws = gws;

                // Initialize the user data from the native object supplied.
                this.user = user;
            }
            else
            {
                if (gws == null)
                {
                    throw new ArgumentNullException(nameof(gws));
                }
                else
                {
                    throw new ArgumentNullException(nameof(user));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Disables the user's account if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public override bool Disable()
        {
            Suspended = true;
            return (Suspended);
        }

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public override bool Enable()
        {
            Suspended = false;
            return !Suspended;
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public override bool MemberOfGroup(Identity.Group group, bool recursive) => gws.GetMemberOfGroup(this, group, recursive);

        /// <summary>
        /// Sets the password of the user. (Uses a SHA2-512 hash.)
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public override bool SetPassword(string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                // Hash the password.
                string hashedPassword = Hash.GetHash(password, HashAlgorithmName.SHA512);

                // Send the password update.
                if ((user = gws.UpdateUser(UniqueId, new() { new(PASSWORD, hashedPassword), new(HASH_FUNCTION, "SHA2-512") })) != null)
                {
                    // The update was successful.
                    return true;
                }
            }
            // A password wasn't supplied or the request to set the password wasn't successful.
            return false;
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public override bool Unlock() => Enable();
    }
}
