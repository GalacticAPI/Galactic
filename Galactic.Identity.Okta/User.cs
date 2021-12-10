using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Galactic.Identity.Okta
{
    public class User : IUser
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
        [OktaPropertyName(UserJson.ACTIVATED)]
        public DateTime? Activated => json.Activated;

        /// <summary>
        /// The user's city.
        /// </summary>
        [OktaPropertyName(UserProfileJson.CITY)]
        public string City
        {
            get => json.Profile.City;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    City = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// Name of a cost center assigned to the User.
        /// </summary>
        [OktaPropertyName(UserProfileJson.COST_CENTER)]
        public string CostCenter
        {
            get => json.Profile.CostCenter;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    CostCenter = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        [OktaPropertyName(UserProfileJson.COUNTRY_CODE)]
        public string CountryCode
        {
            get => json.Profile.CountryCode;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    CountryCode = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// Timestamp when User was created.
        /// </summary>
        [OktaPropertyName(UserJson.CREATED)]
        public DateTime? Created
        {
            get => json.Created;
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => Created;

        /// <summary>
        /// The user's department.
        /// </summary>
        [OktaPropertyName(UserProfileJson.DEPARTMENT)]
        public string Department
        {
            get => json.Profile.Department;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Department = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        [OktaPropertyName(UserProfileJson.DISPLAY_NAME)]
        public string DisplayName
        {
            get => json.Profile.DisplayName;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    DisplayName = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The User's division.
        /// </summary>
        [OktaPropertyName(UserProfileJson.DIVISION)]
        public string Division
        {
            get => json.Profile.Division;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Division = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The User's primary e-mail address.
        /// </summary>
        [OktaPropertyName(UserProfileJson.EMAIL)]
        public string Email
        {
            get => json.Profile.Email;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Email = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public List<string> EmailAddresses
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
        [OktaPropertyName(UserProfileJson.EMPLOYEE_NUMBER)]
        public string EmployeeNumber
        {
            get => json.Profile.EmployeeNumber;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    EmployeeNumber = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [OktaPropertyName(UserProfileJson.FIRST_NAME)]
        public string FirstName
        {
            get => json.Profile.FirstName;
            set
            {
                {
                    // Create the profile object with the value set.
                    UserProfileJson profile = new UserProfileJson
                    {
                        FirstName = value
                    };

                    // Update the user with the new value.
                    okta.UpdateUser(UniqueId, profile);
                }
            }
        }

        /// <summary>
        /// The list of groups this object is a member of, or null if they couldn't be retrieved.
        /// </summary>
        public List<IGroup> Groups
        {
            get
            {
                // Retrieve the list of JSON objects representing the groups.
                List<GroupJson> groupJsonList = okta.GetUserGroups(UniqueId);

                if (groupJsonList != null)
                {
                    // Create Group objects from the JSON.
                    List<IGroup> groups = new();
                    foreach (GroupJson json in groupJsonList)
                    {
                        groups.Add(new Group(okta, json) as IGroup);
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
        [OktaPropertyName(UserProfileJson.HONORIFIC_PREFIX)]
        public string HonorificPrefix
        {
            get => json.Profile.HonorificPrefix;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    HonorificPrefix = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The honorific suffix(es) of the User.
        /// </summary>
        [OktaPropertyName(UserProfileJson.HONORIFIC_SUFFIX)]
        public string HonorificSuffix
        {
            get => json.Profile.HonorificSuffix;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    HonorificSuffix = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The User's unique key.
        /// </summary>
        [OktaPropertyName(UserJson.ID)]
        public string Id => json.Id;


        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public bool IsDisabled
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
        [OktaPropertyName(UserJson.LAST_LOGIN)]
        public DateTime? LastLogin => json.LastLogin;

        /// <summary>
        /// The user's last name.
        /// </summary>
        [OktaPropertyName(UserProfileJson.LAST_NAME)]
        public string LastName
        {
            get => json.Profile.LastName;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    LastName = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// Timestamp when User was last updated.
        /// </summary>
        [OktaPropertyName(UserJson.LAST_UPDATED)]
        public DateTime? LastUpdated => json.LastUpdated;

        /// <summary>
        /// The User's default location for purposes of localizing items such as
        /// currency, date time format, numerical representations, etc.
        /// </summary>
        [OktaPropertyName(UserProfileJson.LOCALE)]
        public string Locale
        {
            get => json.Profile.Locale;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Locale = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        [OktaPropertyName(UserProfileJson.LOGIN)]
        public string Login
        {
            get => json.Profile.Login;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Login = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The display name of the User's manager.
        /// </summary>
        [OktaPropertyName(UserProfileJson.MANAGER)]
        public string Manager
        {
            get => json.Profile.Manager;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Manager = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        [OktaPropertyName(UserProfileJson.MANAGER_ID)]
        public string ManagerId
        {
            get => json.Profile.ManagerId;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    ManagerId = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public string ManagerName => Manager;

        /// <summary>
        /// The user's middle name.
        /// </summary>
        [OktaPropertyName(UserProfileJson.MIDDLE_NAME)]
        public string MiddleName
        {
            get => json.Profile.MiddleName;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    MiddleName = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        [OktaPropertyName(UserProfileJson.MOBILE_PHONE)]
        public string MobilePhone
        {
            get => json.Profile.MobilePhone;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    MobilePhone = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The casual way to address the User in real life.
        /// </summary>
        [OktaPropertyName(UserProfileJson.NICK_NAME)]
        public string NickName
        {
            get => json.Profile.NickName;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    NickName = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        [OktaPropertyName(UserProfileJson.ORGANIZATION)]
        public string Organization
        {
            get => json.Profile.Organization;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    Organization = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// Timestamp when User's password was last changed.
        /// </summary>
        [OktaPropertyName(UserJson.PASSWORD_CHANGED)]
        public DateTime? PasswordChanged => json.PasswordChanged;

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public bool PasswordChangeRequiredAtNextLogin
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
        public bool PasswordExpired
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
        public DateTime? PasswordLastSet => PasswordChanged;

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public string PhyscialAddress
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
        [OktaPropertyName(UserProfileJson.POSTAL_ADDRESS)]
        public string PostalAddress
        {
            get => json.Profile.PostalAddress;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    PostalAddress = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public string PostalCode
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
        public string PrimaryEmailAddress
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
        [OktaPropertyName(UserProfileJson.PRIMARY_PHONE)]
        public string PrimaryPhone
        {
            get => json.Profile.PrimaryPhone;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    PrimaryPhone = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The URL of the User's online profile (e.g. web page).
        /// </summary>
        [OktaPropertyName(UserProfileJson.PROFILE_URL)]
        public string ProfileUrl
        {
            get => json.Profile.ProfileUrl;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    ProfileUrl = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The secondary e-mail address of the User, typically used for account
        /// recovery.
        /// </summary>
        [OktaPropertyName(UserProfileJson.SECOND_EMAIL)]
        public string SecondEmail
        {
            get => json.Profile.SecondEmail;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    SecondEmail = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        [OktaPropertyName(UserProfileJson.STATE)]
        public string State
        {
            get => json.Profile.State;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    State = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The User's current status.
        /// </summary>
        [OktaPropertyName(UserJson.STATUS)]
        public string Status => json.Status;

        /// <summary>
        /// Timestamp when User's status last changed.
        /// </summary>
        [OktaPropertyName(UserJson.STATUS_CHANGED)]
        public DateTime? StatusChanged => json.StatusChanged;

        /// <summary>
        /// The full street address component of the User's address.
        /// </summary>
        [OktaPropertyName(UserProfileJson.STREET_ADDRESS)]
        public string StreetAddress
        {
            get => json.Profile.StreetAddress;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                   StreetAddress = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The User's time zone.
        /// </summary>
        [OktaPropertyName(UserProfileJson.TIME_ZONE)]
        public string TimeZone
        {
            get => json.Profile.TimeZone;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    TimeZone = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        public string Title
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
        [OktaPropertyName(UserJson.TRANSITIONING_TO_STATUS)]
        public string TransitioningToStatus => json.TransitioningToStatus;

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        [OktaPropertyName(UserJson.TYPE)]
        public string Type => json.Type;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId => Id;

        /// <summary>
        /// Used to describe the organization to user relationship such as "Employee"
        /// or "Contractor".
        /// </summary>
        [OktaPropertyName(UserProfileJson.USER_TYPE)]
        public string UserType
        {
            get => json.Profile.UserType;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                    UserType = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
            }
        }

        /// <summary>
        /// The ZIP code or postal code component of the User's address.
        /// </summary>
        [OktaPropertyName(UserProfileJson.ZIP_CODE)]
        public string ZipCode
        {
            get => json.Profile.ZipCode;
            set
            {
                // Create the profile object with the value set.
                UserProfileJson profile = new UserProfileJson
                {
                   ZipCode = value
                };

                // Update the user with the new value.
                okta.UpdateUser(UniqueId, profile);
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
            return okta.SuspendUser(UniqueId);
        }

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public bool Enable()
        {
            return okta.UnsuspendUser(UniqueId);
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
                    foreach (OktaPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<OktaPropertyNameAttribute>())
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
                    foreach (OktaPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<OktaPropertyNameAttribute>())
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
        public bool Unlock()
        {
            return okta.UnlockUser(UniqueId);
        }
    }
}
