using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.Okta
{
    public class User : Identity.User
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the User in Okta.
        /// </summary>
        protected UserJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Timestamp when User's transition to ACTIVE status completed.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.ACTIVATED)]
        public DateTime? Activated => json.Activated;

        /// <summary>
        /// The user's city.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.CITY)]
        public override string City
        {
            get => json.Profile.City;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.CITY, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// Name of a cost center assigned to the User.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.COST_CENTER)]
        public string CostCenter
        {
            get => json.Profile.CostCenter;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.COST_CENTER, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.COUNTRY_CODE)]
        public override string CountryCode
        {
            get => json.Profile.CountryCode;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.COUNTRY_CODE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// Timestamp when User was created.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.CREATED)]
        public DateTime? Created
        {
            get => json.Created;
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public override DateTime? CreationTime => Created;

        /// <summary>
        /// The user's department.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.DEPARTMENT)]
        public override string Department
        {
            get => json.Profile.Department;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.DEPARTMENT, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.DISPLAY_NAME)]
        public override string DisplayName
        {
            get => json.Profile.DisplayName;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.DISPLAY_NAME, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The User's division.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.DIVISION)]
        public string Division
        {
            get => json.Profile.Division;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.DIVISION, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The User's primary e-mail address.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.EMAIL)]
        public string Email
        {
            get => json.Profile.Email;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.EMAIL, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public override List<string> EmailAddresses
        {
            get
            {
                return new() { Email, SecondEmail };
            }
            set
            {
                if (value != null)
                {
                    if (value.Count > 0)
                    {
                        Email = value[0];
                    }
                    if (value.Count > 1)
                    {
                        SecondEmail = value[1];
                    }
                }
            }
        }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.EMPLOYEE_NUMBER)]
        public override string EmployeeNumber
        {
            get => json.Profile.EmployeeNumber;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.EMPLOYEE_NUMBER, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.FIRST_NAME)]
        public override string FirstName
        {
            get => json.Profile.FirstName;
            set
            {
                {
                    // Create the identity attribute object with the value set.
                    IdentityAttribute<string> attribute = new(UserProfileJson.FIRST_NAME, value);

                    // Update the user with the new value.
                    okta.UpdateUser(this, attribute);
                }
            }
        }

        /// <summary>
        /// The list of groups this object is a member of, or null if they couldn't be retrieved.
        /// </summary>
        public override List<Identity.Group> Groups
        {
            get
            {
                // Retrieve the list of JSON objects representing the groups.
                List<GroupJson> groupJsonList = okta.GetUserGroups(UniqueId);

                if (groupJsonList != null)
                {
                    // Create Group objects from the JSON.
                    List<Identity.Group> groups = new();
                    foreach (GroupJson json in groupJsonList)
                    {
                        groups.Add(new Group(okta, json));
                    }

                    // Return the groups.
                    return groups;
                }
                else
                {
                    // There was an error retrieving the list of groups.
                    return null;
                }
            }
        }

        /// <summary>
        /// The honorific prefix(es) of the User, or title in most Western languages.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.HONORIFIC_PREFIX)]
        public string HonorificPrefix
        {
            get => json.Profile.HonorificPrefix;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.HONORIFIC_PREFIX, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The honorific suffix(es) of the User.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.HONORIFIC_SUFFIX)]
        public string HonorificSuffix
        {
            get => json.Profile.HonorificSuffix;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.HONORIFIC_SUFFIX, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The User's unique key.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.ID)]
        public string Id => json.Id;


        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public override bool IsDisabled
        {
            get
            {
                if (json.Status == "SUSPENDED")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Timestamp of User's last login.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.LAST_LOGIN)]
        public DateTime? LastLogin => json.LastLogin;

        /// <summary>
        /// The user's last name.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.LAST_NAME)]
        public override string LastName
        {
            get => json.Profile.LastName;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.LAST_NAME, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// Timestamp when User was last updated.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.LAST_UPDATED)]
        public DateTime? LastUpdated => json.LastUpdated;

        /// <summary>
        /// The User's default location for purposes of localizing items such as
        /// currency, date time format, numerical representations, etc.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.LOCALE)]
        public string Locale
        {
            get => json.Profile.Locale;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.LOCALE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.LOGIN)]
        public override string Login
        {
            get => json.Profile.Login;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.LOGIN, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The display name of the User's manager.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.MANAGER)]
        public string Manager
        {
            get => json.Profile.Manager;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.MANAGER, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.MANAGER_ID)]
        public override string ManagerId
        {
            get => json.Profile.ManagerId;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.MANAGER_ID, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public override string ManagerName => Manager;

        /// <summary>
        /// The user's middle name.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.MIDDLE_NAME)]
        public override string MiddleName
        {
            get => json.Profile.MiddleName;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.MIDDLE_NAME, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.MOBILE_PHONE)]
        public override string MobilePhone
        {
            get => json.Profile.MobilePhone;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.MOBILE_PHONE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The casual way to address the User in real life.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.NICK_NAME)]
        public string NickName
        {
            get => json.Profile.NickName;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.NICK_NAME, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.ORGANIZATION)]
        public override string Organization
        {
            get => json.Profile.Organization;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.ORGANIZATION, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// Timestamp when User's password was last changed.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.PASSWORD_CHANGED)]
        public DateTime? PasswordChanged => json.PasswordChanged;

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public override bool PasswordChangeRequiredAtNextLogin
        {
            get
            {
                if (json.Status == "PASSWORD_EXPIRED" || json.Status == "RECOVERY")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public override bool PasswordExpired
        {
            get
            {
                if (json.Status == "PASSWORD_EXPIRED")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public override DateTime? PasswordLastSet => PasswordChanged;

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public override string PhysicalAddress
        {
            get => StreetAddress;
            set
            {
                StreetAddress = value;
            }
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.POSTAL_ADDRESS)]
        public override string PostalAddress
        {
            get => json.Profile.PostalAddress;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.POSTAL_ADDRESS, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public override string PostalCode
        {
            get => ZipCode;
            set
            {
                ZipCode = value;
            }
        }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public override string PrimaryEmailAddress
        {
            get => Email;
            set
            {
                Email = value;
            }
        }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.PRIMARY_PHONE)]
        public override string PrimaryPhone
        {
            get => json.Profile.PrimaryPhone;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.PRIMARY_PHONE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The URL of the User's online profile (e.g. web page).
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.PROFILE_URL)]
        public string ProfileUrl
        {
            get => json.Profile.ProfileUrl;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.PROFILE_URL, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The secondary e-mail address of the User, typically used for account
        /// recovery.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.SECOND_EMAIL)]
        public string SecondEmail
        {
            get => json.Profile.SecondEmail;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.SECOND_EMAIL, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.STATE)]
        public override string State
        {
            get => json.Profile.State;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.STATE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The User's current status.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.STATUS)]
        public string Status => json.Status;

        /// <summary>
        /// Timestamp when User's status last changed.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.STATUS_CHANGED)]
        public DateTime? StatusChanged => json.StatusChanged;

        /// <summary>
        /// The full street address component of the User's address.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.STREET_ADDRESS)]
        public string StreetAddress
        {
            get => json.Profile.StreetAddress;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.STREET_ADDRESS, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The User's time zone.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.TIME_ZONE)]
        public string TimeZone
        {
            get => json.Profile.TimeZone;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.TIME_ZONE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        public override string Title
        {
            get => HonorificPrefix;
            set
            {
                HonorificPrefix = value;
            }
        }

        /// <summary>
        /// The target status of an in-progress asynchronous status transition
        /// of the User.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.TRANSITIONING_TO_STATUS)]
        public string TransitioningToStatus => json.TransitioningToStatus;

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        [DirectorySystemPropertyName(UserJson.TYPE)]
        public override string Type => json.Type.Id;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public override string UniqueId => Id;

        /// <summary>
        /// Used to describe the organization to user relationship such as "Employee"
        /// or "Contractor".
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.USER_TYPE)]
        public string UserType
        {
            get => json.Profile.UserType;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.USER_TYPE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// The ZIP code or postal code component of the User's address.
        /// </summary>
        [DirectorySystemPropertyName(UserProfileJson.ZIP_CODE)]
        public string ZipCode
        {
            get => json.Profile.ZipCode;
            set
            {
                // Create the identity attribute object with the value set.
                IdentityAttribute<string> attribute = new(UserProfileJson.ZIP_CODE, value);

                // Update the user with the new value.
                okta.UpdateUser(this, attribute);
            }
        }

        /// <summary>
        /// A hashed version of the user's Login to allow for faster compare operations.
        /// </summary>
        public override int HashedIdentifier
        {
            get
            {
                return Login.GetHashCode();
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta user from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the user.</param>
        /// <param name="json">An object representing this user's JSON properties.</param>
        public User(OktaClient okta, UserJson json)
        {
            if (okta != null && json != null)
            {
                // Initialize the client.
                this.okta = okta;

                // Initialize the backing JSON data.
                this.json = json;
            }
            else
            {
                if (okta == null)
                {
                    throw new ArgumentNullException(nameof(okta));
                }
                else
                {
                    throw new ArgumentNullException(nameof(json));
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
            return okta.SuspendUser(UniqueId);
        }

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public override bool Enable()
        {
            return okta.UnsuspendUser(UniqueId);
        }

        /// <summary>
        /// Gets the correct property name to use when searching or filtering for the property with the supplied name.
        /// </summary>
        /// <param name="name">The name of the User property to get the search name of.</param>
        /// <returns>The property's name to use while searching, or null if that property is not supported.</returns>
        public static string GetSearchPropertyName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Check where the property is sourced from.
                if (OktaClient.GetAllJsonPropertyNames(typeof(UserJson)).Contains(name))
                {
                    // The property is sourced from UserJson.
                    switch (name)
                    {
                        case UserJson.ACTIVATED:
                            return UserJson.ACTIVATED;
                        case UserJson.CREATED:
                            return UserJson.CREATED;
                        case UserJson.ID:
                            return UserJson.ID;
                        case UserJson.LAST_UPDATED:
                            return UserJson.LAST_UPDATED;
                        case UserJson.STATUS:
                            return UserJson.STATUS;
                        case UserJson.STATUS_CHANGED:
                            return UserJson.STATUS_CHANGED;
                        case UserJson.TYPE:
                            return "type.id";
                        default:
                            return null;
                    }
                }
                else if (OktaClient.GetAllJsonPropertyNames(typeof(UserProfileJson)).Contains(name))
                {
                    // A prefix to use before all profile properties.
                    const string PROFILE_PREFIX = "profile.";

                    // The property is sourced from UserProfileJson.
                    return PROFILE_PREFIX + name;
                }
                else
                {
                    // Who knows where this is sourced?
                    return null;
                }
            }
            else
            {
                // No property name supplied.
                return null;
            }
        }

        /// <summary>
        /// Gets the UserProfileJson object. Needed for performing update operations.
        /// </summary>
        /// <returns>The Users UserProfileJson object.</returns>
        public UserProfileJson GetUserProfileJson()
        {
            return json.Profile;
        }

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public override bool SetPassword(string password)
        {
            // Create the password object with the value of the password set.
            UserPasswordJson passwordJson = new()
            {
                Value = password
            };

            // Create the credentials object with the password set.
            UserCredentialsJson creds = new()
            {
                Password = passwordJson
            };

            // Update the user with the new credentials.
            if (okta.UpdateUser(UniqueId, creds: creds) != null)
            {
                // The credentials were updated.
                return true;
            }
            else
            {
                // The credentials were not updated.
                return false;
            }
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public override bool Unlock()
        {
            return okta.UnlockUser(UniqueId);
        }
    }
}
