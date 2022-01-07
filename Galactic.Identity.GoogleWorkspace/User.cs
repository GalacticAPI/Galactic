﻿using GoogleGroup = Google.Apis.Admin.Directory.directory_v1.Data.Group;
using GoogleUser = Google.Apis.Admin.Directory.directory_v1.Data.User;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.GoogleWorkspace
{
    public class User : IUser
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



        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Google Workspace.
        /// </summary>
        protected GoogleWorkspaceClient gws = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Indicates if the user is forced to change their password at next login. This setting doesn't apply when the user signs in via a third-party identity provider.
        /// </summary>
        [GoogleWorkspacePropertyName(CHANGE_PASSWORD_AT_NEXT_LOGIN)]
        public bool ChangePasswordAtNextLogin
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's city.
        /// </summary>
        public string City
        {
            get => Locality;
            set
            {
                Locality = value;
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        [GoogleWorkspacePropertyName(ADDRESSES_COUNTRY_CODE)]
        public string CountryCode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        [GoogleWorkspacePropertyName(CREATION_TIME)]
        public DateTime? CreationTime => throw new NotImplementedException();

        /// <summary>
        /// The user's department.
        /// </summary>
        [GoogleWorkspacePropertyName(ORGANIZATIONS_DEPARTMENT)]
        public string Department
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string DisplayName
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
        [GoogleWorkspacePropertyName(EMAILS)]
        public List<string> Emails
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public List<string> EmailAddresses
        {
            get => Emails;
            set
            {
                Emails = value;
            }
        }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        public string EmployeeNumber
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's last name. Required when creating a user account.
        /// </summary>
        [GoogleWorkspacePropertyName(NAME_FAMILY_NAME)]
        public string FamilyName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName
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
        [GoogleWorkspacePropertyName(NAME_FULL_NAME)]
        public string FullName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's first name. Required whten creating a user account.
        /// </summary>
        [GoogleWorkspacePropertyName(NAME_GIVEN_NAME)]
        public string GivenName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The list of groups this object is a member of, or null if they couldn't be retrieved.
        /// </summary>
        public List<IGroup> Groups
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The unique ID for the user. A user id can be used as a user request URI's userKey.
        /// </summary>
        [GoogleWorkspacePropertyName(ID)]
        public string Id
        {
            get => throw new NotImplementedException();
        }


        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public bool IsDisabled
        {
            get => Suspended;
        }

        /// <summary>
        ///  The type of the API resource. For Users resources, the value is admin#directory#user.
        /// </summary>
        public string Kind
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The last time the user logged into the user's account.
        /// </summary>
        [GoogleWorkspacePropertyName(LAST_LOGIN_TIME)]
        public DateTime? LastLoginTime
        {
            get
            {
                //  The value is in ISO 8601 date and time format. The time is the complete date plus hours, minutes, and seconds in the form YYYY-MM-DDThh:mm:ssTZD. For example, 2010-04-05T17:30:04+01:00.
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName
        {
            get => FamilyName;
            set
            {
                FamilyName = value;
            }
        }

        /// <summary>
        /// The town or city of the address.
        /// </summary>
        [GoogleWorkspacePropertyName(ADDRESSES_LOCALITY)]
        public string Locality
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        public string Login
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
        public string ManagerId
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public string ManagerName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's middle name. (Not implemented by Google Workspace.)
        /// </summary>
        public string MiddleName
        {
            get => "";
            set { }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public string MobilePhone
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        public string Organization
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public bool PasswordChangeRequiredAtNextLogin
        {
            get => ChangePasswordAtNextLogin;
        }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public bool PasswordExpired
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public DateTime? PasswordLastSet
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public string PhyscialAddress
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        public string PostalAddress
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        [GoogleWorkspacePropertyName(ADDRESSES_POSTAL_CODE)]
        public string PostalCode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's primary email address. This property is required in a request to create a user account. The primaryEmail must be unique and cannot be an alias of another user.
        /// </summary>
        [GoogleWorkspacePropertyName(PRIMARY_EMAIL)]
        public string PrimaryEmail
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public string PrimaryEmailAddress
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
        public string PrimaryPhone
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }


        /// <summary>
        /// The abbreviated province or state.
        /// </summary>
        [GoogleWorkspacePropertyName(ADDRESSES_REGION)]
        public string Region
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        public string State
        {
            get => Region;
            set
            {
                Region = value;
            }
        }

        /// <summary>
        /// Indicates if user is suspended.
        /// </summary>
        [GoogleWorkspacePropertyName(SUSPENDED)]
        public bool Suspended
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        [GoogleWorkspacePropertyName(ORGANIZATIONS_TITLE)]
        public string Title
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public string Type => Kind;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId => Id;

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
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public int CompareTo(IIdentityObject other)
        {
            return ((IIdentityObject)this).CompareTo(other);
        }

        /// <summary>
        /// Disables the user's account if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public bool Disable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public bool Enable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            return ((IIdentityObject)this).Equals(x, y);
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            // Create a list of IdentityAttributes to return.
            List<IdentityAttribute<object>> attributes = new();

            if (names != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(User).GetProperties();
                Dictionary<string, PropertyInfo> properties = new ();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (GoogleWorkspacePropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<GoogleWorkspacePropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }
                
                // Fill the list of IdentityAttributes with the name and value of the attribute with the supplied name.
                foreach (string name in names)
                {
                    if (properties.ContainsKey(name))
                    {
                        attributes.Add(new(name, properties[name].GetValue(this)));
                    }
                }
            }

            // Return the attributes found.
            return attributes;
        }

        /// <summary>
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public int GetHashCode([DisallowNull] IIdentityObject obj) => IIdentityObject.GetHashCode(obj);

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            return ((IIdentityObject)this).MemberOfGroup(group, recursive);
        }

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<object>> attributes)
        {
            // TODO: Rework so all attributes are set in a single request.

            // Create a list of IdentityAttributes to return with success or failure.
            List<IdentityAttribute<bool>> attributeResults = new();

            if (attributes != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(User).GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (GoogleWorkspacePropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<GoogleWorkspacePropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }

                // Iterate over all the attributes supplied, setting their values and marking success or failure in the attribute list to return.
                foreach (IdentityAttribute<object> attribute in attributes)
                {
                    // Check if the attribute supplied matches a property of the User.
                    if (properties.ContainsKey(attribute.Name))
                    {
                        // Set the property with the attribute value supplied.
                        try
                        {
                            properties[attribute.Name].SetValue(this, attribute.Value);
                            attributeResults.Add(new(attribute.Name, true));
                        }
                        catch
                        {
                            // There was an error setting the attribute's value.
                            attributeResults.Add(new(attribute.Name, false));

                        }
                    }
                }
            }

            // Return the success / failure results of settings the attributes.
            return attributeResults;
        }

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public bool SetPassword(string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public bool Unlock()
        {
            throw new NotImplementedException();
        }
    }
}